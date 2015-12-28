using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Workflow.Activities.Rules;
using BusinessObjects;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Models
{
    public class TaskModel : BusinessObjects.Models.TaskModel, 
        IModelData, 
        INotesOwner, 
        IFileOwner,
        IValidatableObject
    {
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!DateStartPlanTimeAsDate.HasValue)
            {
                yield return new ValidationResult("Время планового старта задачи обязательно", new[] { GlobalPropertyNames.DateStartPlanTimeAsDate });
            }
            if (!DateStartTimeAsDate.HasValue)
            {
                yield return new ValidationResult("Время старта задачи обязательно", new[] { GlobalPropertyNames.DateStartTimeAsDate });
            }
        
            if (!DateEndPlanTimeAsDate.HasValue)
            {
                yield return new ValidationResult("Время планового окончания задачи обязательно", new[] { GlobalPropertyNames.DateEndPlanTimeAsDate });
            }
        
            if (!DateEndTimeAsDate.HasValue)
            {
                yield return new ValidationResult("Время окончания задачи обязательно", new[] { GlobalPropertyNames.DateEndTimeAsDate });
            }

            if (UserOwnerId==0)
            {
                yield return new ValidationResult("Автор задачи обязателен", new[] { GlobalPropertyNames.UserOwnerId });
            }
        
            if (PriorityId==0)
            {
                yield return new ValidationResult("Приоритет задачи обязателен", new[] { GlobalPropertyNames.PriorityId });
            }

            if (TaskStateId==0)
            {
                yield return new ValidationResult("Состояние задачи обязателен", new[] { GlobalPropertyNames.TaskStateId });
            }
            if (TaskNumber<0)
            {
                yield return new ValidationResult("Номер задачи обязателен", new[] { GlobalPropertyNames.TaskNumber });
            }

            if (DonePersent<0)
            {
                yield return new ValidationResult("Процент выполнения задачи от 0 до 100", new[] { GlobalPropertyNames.DonePersent });
            }
        
            if (UserToId==0)
            {
                yield return new ValidationResult("Исполнитель обязателен", new[] { GlobalPropertyNames.UserToId });
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
        //public int ModelUserOwnerId { get; set; }
        /// <summary>
        /// Количество незавершенных задач
        /// </summary>
        /// <returns></returns>
        public static int GetIncomingTaskCount()
        {
            Analitic anStateDone =
                WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(Task.TASK_STATE_INPROGRESS);
            return
                WADataProvider.WA.GetCollection<Task>(true).Where(f => f.UserToId == WADataProvider.CurrentUser.Id && f.TaskStateId == anStateDone.Id).Count();
        }

        /// <summary>
        /// Список последних незавершенных задач
        /// </summary>
        /// <returns></returns>
        public static List<TaskModel> GetLastMyTask()
        {
            Analitic anStateDone =
                WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(Task.TASK_STATE_INPROGRESS);
            return WADataProvider.WA.GetCollection<Task>().Where(f => f.UserToId == WADataProvider.CurrentUser.Id && f.TaskStateId == anStateDone.Id).ToList().OrderByDescending(f => f.DateStart).Take(5).Select(s => ConvertToModel(s)).ToList();
            
        }

        /// <summary>
        /// Количество незавершенных задач поставленных мною
        /// </summary>
        /// <returns></returns>
        public static int GetOutcommingTaskCount()
        {
            Analitic anStateDone =
                WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(Task.TASK_STATE_INPROGRESS);
            return
                WADataProvider.WA.GetCollection<Task>(true).Where(f => f.UserOwnerId == WADataProvider.CurrentUser.Id && f.TaskStateId == anStateDone.Id).Count();
        }

        /// <summary>
        /// Список последних незавершенных задач поставленных мною, исключая назначенные мне
        /// </summary>
        /// <returns></returns>
        public static List<TaskModel> GetLastOutcommingTask()
        {
            Analitic anStateDone =
                WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(Task.TASK_STATE_INPROGRESS);
            return WADataProvider.WA.GetCollection<Task>().Where(f => f.UserOwnerId == WADataProvider.CurrentUser.Id && f.UserToId != WADataProvider.CurrentUser.Id && f.TaskStateId == anStateDone.Id).ToList().OrderByDescending(f => f.DateStart).Take(5).Select(s => ConvertToModel(s)).ToList();

        }


        public string InHierarchies{get; set; }

        /// <summary>Ид иерархии, в которой следует создать задачу</summary>
        public int HierarchyId { get; set; }

        public TaskModel()
        {
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }

        /// <summary>Файлы, связанные с задачей</summary>
        public List<FileDataModel> Files { get; set; }

        /// <summary>Примечания, связанные с задачей</summary>
        public List<NoteModel> Notes { get; set; }
        
        public static TaskModel ConvertToModel(Task val, bool partialLoad=true)
        {
            TaskModel obj = new TaskModel();
            obj.GetData(val);

            obj.DateStartPlanTimeAsDate = Period.TimeSpan2DateTime(val.DateStartPlanTime);
            obj.DateStartTimeAsDate = Period.TimeSpan2DateTime(val.DateStartTime);

            obj.DateEndPlanTimeAsDate = Period.TimeSpan2DateTime(val.DateEndPlanTime);
            obj.DateEndTimeAsDate = Period.TimeSpan2DateTime(val.DateEndTime);

            
            obj.InHierarchies = string.Join(",",HierarchyModel.GetHierarchiesWith<Task>(val).Select(s => s.Id.ToString()));
            if (!partialLoad)
            {
                obj.ReloadOnEdit = false;
                obj.Files = val.GetLinkedFiles().Where(s => s.StateId != State.STATEDELETED).Select(s => FileDataModel.ConvertToModel(s.Right)).ToList();
                obj.Notes = val.GetLinkedNotes().Where(s => s.StateId != State.STATEDELETED).Select(s => NoteModel.ConvertToModel(s.Right)).ToList();
            }
            else
            {
                obj.ReloadOnEdit = true;
            }

            //if (!WADataProvider.ModelsCache.Exists(obj.ModelId))
            //{
            //    WADataProvider.ModelsCache.Add(obj.ModelId, obj);
            //}

            return obj;
        }

        public static TaskModel GetObject(int id, bool partialLoad=true)
        {
            return ConvertToModel(WADataProvider.WA.GetObject<Task>(id), partialLoad);
        }

        public Task ToObject()
        {
            Task obj = new Task {Workarea = WADataProvider.WA};
            if (Id == 0)
            {
                obj.KindId = Task.KINDID_TASKUSER;
                obj.StateId = State.STATEACTIVE;
                obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
            }
            else
                obj.Load(Id);

            obj.UserName = WADataProvider.CurrentMembershipUser.UserName;

            string memo = Regex.Replace(Memo, @"<(.|\n)*?>", string.Empty);
            if (memo.EndsWith("."))
                memo = memo.Substring(0, memo.Length - 1);
            if (!string.IsNullOrEmpty(memo) && memo.Length > 150 && string.IsNullOrEmpty(Name))
                obj.Name = string.IsNullOrEmpty(Name) ? memo.Substring(0, Math.Min(memo.Length, 147)) + "..." : Name;
            else if (!string.IsNullOrEmpty(memo) && string.IsNullOrEmpty(Name) && memo.Length < 151)
            {
                obj.Name = string.IsNullOrEmpty(Name) ? memo : Name;
                if (obj.Name.EndsWith("."))
                    obj.Name = obj.Name.Substring(0, obj.Name.Length - 1);
            }
            else
            {
                obj.Name = string.IsNullOrEmpty(Name) ? "Задача": Name;
                if (obj.Name.EndsWith("."))
                    obj.Name = obj.Name.Substring(0, obj.Name.Length - 1);
            }
            obj.MemoTxt = memo;
            obj.UserName = UserName;
            obj.NameFull = NameFull;
            //obj.StateId = StateId;
            obj.Code = Code;
            //obj.CodeFind = CodeFind;
            //obj.KindId = KindId;
            obj.FlagString = FlagString;
            obj.Memo = Memo;
            obj.DateEnd=DateEnd;
            obj.DateEndPlan=DateEndPlan;
            obj.DateEndPlanTime = Period.DateTime2TimeSpan(DateEndPlanTimeAsDate);
            obj.DateEndTime = Period.DateTime2TimeSpan(DateEndTimeAsDate);
            obj.DateStart = DateStart;
            obj.DateStartPlan = DateStartPlan;
            obj.DateStartPlanTime = Period.DateTime2TimeSpan(DateStartPlanTimeAsDate);
            obj.DateStartTime = Period.DateTime2TimeSpan(DateStartTimeAsDate);
            obj.DonePersent = DonePersent;
            obj.InPrivate = InPrivate;
            obj.PriorityId= PriorityId;
            obj.TaskNumber = TaskNumber;
            obj.TaskStateId = TaskStateId;
            obj.UserOwnerId = UserOwnerId;
            obj.UserToId = UserToId;

            if (IsReadOnly)
                obj.FlagsValue |= FlagValue.FLAGREADONLY;
            else
                obj.FlagsValue &= ~FlagValue.FLAGREADONLY;

            return obj;
        }

        public static List<TaskModel> GetCollection(int HierarchyId, bool Refresh = false, bool nested=false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().Item(HierarchyId);
            List<Task> coll = h.GetTypeContents<Task>(nested, Refresh);
            if (Refresh) WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);
            return coll.Where(s => WADataProvider.IsCompanyIdAllowIdToCurrentUser(s.MyCompanyId)).Select(s=> ConvertToModel(s, true)).OrderByDescending(o => o.DateStart).ToList();
        }

        public static void ToTrash(int id, string hierarchyCode)
        {
            Task obj = WADataProvider.WA.Cashe.GetCasheData<Task>().Item(id);
            obj.Remove();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id, string hierarchyCode)
        {
            if (id != 0)
            {
                Task obj = WADataProvider.WA.Cashe.GetCasheData<Task>().Item(id);
                Task newObj = Task.CreateCopy(obj);
                newObj.Name += " (копия)";
                newObj.Save();
                string hies = string.Join(",", HierarchyModel.GetHierarchiesWith<Task>(obj).Select(s => s.Id.ToString()).ToArray<string>());
                HierarchyModel.UpdateHierarchiesContent<Task>(newObj, hies, hierarchyCode);
                Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id, string hierarchyCode)
        {
            Task obj = WADataProvider.WA.Cashe.GetCasheData<Task>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id, string hierarchyCode)
        {
            Task obj = WADataProvider.WA.Cashe.GetCasheData<Task>().Item(id);
            obj.StateId = State.STATEACTIVE;
            try
            {
                obj.Save();
            }
            catch (DatabaseException dbex)
            {
                if (dbex.Id != 0)
                {
                    ErrorLog err = WADataProvider.WA.GetErrorLog(dbex.Id);
                    if (err != null && err.Message.Contains("конфликт версий"))
                    {
                        obj = WADataProvider.WA.GetObject<Task>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id, string hierarchyCode)
        {
            Task obj = WADataProvider.WA.Cashe.GetCasheData<Task>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            WADataProvider.CacheTaskModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        /// <summary>
        /// Текущее значение аттрибутов в виде строки
        /// </summary>
        /// <returns></returns>
        public string FlagsValueString()
        {
            return ToObject().FlagsValueString();
        }
    }
}