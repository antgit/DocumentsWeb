using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using BusinessObjects;
using DocumentsWeb.Models;
using DevExpress.Web.ASPxEditors;

namespace DocumentsWeb.Areas.General.Models
{
    /// <summary>
    /// Модель периода работы или перерыва
    /// </summary>
    public class TimePeriodModel : IModelData
    {
        public TimePeriodModel()
        {
            ModelId = Guid.NewGuid().ToString();
            ModelUserOwnerId = WADataProvider.CurrentUser.Id;
        }
        public string ModelId { get; set; }
        public int ModelUserOwnerId { get; set; }
        /// <summary>Идентификатор</summary>
        public int Id { get; set; }
        /// <summary>Имя пользователя</summary>
        public string UserName { get; set; }
        /// <summary>Наименование</summary>
        [Required(ErrorMessage = "Наименование обязательно")]
        [StringLength(255, ErrorMessage = "Не более 255 символов")]
        public string Name { get; set; }
        /// <summary>Имя состояния</summary>
        public string StateName { get; set; }
        /// <summary>Идентификатор состояния</summary>
        public int StateId { get; set; }
        /// <summary>Код</summary>
        public string Code { get; set; }
        /// <summary>Примечание</summary>
        public string Memo { get; set; }
        /// <summary>Тип</summary>
        public int KindId { get; set; }
        /// <summary>Наименование типа</summary>
        public string KindName { get; set; }

        public bool MondayW { get; set; }
        public DateTime MondayS { get; set; }
        public DateTime MondayE { get; set; }

        public bool TuesdayW { get; set; }
        public DateTime TuesdayS { get; set; }
        public DateTime TuesdayE { get; set; }

        public bool WednesdayW { get; set; }
        public DateTime WednesdayS { get; set; }
        public DateTime WednesdayE { get; set; }

        public bool ThursdayW { get; set; }
        public DateTime ThursdayS { get; set; }
        public DateTime ThursdayE { get; set; }

        public bool FridayW { get; set; }
        public DateTime FridayS { get; set; }
        public DateTime FridayE { get; set; }

        public bool SaturdayW { get; set; }
        public DateTime SaturdayS { get; set; }
        public DateTime SaturdayE { get; set; }

        public bool SundayW { get; set; }
        public DateTime SundayS { get; set; }
        public DateTime SundayE { get; set; }

        public bool canChangeType { get; set; }
        public bool IsReadOnly { get; set; }
        public bool IsSystem { get; set; }

        public int MyCompanyId { get; set; }
        /// <summary>Компания</summary>
        public string MyCompanyName { get; set; }
        /// <summary>Дата изменения</summary>
        public DateTime? DateModified { get; set; }

        public bool ReloadOnEdit { get; set; }

        public string InHierarchies { get; set; }

        public TimePeriod ToObject(Workarea workarea)
        {
            TimePeriod obj = new TimePeriod { Workarea = workarea };
            obj.Load(Id);
            if (obj.Id == 0)
            {
                obj.IsNew = true;
            }

            obj.Name = Name;
            obj.Code = Code;
            obj.Memo = Memo;
            obj.KindId = (KindId == 0 ? TimePeriod.KINDID_WORK : KindId);
            obj.StateId = StateId == 0 ? State.STATEACTIVE : StateId;

            obj.MondayW = MondayW ? 1 : 0;
            obj.MondaySH = MondayS.Hour;
            obj.MondaySM = MondayS.Minute;
            obj.MondayEH = MondayE.Hour;
            obj.MondayEM = MondayE.Minute;

            obj.TuesdayW = TuesdayW ? 1 : 0;
            obj.TuesdaySH = TuesdayS.Hour;
            obj.TuesdaySM = TuesdayS.Minute;
            obj.TuesdayEH = TuesdayE.Hour;
            obj.TuesdayEM = TuesdayE.Minute;

            obj.WednesdayW = WednesdayW ? 1 : 0;
            obj.WednesdaySH = WednesdayS.Hour;
            obj.WednesdaySM = WednesdayS.Minute;
            obj.WednesdayEH = WednesdayE.Hour;
            obj.WednesdayEM = WednesdayE.Minute;

            obj.ThursdayW = ThursdayW ? 1 : 0;
            obj.ThursdaySH = ThursdayS.Hour;
            obj.ThursdaySM = ThursdayS.Minute;
            obj.ThursdayEH = ThursdayE.Hour;
            obj.ThursdayEM = ThursdayE.Minute;

            obj.FridayW = FridayW ? 1 : 0;
            obj.FridaySH = FridayS.Hour;
            obj.FridaySM = FridayS.Minute;
            obj.FridayEH = FridayE.Hour;
            obj.FridayEM = FridayE.Minute;

            obj.SaturdayW = SaturdayW ? 1 : 0;
            obj.SaturdaySH = SaturdayS.Hour;
            obj.SaturdaySM = SaturdayS.Minute;
            obj.SaturdayEH = SaturdayE.Hour;
            obj.SaturdayEM = SaturdayE.Minute;

            obj.SundayW = SundayW ? 1 : 0;
            obj.SundaySH = SundayS.Hour;
            obj.SundaySM = SundayS.Minute;
            obj.SundayEH = SundayE.Hour;
            obj.SundayEM = SundayE.Minute;
            return obj;
        }

