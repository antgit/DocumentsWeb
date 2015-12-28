using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Areas.Analitics.Models
{
    public enum AnaliticGroupKind
    {
        /// <summary>
        /// Опросник
        /// </summary>
        MktgQuestionnaire = 1,

        /// <summary>
        /// Тип продукции
        /// </summary>
        MktgProductType = 2,

        /// <summary>
        /// Тип холодильника
        /// </summary>
        MktgRefrigeratorType = 3,

        /// <summary>
        /// Принадлежность ХО
        /// </summary>
        MktgRefrigeratorAttachment = 4,

        /// <summary>
        /// В чьей собственности ХО
        /// </summary>
        MktgRefrigeratorOwn = 5
    }

    public class AnaliticGroupModel
    {
        public int Id { get; set; }
        public AnaliticGroupKind KindId { get; set; }
        public string Name { get; set; }

        public static List<AnaliticGroupModel> GetGroups() 
        {
            List<AnaliticGroupModel> list = new List<AnaliticGroupModel>();

            list.Add(new AnaliticGroupModel { Id = 1, KindId = AnaliticGroupKind.MktgQuestionnaire, Name = "Размещение холодильников" });
            list.Add(new AnaliticGroupModel { Id = 2, KindId = AnaliticGroupKind.MktgQuestionnaire, Name = "Отношения торговой точки с Геркулесом" });
            list.Add(new AnaliticGroupModel { Id = 3, KindId = AnaliticGroupKind.MktgQuestionnaire, Name = "Товарные группы по которым готова сотрудничать торговая точка" });
            list.Add(new AnaliticGroupModel { Id = 13, KindId = AnaliticGroupKind.MktgQuestionnaire, Name = "Общие состояние магазина" });
            list.Add(new AnaliticGroupModel { Id = 16, KindId = AnaliticGroupKind.MktgQuestionnaire, Name = "Товары со слишком высокой ценой" });

            list.Add(new AnaliticGroupModel { Id = 4, KindId = AnaliticGroupKind.MktgProductType, Name = "Мороженое" });
            list.Add(new AnaliticGroupModel { Id = 5, KindId = AnaliticGroupKind.MktgProductType, Name = "Замороженные фрукты/овощи" });
            list.Add(new AnaliticGroupModel { Id = 6, KindId = AnaliticGroupKind.MktgProductType, Name = "Полуфабрикаты фасованные" });
            list.Add(new AnaliticGroupModel { Id = 7, KindId = AnaliticGroupKind.MktgProductType, Name = "Полуфабрикаты весовые" });
            list.Add(new AnaliticGroupModel { Id = 8, KindId = AnaliticGroupKind.MktgProductType, Name = "Колбасная продукция" });
            list.Add(new AnaliticGroupModel { Id = 9, KindId = AnaliticGroupKind.MktgProductType, Name = "Молочная продукция" });
            list.Add(new AnaliticGroupModel { Id = 14, KindId = AnaliticGroupKind.MktgProductType, Name = "Майонез" });
            list.Add(new AnaliticGroupModel { Id = 15, KindId = AnaliticGroupKind.MktgProductType, Name = "Твердые сыры" });

            list.Add(new AnaliticGroupModel { Id = 10, KindId = AnaliticGroupKind.MktgRefrigeratorType, Name = "Горизонтальное холодильное оборудование" });
            list.Add(new AnaliticGroupModel { Id = 11, KindId = AnaliticGroupKind.MktgRefrigeratorType, Name = "Вертикальное холодильное оборудование" });
            list.Add(new AnaliticGroupModel { Id = 12, KindId = AnaliticGroupKind.MktgRefrigeratorType, Name = "Холодильные камеры и бонеты глубокой заморозки" });

            list.Add(new AnaliticGroupModel { Id = 17, KindId = AnaliticGroupKind.MktgRefrigeratorAttachment, Name = "Собственность ТРТ" });
            list.Add(new AnaliticGroupModel { Id = 18, KindId = AnaliticGroupKind.MktgRefrigeratorAttachment, Name = "Геркулес/Украинский продукт" });
            list.Add(new AnaliticGroupModel { Id = 19, KindId = AnaliticGroupKind.MktgRefrigeratorAttachment, Name = "Внешний бренд" });

            list.Add(new AnaliticGroupModel { Id = 20, KindId = AnaliticGroupKind.MktgRefrigeratorOwn, Name = "Брендированный" });
            list.Add(new AnaliticGroupModel { Id = 21, KindId = AnaliticGroupKind.MktgRefrigeratorOwn, Name = "В собственности ТРТ" });
            list.Add(new AnaliticGroupModel { Id = 22, KindId = AnaliticGroupKind.MktgRefrigeratorOwn, Name = "Чужой (внешняя ТМ)" });

            return list;
        }
    }
}