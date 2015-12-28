using System;
using System.Collections.Generic;

namespace DocumentsWeb.Documents
{
    public class DocumentTire
    {
        public DocumentTire()
        {
            TireRuns = new List<TireRun>();
        }
        public class TireRun
        {
            public TireRun()
            {
                
            }
            /// <summary>
            /// Тип, модель транспортного средства
            /// </summary>
            public string Model { get; set; }
            /// <summary>Пробег</summary>
            public int Run { get; set; }
            /// <summary>Дата установки</summary>
            public DateTime? DateOn { get; set; }
            /// <summary>Дата снятия</summary>
            public DateTime? DateOff { get; set; }
            /// <summary>Пробег тис. км. за месяц</summary>
            public DateTime? OdometrMonth { get; set; }
            /// <summary>Пробег тис. км. всего</summary>
            public DateTime? OdometrAll { get; set; }
            /// <summary>Техническое состояние</summary>
            public string State { get; set; }
            /// <summary>Текущая высота протектора</summary>
            public string Height { get; set; }
            /// <summary>Причина снятия шины</summary>
            public string StateOff { get; set; }
            /// <summary>ФИО водителя</summary>
            public string SignName { get; set; }
            /// <summary>Дата подписи водителя</summary>
            public string SignDate { get; set; }
        }

        public List<TireRun> TireRuns { get; set; }
        /// <summary>
        /// Условное обозначение размера шины
        /// </summary>
        public string Size { get; set; }
        /// <summary>
        /// Модель или рисунок протектора
        /// </summary>
        public string Model { get; set; }
        /// <summary>
        /// Номер документа по которому изготовлена шина
        /// </summary>
        public string DocNumber { get; set; }
        /// <summary>Ответственный</summary>
        public string ManagerName { get; set; }
        /// <summary>Дата подписи</summary>
        public string SignDate { get; set; }
        /// <summary>Телефон</summary>
        public string Phone { get; set; }
        /// <summary>Факс</summary>
        public string Fax { get; set; }
        /// <summary>Email</summary>
        public string Email { get; set; }
        /// <summary>Дата изготовления</summary>
        public DateTime TireDate { get; set; }
        /// <summary>Индекс нагрузки</summary>
        public string IndexLoad { get; set; }
        /// <summary>Индекс скорости</summary>
        public string IndexSpeed { get; set; }
        /// <summary>Инвентарный номер</summary>
        public string InventoryNumber { get; set; }
        /// <summary>Номер изготовителя</summary>
        public string DeveloperNumber { get; set; }
        /// <summary>Код нормы</summary>
        public string ResourceNumber { get; set; }
        /// <summary>Код нормы в тис км.</summary>
        public int ResourceValue { get; set; }


    }

    public class DocumentBattery
    {
        public class BatteryRun
        {
            /// <summary>
            /// Дата изготовления
            /// </summary>
            public DateTime Date { get; set; }
            /// <summary>
            /// Дата установки/снятия
            /// </summary>
            public DateTime? DateOff { get; set; }
            /// <summary>Модель</summary>
            public string Model { get; set; }
            /// <summary>Номер танспортного средства</summary>
            public string Number { get; set; }
            /// <summary>Наработка тис.км.</summary>
            public int Value { get; set; }
            
        }
        public class BatteryYear
        {
            /// <summary>
            /// Год
            /// </summary>
            public int Year { get; set; }
            /// <summary>Наработка</summary>
            public int Value { get; set; }
            /// <summary>За 1 квартал</summary>
            public int Q1Value { get; set; }
            /// <summary>За 2 квартал</summary>
            public int Q2Value { get; set; }
            /// <summary>За 3 квартал</summary>
            public int Q3Value { get; set; }
            /// <summary>За 4 квартал</summary>
            public int Q4Value { get; set; }
            /// <summary>Наработка за год</summary>
            public int ValueTotal { get; set; }
            /// <summary>Наработка всего</summary>
            public int ValueAll { get; set; }
        }
        public string TypeName { get; set; }
        public string KindName { get; set; }
        public string DevelopName { get; set; }
        public string SerialNumber { get; set; }
        public string InventoryNumber { get; set; }
        public string InDocNumber { get; set; }
        public string InDocDate { get; set; }
        public string InDocKind { get; set; }
        public string WarrantyKind { get; set; }
        public string WarrantyValue { get; set; }
        public string NormKind { get; set; }
        public string NormValue { get; set; }

        /// <summary>Приспособления облегчения пуска двигателя</summary>
        public bool MotorStarting { get; set; }
        /// <summary>Электроусилитель руля</summary>
        public bool PowerSteering { get; set; }
        /// <summary>Противотуманные фары</summary>
        public bool FogLights { get; set; }
        /// <summary>Рация</summary>
        public bool Radio { get; set; }

        /// <summary>Реанимационное обрудование</summary>
        public bool ResuscitationEquipment { get; set; }
        /// <summary>Бортовой компьютер</summary>
        public bool Computer { get; set; }
        /// <summary>Электроподогрев зеркал</summary>
        public bool HeatedMirrors { get; set; }
        /// <summary>Тахограф электромеханический</summary>
        public bool Tachograph { get; set; }
        /// <summary>Тахограф цифровой</summary>
        public bool TachographDigital { get; set; }
        /// <summary>Таксометр</summary>
        public bool Taximeter { get; set; }
        /// <summary>Подогрев стекол</summary>
        public bool HeatedGlass { get; set; }
        /// <summary>Кондиционер</summary>
        public bool AirConditioning { get; set; }
        /// <summary>Аудио-видео техника</summary>
        public bool Audio { get; set; }
        /// <summary>Охранная сигнализация</summary>
        public bool SecurityAlarm { get; set; }

        /// <summary>Нейтрализатор опасных выхлопов</summary>
        public bool ConverterDangerousEmissions { get; set; }

        /// <summary>Дата изготовления</summary>
        public DateTime DateCreate { get; set; }
        /// <summary>Дата приобретения</summary>
        public DateTime DatePurchase { get; set; }
        /// <summary>Дата ввода в эксплуатацию</summary>
        public DateTime DateIn { get; set; }
        /// <summary>Дата вывода из эксплуатации</summary>
        public DateTime DateOut { get; set; }
        /// <summary>Дата списания</summary>
        public DateTime DateOff { get; set; }


    }
}