        public static void ToTrash(int id)
        {
            TimePeriod obj = WADataProvider.WA.Cashe.GetCasheData<TimePeriod>().Item(id);
            obj.Remove();
            //Hierarchy hRoot = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //DocumentsWeb.Models.WADataProvider.CacheUnitModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void CreateCopy(int id)
        {
            if (id != 0)
            {
                TimePeriod obj = WADataProvider.WA.Cashe.GetCasheData<TimePeriod>().Item(id);
                TimePeriod newObj = TimePeriod.CreateCopy(obj);
                newObj.Name += " (копия)";
                newObj.Save();
                Hierarchy hCurrent = obj.FirstHierarchy();
                if (hCurrent != null)
                {
                    hCurrent.ContentAdd(newObj, true);
                }
                //Hierarchy hRoot = DocumentsWeb.Models.WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
                //DocumentsWeb.Models.WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(newObj));
            }
        }

        public static void SetStateNotDone(int id)
        {
            TimePeriod obj = WADataProvider.WA.Cashe.GetCasheData<TimePeriod>().Item(id);
            obj.StateId = State.STATENOTDONE;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStatetDone(int id)
        {
            TimePeriod obj = WADataProvider.WA.Cashe.GetCasheData<TimePeriod>().Item(id);
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
                        obj = WADataProvider.WA.GetObject<TimePeriod>(id);

                        obj.StateId = State.STATEACTIVE;
                        obj.Save();
                    }
                }
            }
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static void SetStateDeny(int id)
        {
            TimePeriod obj = WADataProvider.WA.Cashe.GetCasheData<TimePeriod>().Item(id);
            obj.StateId = State.STATEDENY;
            obj.Save();
            //Hierarchy hRoot = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(hierarchyCode);
            //WADataProvider.CacheAnaliticsModelData.AddToCashe(hRoot.Id, ConvertToModel(obj));
        }

        public static TimePeriodModel GetObject(int id)
        {
            TimePeriod obj = WADataProvider.WA.GetObject<TimePeriod>(id);
            return ConvertToModel(obj);
        }

