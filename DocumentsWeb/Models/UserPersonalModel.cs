using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Security;
using BusinessObjects.Web.Core;
using DocumentsWeb.Areas.Agents.Models;
using DocumentsWeb.Areas.UserPersonal.Models;

namespace DocumentsWeb.Models
{
    public class UserPersonalModel
    {
        public UserPersonalModel()
        {
            ModelId = Guid.NewGuid().ToString();
        }
        public string ModelId { get; set; }
        public int Id { get; set; }

        [Display(Name = "Логин")]
        public string Name { get; set; }

        [Display(Name = "ФИО")]
        public string NameFull { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email адрес")]
        public string Email { get; set; }

        [Display(Name = "Примечание")]
        public string Memo { get; set; }

        [Display(Name = "Активен")]
        public bool IsActive { get; set; }
        [Display(Name = "Пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        /// <summary> Состояние</summary>
        public int StateId { get; set; }
        /// <summary>
        /// Имя состояни
        /// </summary>
        public string StateName { get; set; }
        /// <summary>
        /// Идентификатор компании
        /// </summary>
        public int CompanyId { get; set; }
        /// <summary>
        /// Идентификатор сотрудника
        /// </summary>
        public int WorkerId { get; set; }
        /// <summary>
        /// Наименование сотрудника
        /// </summary>
        public string WorkerName { get; set; }

        [Required]
        public string WorkerFirstName { get; set; }
        [Required]
        public string WorkerLastName { get; set; }
        [Required]
        public string WorkerMidleName { get; set; }

        public string WorkerAdress { get; set; }
        
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        
        public string Inn { get; set; }
        public bool IsSystem { get; set; }
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Компания в которой зарегестрирован 
        /// </summary>
        public string CompanyDefaultName { get; set; }


        public static bool CanSave(int id)
        {
            Uid uid = new Uid { Workarea = WADataProvider.WA };
            uid.Load(id);
            return !(uid.IsSystem | uid.IsReadOnly);
        }
        /// <summary>
        /// Преобразовать в объект Uid
        /// </summary>
        /// <param name="workarea"></param>
        /// <returns></returns>
        public Uid ToObject(Workarea workarea)
        {
            Uid uid = new Uid { Workarea = workarea };
            uid.Load(Id);
            if (uid.Id == 0)
            {
                uid.IsNew = true;
            }
            uid.Email = Email;
            uid.Memo = Memo;
            return uid;
        }
        /// <summary>
        /// Преобразование пользователя в модель
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static UserPersonalModel ConvertToModel(Uid value)
        {
            
            UserPersonalModel obj = new UserPersonalModel
                                {
                                    Id = value.Id,
                                    Name = value.Name,
                                    Email = value.Email,
                                    Memo = value.Memo,
                                    IsActive = value.IsStateActive,
                                    StateName = value.State.Name,
                                    Password = value.Password,
                                    WorkerId = value.AgentId,
                                    CompanyId = value.MyCompanyId,
                                    StateId = value.StateId,
                                    
                                };

            obj.IsReadOnly = value.IsReadOnly;
            obj.IsSystem = value.IsSystem;

            if (value.MyCompanyId != 0)
                obj.CompanyDefaultName = value.MyCompany.Name;
            if (value.Agent != null)
            {
                obj.NameFull = string.IsNullOrEmpty(value.Agent.NameFull) ? value.Agent.Name : value.Agent.NameFull;
                obj.WorkerId = value.AgentId;
                obj.WorkerName = value.Agent.Name;
                obj.WorkerAdress = value.Agent.AddressPhysical;
                obj.WorkerFirstName = value.Agent.People.FirstName;
                obj.WorkerLastName = value.Agent.People.LastName;
                obj.WorkerMidleName = value.Agent.People.MidleName;
                obj.Inn = value.Agent.CodeTax;
            }
            return obj;
        }
        /// <summary>
        /// Пользователь по идентификатору
        /// </summary>
        public static UserPersonalModel GetObject(int id)
        {
            Uid obj = WADataProvider.WA.Cashe.GetCasheData<Uid>().Item(id);
            return ConvertToModel(obj);
        }
    }
}