using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Documents;
using DocumentsWeb.Models;
using BusinessObjects;

namespace DocumentsWeb.Areas.Routes.Models
{
    /// <summary>
    /// Шаблон маршрута
    /// </summary>
    public class RouteTemplateModel
    {
        /// <summary>
        /// Идентификатор шаблона
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Идентификатор модели
        /// </summary>
        public string ModelId { get; set; }

        /// <summary>
        /// Имя шаблона
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Состояние
        /// </summary>
        public int StateId { get; set; }

        /// <summary>
        /// Строковое представление состояния
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// Идентификатор устройства
        /// </summary>
        public int DeviceId { get; set; }

        /// <summary>
        /// Наименование устройства
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// Идентификатор участника маршрута
        /// </summary>
        public int RouteMemberId { get; set; }

        /// <summary>
        /// Наименование участника маршрута
        /// </summary>
        public string RouteMemberName { get; set; }

        public bool? Monday { get; set; }
        public bool? Tuesday { get; set; }
        public bool? Wednesday { get; set; }
        public bool? Thursday { get; set; }
        public bool? Friday { get; set; }
        public bool? Saturday { get; set; }
        public bool? Sunday { get; set; }

        /// <summary>
        /// Детализация шаблона
        /// </summary>
        public List<RouteTemplateDetailModel> Details { get; set; }

        /// <summary>
        /// Имя пользователя работавшего с шаблоном
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public RouteTemplateModel()
        {
            Details = new List<RouteTemplateDetailModel>();
            ModelId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Конвертирует маршрут в модель
        /// </summary>
        /// <param name="doc">Шаблон маршрута</param>
        /// <returns>Модель шаблона маршрута</returns>
        public static RouteTemplateModel ConvertToModel(DocumentRoute doc)
        {
            RouteTemplateModel model = new RouteTemplateModel
            {
                Id = doc.Id,
                Name = doc.Document.Name,
                RouteMemberId = doc.RouteMemberId,
                RouteMemberName = doc.RouteMember.Name,
                StateId = doc.StateId,
                StateName = doc.State.Name,
                DeviceId = doc.DeviceId,
                DeviceName = doc.Device.Name,
                Monday = doc.Monday,
                Tuesday = doc.Tuesday,
                Wednesday = doc.Wednesday,
                Thursday = doc.Thursday,
                Friday = doc.Friday,
                Saturday = doc.Saturday,
                Sunday = doc.Sunday,
                Details = doc.Details.Where(s => !s.IsStateDeleted).Select(RouteTemplateDetailModel.ConvertToModel).ToList()
            };
            return model;
        }

        /// <summary>
        /// Преобразует модель шаблона в документ
        /// </summary>
        /// <returns>Документ "Маршрут"</returns>
        public DocumentRoute ToObject()
        {
            DocumentRoute doc = new DocumentRoute { Workarea = WADataProvider.WA };
            if (Id != 0)
            {
                doc.Load(Id);
            }

            if (doc.Id == 0)
            {
                doc.IsNew = true;
                doc.Kind = DocumentRoute.KINDID_PLANFACT;
                UserName = HttpContext.Current.User.Identity.Name;
            }

            if (doc.Document == null)
            {
                // TODO: Сделать через код
                Folder fld = WADataProvider.WA.Cashe.GetCasheData<Folder>().Item(117);//ItemCode<Folder>(Folder.CODE_FIND_ROUTE_ROUTE);
                doc.Document = new Document { Workarea = WADataProvider.WA };
                doc.Document.KindId = DocumentRoute.KINDID_PLANFACT;
                doc.Document.FolderId = fld.Id;
                doc.Document.ProjectItemId = fld.ProjectItem.Id;
                doc.Document.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
                doc.Document.DatabaseId = 1001;
                doc.Document.CurrencyId = 515;
                doc.DatabaseId = 1001;
                doc.Document.AgentFromId = 1;
                doc.Document.AgentDepartmentToId = 1;
                doc.Document.AgentDepartmentToId = 1;
                doc.Document.AgentDepartmentFromId = WADataProvider.CurrentUser.MyCompanyId;
                doc.Document.ClientId = WADataProvider.CurrentUser.MyCompanyId;
            }

            doc.Document.Name = Name;
            doc.StateId = StateId;
            doc.DeviceId = DeviceId;
            doc.RouteMemberId = RouteMemberId;

            doc.Date = DateTime.Now;
            doc.PlanDate = DateTime.Now;
            doc.Multiplier = 1;

            doc.Monday = Monday ?? false;
            doc.Tuesday = Tuesday ?? false;
            doc.Wednesday = Wednesday ?? false;
            doc.Thursday = Thursday ?? false;
            doc.Friday = Friday ?? false;
            doc.Saturday = Saturday ?? false;
            doc.Sunday = Sunday ?? false;

            doc.Details = Details.Select(s => s.ToObject(doc)).ToList();

            return doc;
        }

        /// <summary>
        /// Удаляет шаблон маршрута
        /// </summary>
        /// <param name="id">Идентификатор шаблона маршрута</param>
        public static void Delete(int id)
        {
            if (id > 0)
            {
                DocumentRoute route = new DocumentRoute { Workarea = WADataProvider.WA };
                route.Load(id);
                route.StateId = State.STATEDELETED;
                route.Save();
            }
        }
    }
}