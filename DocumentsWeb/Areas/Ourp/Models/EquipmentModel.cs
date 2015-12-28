using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Ourp.Models
{
	public class EquipmentModel
	{
		public string ModelId { get; set; }
		/// <summary>Идентификатор</summary>
		public int Id { get; set; }
        /// <summary>Идентификатор родительского элемента</summary>
        public int ParentId { get; set; }
		/// <summary>Идентификатор типа</summary>
		public int KindId { get; set; }
		/// <summary>Наименование</summary>
		public string Name { get; set; }
		/// <summary>Печатное наименование</summary>
		public string NameFull { get; set; }
		/// <summary>Код валюты</summary>
		public string Code { get; set; }
		/// <summary>Код поиска</summary>
		public string CodeFind { get; set; }
		/// <summary>Примечание</summary>
		public string Memo { get; set; }
		/// <summary>Основная група</summary>
		public string DefaultGroup { get; set; }
		/// <summary>Имя пользователя создавшего/изменившего аналитику</summary>
		public string UserName { get; set; }
		/// <summary>Имя состояния</summary>
		public string StateName { get; set; }
		/// <summary>Идентификатор состояния</summary>
		public int StateId { get; set; }
		/// <summary>Идентификатор компании владельца</summary>
		public int MyCompanyId { get; set; }
		/// <summary>Компания</summary>
		public string MyCompanyName { get; set; }
		/// <summary>Дата изменения</summary>
		public DateTime? DateModified { get; set; }
		/// <summary>Только для чтения</summary>
		public bool IsReadOnly { get; set; }
		/// <summary>Флаг "Системный"</summary>
		public bool IsSystem { get; set; } //{ get { return (FlagsValue & FlagValue.FLAGSYSTEM) == FlagValue.FLAGSYSTEM; } }
		/// <summary>Флаг</summary>
		public int FlagsValue { get; set; }
		/// <summary>Id создавшего пользователя</summary>
		public int UserOwnerId { get; set; }
		/// <summary>Номер чертежа</summary>
		public string DrawingNumber { get; set; }
		/// <summary>Номер сборочного чертежа</summary>
		public string DrawingAssemblyNumber { get; set; }
		/// <summary>Масса</summary>
		public decimal Weight { get; set; }
		/// <summary>Id Типа транспортного средства</summary>
		public int AutoTypeId { get; set; }

		public EquipmentModel()
        {
            ModelId = Guid.NewGuid().ToString();
            StateId = State.STATEACTIVE;
        }

		/// <summary>Преобразование модели в объект</summary>
		/// <returns></returns>
        public Equipment ToObject()
		{
			Equipment obj = new Equipment();
			if (Id == 0)
				obj = new Equipment { Workarea = WADataProvider.WA };
			else
				obj = WADataProvider.WA.Cashe.GetCasheData<Equipment>().Item(Id);

			if (obj.Id == 0)
			{
				obj.IsNew = true;
				obj.StateId = State.STATEACTIVE;
				obj.Id = Id;
				obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
				obj.MyCompanyId = MyCompanyId == 0 ? WADataProvider.CurrentUser.MyCompanyId : MyCompanyId;
			}
			obj.Name = Name;
			obj.KindId = KindId;
			obj.NameFull = NameFull;
			obj.Code = Code;
			obj.CodeFind = CodeFind;
			obj.Memo = Memo;
			obj.UserOwnerId = UserOwnerId;
			obj.DrawingNumber = DrawingNumber;
			obj.DrawingAssemblyNumber = DrawingAssemblyNumber;
			obj.Weight = Weight;
			obj.AutoTypeId = AutoTypeId;
			return obj;
		}

		public static List<EquipmentModel> GetCollection(int id, bool refresh = false)
		{
			List<EquipmentModel> list = new List<EquipmentModel>();
			return list;
		}

		public static EquipmentModel ConvertToModel(Equipment value)
		{
			EquipmentModel obj = new EquipmentModel
			{
				Id = value.Id,
				KindId = value.KindId,
				Name = value.Name,
				NameFull = value.NameFull,
				Code = value.Code,
				CodeFind = value.CodeFind,
				Memo = value.Memo,
				UserName = value.UserName,
				StateName = value.State.Name,
				StateId = value.StateId,
				DateModified = value.DateModified,
				MyCompanyId = value.MyCompanyId,
				FlagsValue = value.FlagsValue,
				UserOwnerId = value.UserOwnerId,
				DrawingNumber = value.DrawingNumber,
				DrawingAssemblyNumber = value.DrawingAssemblyNumber,
				Weight = value.Weight,
				AutoTypeId = value.AutoTypeId
			};
			obj.IsReadOnly = value.IsReadOnly;
			obj.IsSystem = value.IsSystem;
			if (value.MyCompanyId != 0)
			{
				obj.MyCompanyName = value.MyCompany.Name;
			}
			if (value.FirstHierarchy() != null)
				obj.DefaultGroup = value.FirstHierarchy().Name;
			return obj;
		}

		public static EquipmentModel GetObject(int id)
        {
			Equipment e = WADataProvider.WA.Cashe.GetCasheData<Equipment>().Item(id);
			return EquipmentModel.ConvertToModel(e);
        }
	}
}