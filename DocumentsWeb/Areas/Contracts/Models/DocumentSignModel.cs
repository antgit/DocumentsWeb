using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Documents;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Contracts.Models
{
    public class DocumentSignModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Тип подписи
        /// </summary>
        public int Kind { get; set; }

        /// <summary>
        /// Ответственный
        /// </summary>
        public string AgentName { get; set; }
        public int AgentId
        {
            get
            {
                return _AgentId;
            }

            set
            {
                if (AgentName == null || AgentName.Length == 0)
                {
                    Agent a = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    AgentName = a.Name;
                }
                _AgentId = value;
            }
        }
        private int _AgentId;

        /// <summary>
        /// Заместитель
        /// </summary>
        public string AgentSubName { get; set; }
        public int AgentSubId
        {
            get
            {
                return _AgentSubId;
            }

            set
            {
                if (AgentSubName == null || AgentSubName.Length == 0)
                {
                    Agent a = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    AgentSubName = a.Name;
                }
                _AgentSubId = value;
            }
        }
        private int _AgentSubId;

        /// <summary>
        /// Корреспондент поставивший подпись
        /// </summary>
        public string AgentSignName { get; set; }
        public int AgentSignId
        {
            get
            {
                return _AgentSignId;
            }

            set
            {
                if (AgentSignName == null || AgentSignName.Length == 0)
                {
                    Agent a = WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    AgentSignName = a.Name;
                }
                _AgentSignId = value;
            }
        }
        private int _AgentSignId;

        /// <summary>
        /// Дата (подписать до)
        /// </summary>
        public DateTime DateTo { get; set; }

        /// <summary>
        /// Дата (фактическая дата подписи)
        /// </summary>
        public DateTime? DateSign { get; set; }

        /// <summary>
        /// Идентификатор сообщения
        /// </summary>
        public int MessageId { get; set; }

        /// <summary>
        /// Необходимость сообщения
        /// </summary>
        public bool MessageNeed { get; set; }

        /// <summary>
        /// Идентификатор задания
        /// </summary>
        public int TaskId { get; set; }

        /// <summary>
        /// Необходимость задания
        /// </summary>
        public bool TaskNeed { get; set; }

        /// <summary>
        /// Порядок подписания
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// Идентификатор документа
        /// </summary>
        public int OwnerId { get; set; }

        /// <summary>
        /// Резолюция
        /// </summary>
        public string ResolutionName { get; set; }
        public int ResolutionId
        {
            get
            {
                return _ResolutionId;
            }

            set
            {
                if (ResolutionName == null || ResolutionName.Length == 0)
                {
                    Analitic a = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(value);
                    ResolutionName = a.Name;
                }
                _ResolutionId = value;
            }
        }
        private int _ResolutionId;

        /// <summary>
        /// Тип подписи (ИЛИ/И)
        /// </summary>
        public bool SignKind { get; set; }

        /// <summary>
        /// Идентификатор состояния
        /// </summary>
        public int StateId { get; set; }

        /// <summary>Ид важности</summary>
        public int PriorityId { get; set; }
        /// <summary>Важность</summary>
        public string PriorityName { get; set; }
        /// <summary>Группа</summary>
        public int GroupNo { get; set; }

        public string DepatmentName { get; set; }

        /// <summary>
        /// Системный объект
        /// </summary>
        public bool IsSystem { get; set; }

        /// <summary>
        /// Только чтение
        /// </summary>
        public bool IsReadOnly { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Имя пользователя
        /// </summary>
        public string UserName { get; set; }

        public string RowId { get; set; }

        public DocumentSignModel()
        {
            RowId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Конвертирует объект в модель
        /// </summary>
        public static DocumentSignModel ConvertToModel(DocumentSign val)
        {
            DocumentSignModel obj = new DocumentSignModel
                                        {
                                            Id = val.Id,
                                            AgentId = val.AgentId,
                                            AgentName = val.AgentId > 0 ? val.Agent.Name : "",
                                            AgentSubId = val.AgentSubId,
                                            AgentSubName = val.AgentSubId > 0 ? val.AgentSub.Name : "",
                                            AgentSignId = val.AgentSignId,
                                            AgentSignName = val.AgentSignId > 0 ? val.AgentSign.Name : "",
                                            DateTo = val.Date,
                                            DateSign = val.DateSign,
                                            Kind = val.Kind,
                                            MessageId = val.MessageId,
                                            MessageNeed = val.MessageNeed,
                                            OrderNo = val.OrderNo,
                                            OwnerId = val.OwnId,
                                            ResolutionId = val.ResolutionId,
                                            ResolutionName = val.ResolutionId > 0 ? val.Resolution.Name : "",
                                            TaskId = val.TaskId,
                                            TaskNeed = val.TaskNeed,
                                            StateId = val.StateId,
                                            IsReadOnly = val.IsReadOnly,
                                            IsSystem = val.IsSystem,
                                            Memo = val.Memo,
                                            UserName = val.UserName,
                                            PriorityId = val.PriorityId,
                                            PriorityName = val.PriorityId == 0 ? string.Empty : val.Priority.Name,
                                            GroupNo = val.GroupNo,
                                            DepatmentName = val.DepatmentId == 0 ? string.Empty : val.Depatment.Name
                                        };
            return obj;
        }

        /// <summary>
        /// Конвертирует объект в модель
        /// </summary>
        public static DocumentSignModel ConvertToModel(int Id)
        {
            DocumentSign obj = WADataProvider.WA.Cashe.GetCasheData<DocumentSign>().Item(Id);
            return ConvertToModel(obj);
        }

        /// <summary>
        /// Конвертирует модель в объект
        /// </summary>
        public DocumentSign ToObject()
        {
            DocumentSign obj = new DocumentSign {Workarea = WADataProvider.WA};
            if (Id != 0)
                obj.Load(Id);
            else
            {
                obj.Id = Id;
                obj.Guid = Guid.NewGuid();
            }
            obj.AgentId = AgentId;
            obj.AgentSubId = AgentSubId;
            obj.AgentSignId = AgentSignId;
            obj.Date = DateTo;
            obj.DateSign = DateSign;
            obj.Kind = Kind;
            obj.MessageId = MessageId;
            obj.MessageNeed = MessageNeed;
            obj.OrderNo = OrderNo;
            obj.OwnId = OwnerId;
            obj.ResolutionId = ResolutionId;
            obj.TaskId = TaskId;
            obj.TaskNeed = TaskNeed;
            obj.StateId = StateId == 0 ? State.STATEACTIVE : StateId;
            obj.PriorityId = PriorityId;
            obj.GroupNo = GroupNo;
            obj.Memo = Memo;
            obj.UserName = UserName;
            return obj;
        }

        /// <summary>
        /// Возвращает коллекцию ответственных для документа
        /// </summary>
        /// <param name="DocumentId">Идентификатор документа</param>
        /// <returns>Коллекция ответственных для документа</returns>
        public static List<DocumentSignModel> GetCollection(int DocumentId)
        {
            Document doc = new Document { Workarea = WADataProvider.WA };
            doc.Load(DocumentId);
            return doc.Signs().Select(ConvertToModel).ToList();
        }
    }
}