using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;
using System.Data.SqlClient;

namespace DocumentsWeb.Areas.Ourp.Models
{
	public class AutoModel
	{
		public string ModelId { get; set; }
		/// <summary>Идентификатор</summary>
		public int Id { get; set; }
		/// <summary>Имя пользователя создавшего/изменившего аналитику</summary>
		public string UserName { get; set; }
		/// <summary>Имя состояния</summary>
		public string StateName { get; set; }
		/// <summary>Идентификатор состояния</summary>
		public int StateId { get; set; }

		/// <summary>Принадлежность</summary>
		public int MyCompanyId { get; set; }
		/// <summary>Принадлежность(название организации)</summary>
		public string MyCompanyName { get; set; }
		/// <summary>Наименование</summary>
		public string Name { get; set; }
		/// <summary>Id модели автомобиля</summary>
		public int EquipmentId { get; set; }
		/// <summary>Наименование модели автомобиля</summary>
		public string EquipmentName { get; set; }
		/// <summary>Название типа модели автомобиля(авто/прицеп)</summary>
		public string EquipmentTypeName { get; set; }

		/// <summary>Дополнительный бак</summary>
		public bool AdditionalFuelTank { get; set; }
		/// <summary>Номер шасси</summary>
		public string AutoChassis { get; set; }
		/// <summary>Номер двигателя</summary>
		public string AutoMotor { get; set; }
		/// <summary>Номер тех. паспорта</summary>
		public string AutoPassportNumber { get; set; }
		/// <summary>Год выпуска</summary>
		public DateTime? DateOn { get; set; }
		/// <summary>Государственный номер</summary>
		public string GosNumber { get; set; }
		/// <summary>Полная масса</summary>
		public decimal GrossVehicleWeight { get; set; }
		/// <summary>Дата окончания страховки</summary>
		public DateTime? InsuranceDateExpiration { get; set; }
		/// <summary>Дата начала страховки</summary>
		public DateTime? InsuranceDateStart { get; set; }
		/// <summary>Номер страховки</summary>
		public string InsuranceNumber { get; set; }
		/// <summary>Отсекаемый объем слева</summary>
		public decimal InterceptedVolumesLeft { get; set; }
		/// <summary>Отсекаемый объем справа</summary>
		public decimal InterceptedVolumesRight { get; set; }
		/// <summary>Дата окончания лицензии</summary>
		public DateTime? LicenseDateExpiration { get; set; }
		/// <summary>Дата начала лицензии</summary>
		public DateTime? LicenseDateStart { get; set; }
		/// <summary>Номер лицензии</summary>
		public string LicenseNumber { get; set; }
		/// <summary>Паллетоместа</summary>
		public int Pallet { get; set; }
		/// <summary>Норма расхода топлива</summary>
		public decimal RateFuelConsumption { get; set; }
		/// <summary>Дата окончания санитарного пасспорта</summary>
		public DateTime? SanitaryDateExpiration { get; set; }
		/// <summary>Дата начала санитарного пасспорта</summary>
		public DateTime? SanitaryDateStart { get; set; }
		/// <summary>Номер санитарного пасспорта</summary>
		public string SanitaryNumber { get; set; }
		/// <summary>Id Вида ТС по тоннажу</summary>
		public int TonnageKindId { get; set; }
		/// <summary>Id Закрепленного за прицепом автомобиля</summary>
		public int TrailerUsedAutoId { get; set; }
		/// <summary>Собственная масса</summary>
		public decimal UnladenWeight { get; set; }
		/// <summary>Id Используемого топлива</summary>
		public int UsedFuelId { get; set; }
		/// <summary>Объем двигнателя</summary>
		public decimal VolumeVehicleEngine { get; set; }

		public AutoModel()
        {
            ModelId = Guid.NewGuid().ToString();
            StateId = State.STATEACTIVE;
        }

		/// <summary>Преобразование модели в объект</summary>
		/// <returns></returns>
        public Product ToObject()
		{
			Product obj = new Product();
			if (Id == 0)
				obj = new Product { Workarea = WADataProvider.WA };
			else
				obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(Id);

			if (obj.Id == 0)
			{
				obj.IsNew = true;
				obj.StateId = State.STATEACTIVE;
				obj.Id = Id;
				//obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
				//obj.MyCompanyId = MyCompanyId == 0 ? WADataProvider.CurrentUser.MyCompanyId : MyCompanyId;
			}
			obj.MyCompanyId = MyCompanyId == 0 ? WADataProvider.CurrentUser.MyCompanyId : MyCompanyId;
			obj.Auto.EquipmentId = EquipmentId;
			obj.Auto.AdditionalFuelTank = AdditionalFuelTank;
			obj.Auto.AutoChassis = AutoChassis;
			obj.Auto.AutoMotor = AutoMotor;
			obj.Auto.AutoPassportNumber = AutoPassportNumber;
			obj.Auto.DateOn = DateOn;
			obj.Auto.GosNumber = GosNumber;
			obj.Auto.GrossVehicleWeight = GrossVehicleWeight;
			obj.Auto.UnladenWeight = UnladenWeight;
			obj.Auto.InsuranceDateExpiration = InsuranceDateExpiration;
			obj.Auto.InsuranceDateStart = InsuranceDateStart;
			obj.Auto.InsuranceNumber = InsuranceNumber;
			obj.Auto.InterceptedVolumesLeft = InterceptedVolumesLeft;
			obj.Auto.InterceptedVolumesRight = InterceptedVolumesRight;
			obj.Auto.LicenseDateExpiration = LicenseDateExpiration;
			obj.Auto.LicenseDateStart = LicenseDateStart;
			obj.Auto.LicenseNumber = LicenseNumber;
			obj.Auto.Pallet = Pallet;
			obj.Auto.RateFuelConsumption = RateFuelConsumption;
			obj.Auto.SanitaryDateExpiration = SanitaryDateExpiration;
			obj.Auto.SanitaryDateStart = SanitaryDateStart;
			obj.Auto.SanitaryNumber = SanitaryNumber;
			obj.Auto.TonnageKindId = TonnageKindId;
			obj.Auto.TrailerUsedAutoId = TrailerUsedAutoId;
			obj.Auto.UsedFuelId = UsedFuelId;
			obj.Auto.VolumeVehicleEngine = VolumeVehicleEngine;
			return obj;
		}

