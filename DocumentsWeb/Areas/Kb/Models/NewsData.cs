using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Kb.Models
{
    public static class NewsData
    {
        public static string GetTooltip(string fieldName)
        {
            if (fieldName == GlobalPropertyNames.Name)
                return "Заголовок новости является обязательным! Старайтесь задавать значение максимально кратким и понятным. Максимальная длина - 255 символов.";
            if (fieldName == GlobalPropertyNames.NameFull)
                return "Краткое представление новости используемое при печати в документах и отчетах. Если значение не указано используется \"Наименование\". Максимальная длина не ограничена.";
            if (fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание новости используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем. Максимальная длина не ограничена.";
            if (fieldName == GlobalPropertyNames.SendDate)
                return "Дата публикации новости. Если дата указана новость считается опубликованной и неопубликованной в противном случае.";
            
            

            if (fieldName == GlobalPropertyNames.Code)
                return "Системный код новости используется как самой системой так и пользователем. Рекомендуется использовать уникальные значения. Максимальная длина 50 символов.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "Дополнительный код поиска новости: при быстром вводе в соотвествующие поля - помимо наименования анализируется данное свойство. По умолчанию код поиска формируется путем транслитерации наименования и атрибута \"Код\".";

            if (fieldName == GlobalPropertyNames.DateModified)
                return "Дата последнего изменения данных";
            if (fieldName == GlobalPropertyNames.Id)
                return "Уникальный числовой идентификатор";
            if (fieldName == GlobalPropertyNames.UserName)
                return "Имя пользователя создавшего или изменившего данные в последний раз";
            if (fieldName == GlobalPropertyNames.MyCompanyId)
                return "Идентификатор компании-владельца данных";
            if (fieldName == GlobalPropertyNames.MyCompanyName)
                return "Наименование компании-владельца данных";
            return string.Empty;
        }

        public static string GetNullText(string fieldName)
        {
            switch (fieldName)
            {
                case GlobalPropertyNames.Name:
                    return "Введите заголовок новости...";
                case GlobalPropertyNames.NameFull:
                    return "Введите краткое наименование новости...";
                case GlobalPropertyNames.Memo:
                    return "Введите полное описание...";
                case GlobalPropertyNames.Code:
                    return "Введите код новости...";
                case GlobalPropertyNames.CodeFind:
                    return "Введите код поиска новости...";
            }
            return string.Empty;
        }

        public static List<NewsModel> GetLastFiveNews()
        {
            return Message.MessageNewsLastFive(WADataProvider.CurrentUser).Select(NewsModel.ConvertToModel).OrderByDescending(o => o.SendDate).ToList();
        }

        public static List<NewsModel> GetSharedLastFiveNews()
        {
            return Message.MessageNewsLastFive(WADataProvider.SystemInternalUser()).Select(NewsModel.ConvertToModel).OrderByDescending(o => o.SendDate).ToList();
        }

        
    }
}