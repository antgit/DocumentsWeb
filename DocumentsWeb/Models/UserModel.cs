using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.UserPersonal.Models;
using DocumentsWeb.Code;

namespace DocumentsWeb.Models
{
    //public enum MessageRblFilter
    //{
    //    ForToday,
    //    ForWeek,
    //    ForMonth
    //}

    public sealed class UserModel: BusinessObjects.Models.UidModel, IModelData, IValidatableObject
    {
        public UserModel()
        {
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
            StateId = State.STATEACTIVE;
        }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult("Наименование обязательно", new[] { GlobalPropertyNames.Name });
            }

            if (string.IsNullOrEmpty(Email))
            {
                yield return new ValidationResult("Email обязателен", new[] { GlobalPropertyNames.Email });
            }

            if (string.IsNullOrEmpty(Password))
            {
                yield return new ValidationResult("Пароль обязателен", new[] { GlobalPropertyNames.Password });
            }

            if (!string.IsNullOrEmpty(Password) && Password.Length < Membership.Provider.MinRequiredPasswordLength)
            {
                yield return new ValidationResult(string.Format("Пароль должен быть не менее {0} символов", Membership.Provider.MinRequiredPasswordLength), new[] { GlobalPropertyNames.Password });
            }

            if (Id != 0 && MyCompanyId == 0)
            {
                yield return new ValidationResult("Укажите предприятие", new[] { GlobalPropertyNames.MyCompanyId });
            }

            if (EmployerId == 0)
            {
                yield return new ValidationResult("Укажите сотрудника", new[] { GlobalPropertyNames.EmployerId });
            }

            Type targetType = this.GetType();
            string fulltypename = targetType.FullName;
            IEnumerable<Ruleset> collRules = WADataProvider.WA.GetCollection<Ruleset>().Where(s => s.ActivityName == fulltypename && s.StateId == 1 && s.KindValue == Ruleset.KINDVALUE_WEBRULESET);
            //if (collRules == null) return true;