		public static List<AutoModel> GetCollection(bool refresh = false)
		{
			//if (string.IsNullOrEmpty(rootHierarchyCode))
			//{
			//    return WADataProvider.WA.GetCollection<Product>().Where(s => !s.IsHiden).Select(ConvertToModel).ToList();
			//}

			Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("SYSTEM_PRODUCT_ASSETS_AUTO");
			List<Product> coll = h.GetTypeContents<Product>(true, refresh);

			if (refresh)
				WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);

			return coll.Select(ConvertToModel).ToList();
		}

		public static AutoModel ConvertToModel(Product value)
		{
			AutoModel obj = new AutoModel();
			if (value.IsAuto)
			{
				obj = new AutoModel
				{
					Id = value.Id,
					AdditionalFuelTank = value.Auto.AdditionalFuelTank,
					AutoChassis = value.Auto.AutoChassis,
					AutoMotor = value.Auto.AutoMotor,
					AutoPassportNumber = value.Auto.AutoPassportNumber,
					DateOn = value.Auto.DateOn,
					GosNumber = value.Auto.GosNumber,
					GrossVehicleWeight = value.Auto.GrossVehicleWeight,
					UnladenWeight = value.Auto.UnladenWeight,
					InsuranceDateExpiration = value.Auto.InsuranceDateExpiration,
					InsuranceDateStart = value.Auto.InsuranceDateStart,
					InsuranceNumber = value.Auto.InsuranceNumber,
					InterceptedVolumesLeft = value.Auto.InterceptedVolumesLeft,
					InterceptedVolumesRight = value.Auto.InterceptedVolumesRight,
					LicenseDateExpiration = value.Auto.LicenseDateExpiration,
					LicenseDateStart = value.Auto.LicenseDateStart,
					LicenseNumber = value.Auto.LicenseNumber,
					Pallet = value.Auto.Pallet,
					RateFuelConsumption = value.Auto.RateFuelConsumption,
					SanitaryDateExpiration = value.Auto.SanitaryDateExpiration,
					SanitaryDateStart = value.Auto.SanitaryDateStart,
					SanitaryNumber = value.Auto.SanitaryNumber,
					TonnageKindId = value.Auto.TonnageKindId,
					TrailerUsedAutoId = value.Auto.TrailerUsedAutoId,
					UsedFuelId = value.Auto.UsedFuelId,
					VolumeVehicleEngine = value.Auto.VolumeVehicleEngine,
					MyCompanyId = value.MyCompanyId,
					MyCompanyName = value.MyCompany.Name,
					Name = value.Name,
					EquipmentId = value.Auto.EquipmentId,
					EquipmentName = WADataProvider.WA.Cashe.GetCasheData<Equipment>().Item(value.Auto.EquipmentId).Name,
					EquipmentTypeName = WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(WADataProvider.WA.Cashe.GetCasheData<Equipment>().Item(value.Auto.EquipmentId).AutoTypeId).Name
				};
			}
			return obj;
		}

        public static AutoModel GetObject(int id)
        {
			Product e = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
			return AutoModel.ConvertToModel(e);
        }

		public int SaveTransaction(Product prod)
		{
			int autoid = -1;
			//Product prod = new Product();

			if (prod.Id == 0)
			{
				//prod.Id = auto.Id;
				prod.Name = prod.Auto.GosNumber;
				prod.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
				prod.KindId = Product.KINDID_AUTO;
			}
			/*else
			{
				//TODO: Сделать изменение в product.mycompanyid
				prod = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(auto.Id);
			}*/

			using (SqlConnection con = WADataProvider.WA.GetDatabaseConnection())
			{
				SqlTransaction tran = con.BeginTransaction();
				try
				{
					prod.Save(tran);
					prod.Auto.Save(tran);
					tran.Commit();
					Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("SYSTEM_PRODUCT_ASSETS_AUTO");
                    h.ContentAdd(prod, true);
				}
				catch (Exception)
				{
					tran.Rollback();
				}
				con.Close();
			}

			if (prod.Id > 0)
				autoid = prod.Id;

			return autoid;
		}

		public static void ToTrash(int id, string hierarchyCode)
		{
			Product obj = WADataProvider.WA.Cashe.GetCasheData<Product>().Item(id);
			obj.Remove();
			Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
			//WADataProvider.CacheProductsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
		}
	}
}