        public static TimePeriodModel ConvertToModel(TimePeriod value)
        {
            DateTime now = DateTime.Now;
            TimePeriodModel obj = new TimePeriodModel
            {
                Id = value.Id,
                UserName = value.UserName,
                Name = value.Name,
                Code = value.Code,
                Memo = value.Memo,
                KindId = value.KindId,
                KindName = value.KindName,
                StateId = value.StateId,
                StateName = value.State.Name,
                DateModified = value.DateModified,

                MondayW = value.MondayW > 0,
                MondayS = new DateTime(now.Year, now.Month, now.Day, value.MondaySH, value.MondaySM, 0),
                MondayE = new DateTime(now.Year, now.Month, now.Day, value.MondayEH, value.MondayEM, 0),

                TuesdayW = value.TuesdayW > 0,
                TuesdayS = new DateTime(now.Year, now.Month, now.Day, value.TuesdaySH, value.TuesdaySM, 0),
                TuesdayE = new DateTime(now.Year, now.Month, now.Day, value.TuesdayEH, value.TuesdayEM, 0),

                WednesdayW = value.WednesdayW > 0,
                WednesdayS = new DateTime(now.Year, now.Month, now.Day, value.WednesdaySH, value.WednesdaySM, 0),
                WednesdayE = new DateTime(now.Year, now.Month, now.Day, value.WednesdayEH, value.WednesdayEM, 0),

                ThursdayW = value.ThursdayW > 0,
                ThursdayS = new DateTime(now.Year, now.Month, now.Day, value.ThursdaySH, value.ThursdaySM, 0),
                ThursdayE = new DateTime(now.Year, now.Month, now.Day, value.ThursdayEH, value.ThursdayEM, 0),

                FridayW = value.FridayW > 0,
                FridayS = new DateTime(now.Year, now.Month, now.Day, value.FridaySH, value.FridaySM, 0),
                FridayE = new DateTime(now.Year, now.Month, now.Day, value.FridayEH, value.FridayEM, 0),

                SaturdayW = value.SaturdayW > 0,
                SaturdayS = new DateTime(now.Year, now.Month, now.Day, value.SaturdaySH, value.SaturdaySM, 0),
                SaturdayE = new DateTime(now.Year, now.Month, now.Day, value.SaturdayEH, value.SaturdayEM, 0),

                SundayW = value.SundayW > 0,
                SundayS = new DateTime(now.Year, now.Month, now.Day, value.SundaySH, value.SundaySM, 0),
                SundayE = new DateTime(now.Year, now.Month, now.Day, value.SundayEH, value.SundayEM, 0),

                MyCompanyId = -1,
                IsReadOnly = value.IsReadOnly,
                IsSystem = value.IsSystem
            };
            
            if (value.MyCompanyId != 0)
            {
                obj.MyCompanyName = value.MyCompany.Name;
            }
            return obj;
        }

        public static IEnumerable GetCollection()
        {
            List<TimePeriod> coll = WADataProvider.WA.GetCollection<TimePeriod>();
            return coll.Select(ConvertToModel).OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Список графиков работы
        /// </summary>
        /// <returns></returns>
        public static List<TimePeriodModel> GetTimePeriod()
        {
            List<TimePeriod> coll = WADataProvider.WA.GetCollection<TimePeriod>().Where(w => w.KindId == TimePeriod.KINDID_WORK && w.StateId != State.STATEDELETED).ToList<TimePeriod>();
            return coll.Select(ConvertToModel).ToList();
        }
        /// <summary>
        /// Список работы (разрешенного времени входа) пользователя
        /// </summary>
        /// <returns></returns>
        public static List<TimePeriodModel> GetAccountTimePeriod(bool refresh=false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.SYSTEM_TIMEPERIOD_USERLOGON);
            List<TimePeriod> coll = h.GetTypeContents<TimePeriod>(refresh: refresh).Where(w => w.KindId == TimePeriod.KINDID_WORK && w.StateId != State.STATEDELETED && WADataProvider.IsCompanyIdAllowIdToCurrentUser(w.MyCompanyId)).ToList();
            return coll.Select(ConvertToModel).ToList();
        }
        /// <summary>
        /// Список графиков перерывов
        /// </summary>
        /// <returns></returns>
        public static List<TimePeriodModel> GetBreakPeriod()
        {
            List<TimePeriod> coll = WADataProvider.WA.GetCollection<TimePeriod>().Where(w => w.KindId == TimePeriod.KINDID_BREAK && w.StateId != State.STATEDELETED).ToList<TimePeriod>();
            return coll.Select(ConvertToModel).ToList();
        }

        public static bool CanSave(int id)
        {
            TimePeriod tp = new TimePeriod { Workarea = WADataProvider.WA };
            tp.Load(id);
            return !(tp.IsSystem | tp.IsReadOnly);
        }

        public static object GetTimePeriodRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            string name = args.Filter;
            List<TimePeriodModel> list = TimePeriodModel.GetTimePeriod();
            return list.Where(w => w.Name.ToLower().Contains(name.ToLower()) && w.StateId == State.STATEACTIVE);
        }

        public static object GetBreakPeriodRange(ListEditItemsRequestedByFilterConditionEventArgs args)
        {
            string name = args.Filter;
            List<TimePeriodModel> list = TimePeriodModel.GetBreakPeriod();
            return list.Where(w => w.Name.ToLower().Contains(name.ToLower()) && w.StateId == State.STATEACTIVE);
        }

        public static object GetTimePeriodByID(ListEditItemRequestedByValueEventArgs args)
        {
            if (args.Value != null)
            {
                int id = (int)args.Value;
                return GetObject(id);
            }
            return null;
        }
    }
}