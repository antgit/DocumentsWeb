using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Documents;
using DocumentsWeb.Models;
using DocumentsWeb.Areas.Agents.Models;

namespace DocumentsWeb.Areas.Routes.Models
{
    /// <summary>
    /// Строка детализации шаблона маршрута
    /// </summary>
    public class RouteTemplateDetailModel : DocumentDetailBaseModel
    {
        /// <summary>
        /// Идентификатор корреспондента
        /// </summary>
        public int AgentId { get; set; }

        /// <summary>
        /// Имя корреспондента
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// Идентификатор адреса корреспондента
        /// </summary>
        public int AddressId { get; set; }

        /// <summary>
        /// Строковое представление адреса корреспондента
        /// </summary>
        public string AddressName { get; set; }

        /// <summary>
        /// Строковое представление состояния
        /// </summary>
        public string StateName { get; set; }

        /// <summary>
        /// Плановое время посещения
        /// </summary>
        public DateTime PlanTime { get; set; }

        /// <summary>
        /// Позиция
        /// </summary>
        public int OrderNo { get; set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        public RouteTemplateDetailModel()
        {
            RowId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// Преобразование строки шаблона маршрута в модель
        /// </summary>
        /// <param name="row">Строка шаблона маршрута</param>
        /// <returns>Модель строки шаблона маршрута</returns>
        public static RouteTemplateDetailModel ConvertToModel(DocumentDetailRoute row) 
        {
            AgentAddressModel addr = AgentAddressModel.GetMktgAddressByAgentId(row.AgentId);
            RouteTemplateDetailModel model = new RouteTemplateDetailModel
            {
                Id = row.Id,
                AddressId = row.AddressId,
                AddressName = addr == null ? "" : addr.GetShortName(),
                AgentId = row.AgentId,
                AgentName = row.Agent.Name,
                StateId = row.StateId,
                StateName = row.State.Name,
                Guid = row.Guid,
                OwnerId = row.OwnerId,
                PlanTime = row.PlanTime.HasValue ? new DateTime().Add(row.PlanTime.Value) : new DateTime(),
                OrderNo = row.OrderNo
            };
            return model;
        }

        /// <summary>
        /// Преобразование модели в строку шаблона маршрута
        /// </summary>
        /// <returns>Строка шаблона маршрута</returns>
        public DocumentDetailRoute ToObject(DocumentRoute doc)
        {
            DocumentDetailRoute row = new DocumentDetailRoute { Workarea = WADataProvider.WA };

            row.Document = doc;
            row.Id = Id;
            row.AddressId = AddressId;
            row.AgentId = AgentId;
            row.StateId = StateId;
            row.Guid = Guid;
            row.OwnerId = OwnerId;
            row.PlanTime = PlanTime.TimeOfDay;
            row.OrderNo = OrderNo;

            return row;
        }
    }
}