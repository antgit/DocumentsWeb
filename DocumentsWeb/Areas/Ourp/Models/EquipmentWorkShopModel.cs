using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Ourp.Models
{
	public class EquipmentWorkShopModel
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

		public EquipmentWorkShopModel()
        {
            ModelId = Guid.NewGuid().ToString();
            StateId = State.STATEACTIVE;
        }

		/// <summary>Преобразование модели в объект</summary>
		/// <returns></returns>
        public Depatment ToObject()
		{
			Depatment obj = new Depatment();
			if (Id == 0)
				obj = new Depatment { Workarea = WADataProvider.WA };
			else
				obj = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(Id);

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
			return obj;
		}

		public static List<EquipmentWorkShopModel> GetCollection(int id, bool refresh = false)
		{
			List<EquipmentWorkShopModel> list = new List<EquipmentWorkShopModel>();
			return list;
		}

		public static EquipmentWorkShopModel ConvertToModel(Depatment value)
		{
			EquipmentWorkShopModel obj = new EquipmentWorkShopModel
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
				FlagsValue = value.FlagsValue
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

		public static EquipmentWorkShopModel GetObject(int id)
        {
			Depatment e = WADataProvider.WA.Cashe.GetCasheData<Depatment>().Item(id);
			return EquipmentWorkShopModel.ConvertToModel(e);
        }
	}
}