            foreach (Ruleset rule in collRules)
            {
                RuleSet ruleSetToValidate = rule.DeserializeRuleSet();
                if (ruleSetToValidate != null)
                {
                    RuleEngine engine = new RuleEngine(ruleSetToValidate, targetType);
                    engine.Execute(this);
                    if (this.Errors.Count > 0)
                    {
                        foreach (string k in this.Errors.Keys)
                        {
                            yield return new ValidationResult(Errors[k], new[] { k });
                        }
                    }
                }
            }

        }
        public string InHierarchies { get; set; }

        public static bool CanSave(int id)
        {
            Uid a = WADataProvider.WA.Cashe.GetCasheData<Uid>().Item(id);
            return !(a.IsSystem | a.IsReadOnly);
        }
        //[Required]
        //[Display(Name = "Логин")]
        //public string Name { get; set; }

        //[Display(Name = "ФИО")]
        //public string NameFull { get; set; }

        //[Required]
        //[DataType(DataType.EmailAddress)]
        //[Display(Name = "Email адрес")]
        //public string Email { get; set; }

        //[Display(Name = "Примечание")]
        //public string Memo { get; set; }

        //[Display(Name = "Администратор")]
        //public bool IsAdmin { get; set; }

        //[Display(Name = "Активен")]
        //public bool IsActive { get; set; }
        //[Display(Name = "Пароль")]
        //[DataType(DataType.Password)]
        //public string Password { get; set; }

        //[Display(Name = "Аватар")]
        //public string Avatar { get; set; }

        ///// <summary>
        ///// Компания в которой зарегестрирован сотрудник
        ///// </summary>
        //public string CompanyName { get; set; }

        //public string MyCompanyName { get { return CompanyName; } }

        ///// <summary>
        ///// Компания в которой зарегестрирован 
        ///// </summary>
        //public string CompanyDefaultName { get; set; }

        private List<UserModel> _groups;
        /// <summary>
        /// Группы пользователя
        /// </summary>
        public List<UserModel> Groups 
        { 
            get { 
                if (_groups == null) {
                    if (Id == 0)
                        _groups = new List<UserModel>();
                    else
                        _groups = WADataProvider.WA.GetObject<Uid>(Id).Groups.Select(ConvertToModel).ToList();
                }
                return _groups;
            } 
        }

        private ClientModel _worker=null;
        /// <summary>
        /// Сотрудник
        /// </summary>
        /// <returns></returns>
        public ClientModel GetWorker()
        {
            if (_worker == null && EmployerId.HasValue && EmployerId.Value!=0)
            {
                _worker = ClientModel.GetObject(EmployerId.Value);
            }
            return _worker;
            
        }
        
        public byte[] Photo
        {
            get { return GetPhoto(); }
        }
        public bool HasPhoto
        {
            get { return GetPhoto() != null; }
        }
        private byte[] _Photo;
        public byte[] GetPhoto()
        {
            if (!EmployerId.HasValue || EmployerId.Value == 0)
                return null;
            Agent ag = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(EmployerId.Value);
            if (ag == null) return null;

            if(ag.People.AgentPassport.Count>0)
            {
                //(_streamData == null || _streamData.All(v => v == 0));
                if(ag.People.AgentPassport[0].Signature==null || ag.People.AgentPassport[0].Signature.All(v => v == 0))
                {
                    return null;
                }
                else
                {
                    _Photo = ag.People.AgentPassport[0].Signature;
                    return _Photo;
                }
            }
            return null;
        }
        /// <summary>
        /// Преобразовать в объект Uid
        /// </summary>
        /// <param name="workarea"></param>
        /// <returns></returns>
        public Uid ToObject(Workarea workarea)
        {
            Uid uid = new Uid { Workarea = workarea };
            
            if (Id == 0)
            {
                uid.IsNew = true;
            }
            else
            {
                uid = WADataProvider.WA.Cashe.GetCasheData<Uid>().Item(Id);
            }
            uid.KindId = Uid.KINDID_USER;
            uid.AuthenticateKind = (int)AuthenticateKind.NoLogin;
            uid.StateId = IsActive ? State.STATEACTIVE : State.STATEDENY;
            //uid.Id = Id;
            uid.Name = Name;
            uid.Code = Name;
            uid.NameFull = NameFull;
            uid.Email = Email;
            uid.Memo = Memo;
            uid.Password = Password;
            uid.MyCompanyId = MyCompanyId;
            if(EmployerId.HasValue && EmployerId.Value!=0)
                uid.AgentId = EmployerId.Value;
            else
                uid.AgentId = 0;

            uid.AllowChangePassword = AllowChangePassword;
            uid.RecommendedDateChangePassword = RecommendedDateChangePassword;
            uid.AutogenerateNextPassword = AutogenerateNextPassword;
            uid.TimePeriodId = TimePeriodId.HasValue? TimePeriodId.Value:0;
            if (IsReadOnly)
                uid.SetFlagReadOnly();
            else
                uid.RemoveFlagReadOnly();
            return uid;
        }
        /// <summary>
        /// Преобразование пользователя в модель
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UserModel ConvertToModel(Uid value)
        {
            UserModel obj = new UserModel();
            obj.GetData(value);
            obj.ReloadOnEdit = false;
            return obj;
            //UserModel obj = new UserModel { 
            //    Id = value.Id, 
            //    Name = value.Name, 
            //    Email = value.Email, 
            //    Memo = value.Memo,
            //    IsAdmin = DataSecurityProvider.WA.Access.IsUserExistsInGroup(value.Name, Uid.GROUP_GROUPWEBADMIN), 
            //    UserName = value.UserName, 
            //    IsActive = value.IsStateActive,
            //    StateName = value.State.Name,
            //    Password = value.Password,
            //    WorkerId = value.AgentId,
            //    CompanyId = value.MyCompanyId,
            //    StateId = value.StateId,
            //    DateModified = value.DateModified
            //};
            
            //obj.IsReadOnly = value.IsReadOnly;
            //obj.IsSystem = value.IsSystem;

            //if (value.MyCompanyId != 0)
            //    obj.CompanyDefaultName = value.MyCompany.Name;
            //if(value.Agent!=null)
            //{
            //    obj.NameFull = string.IsNullOrEmpty(value.Agent.NameFull) ? value.Agent.Name : value.Agent.NameFull;
            //    obj.WorkerId = value.AgentId;
            //    obj.WorkerName = value.Agent.Name;
            //    string filePath = "~/Images/" + value.Id + ".png";
            //    if (System.IO.File.Exists(filePath))
            //    {
            //        obj.Avatar = value.Id.ToString();
            //    }
            //    else
            //    {
            //        obj.Avatar = "noavatar.png";
            //    }

            //    List<Agent> company = (value.Agent as IChains<Agent>).DestinationList(value.Workarea.WorkresChainId());
            //    if(company!=null && company.Count>0)
            //    {
            //        obj.CompanyName = company[0].Name;
            //        obj.CompanyId = company[0].Id;
            //    }
            //}
            //return obj;
        }
        /// <summary>
        /// Список пользователей
        /// </summary>
        public static List<UserModel> GetCollection(bool refresh=false)
        {
            Uid grp = WADataProvider.WA.Access.GetAllGroups().FirstOrDefault(s=>s.Name.ToUpper()==Uid.GROUP_GROUPWEBUSER.ToUpper());
            List<Uid> usersGroups = WADataProvider.WA.GetCollection<Uid>(refresh);
            List<Uid> resUsers = new List<Uid>();
            foreach (Uid user in usersGroups)
                if (user.KindValue == 1)
                {
                    bool flag = user.Groups.Any(group => group.Id == grp.Id);
                    if (flag)
                        resUsers.Add(user);
                }
            return resUsers.Select(ConvertToModel).ToList();
        }
        /// <summary>
        /// Пользователь по идентификатору
        /// </summary>
        public static UserModel GetObject(int id)
        {
            Uid obj = WADataProvider.WA.Cashe.GetCasheData<Uid>().Item(id);
            return ConvertToModel(obj);
        }       

        public List<Message> GetIncomingMessages()
        {
            List<Message> resultList = WebMessageModel.GetIncomingMessages(Id);
            return resultList;
        }

        public List<Message> GetOutcomingMessages()
        {
            List<Message> resultList = WebMessageModel.GetOutcomingMessages(Id);
            return resultList;
        }

        public List<Message> GetDeletedMessages()
        {
            List<Message> resultList = WebMessageModel.GetDeletedMessages(Id);
            return resultList;
        }

        public List<Message> GetRoughCopiesMessages()
        {
            List<Message> resultList = WebMessageModel.GetRoughCopiesMessages(Id);
            return resultList;
        }

        /// <summary>
        /// Получить список пользователей, а которых в именах будут проставлены имена соответствующих им корреспондентов
        /// </summary>
        /// <returns></returns>
        public static List<UserModel> GetUserAgents(int? currentSelectedId = null)
        {
            Uid grp = WADataProvider.WA.Access.GetAllGroups().FirstOrDefault(s => s.Name.ToUpper() == Uid.GROUP_GROUPWEBUSER.ToUpper());
            List<Uid> usersGroups = WADataProvider.WA.GetCollection<Uid>();
            List<Uid> resUsers = new List<Uid>();
            foreach (Uid user in usersGroups)
                if (user.KindValue == 1 && WADataProvider.IsCompanyIdAllowIdToCurrentUser(user.MyCompanyId) 
                    && WADataProvider.WA.Access.GetCompanyScopeView(user.Name).Exists(f=>f==WADataProvider.CurrentUser.MyCompanyId))
                {
                    bool flag = user.Groups.Any(group => group.Id == grp.Id);
                    if (flag)
                        resUsers.Add(user);
                }

            List<UserModel> userModelColl = resUsers.Select(ConvertToModel).ToList();
            foreach (UserModel user in userModelColl)
            {
                if (user.EmployerId != 0)
                {
                    user.Name = user.WorkerName;
                }
            }
            if (currentSelectedId.HasValue && currentSelectedId.Value != 0)
            {
                if (!userModelColl.Exists(f => f.Id == currentSelectedId.Value))
                {
                    UserModel objCurrent = UserModel.GetObject(currentSelectedId.Value);
                    if (objCurrent != null && objCurrent.Id != 0)
                    {
                        if (objCurrent.EmployerId != 0)
                            objCurrent.Name = objCurrent.WorkerName;
                        userModelColl.Add(objCurrent);
                    }
                }
            }
            return userModelColl.OrderBy(s => s.Name).ToList();
            //foreach (Uid user in resUsers)
            //    if(user.AgentId!=0)
            //        user.Name = user.Agent.Name;

            //return resUsers.Select(ConvertToModel).OrderBy(s=>s.Name).ToList();
        }

        public static List<UserModel> GetAllGroups()
        {
            List<UserModel> coll = WADataProvider.WA.Access.GetAllGroups().Select(ConvertToModel).ToList();
            return coll;
        }
        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject(WADataProvider.WA).FlagsValueString();
        }
    }
}