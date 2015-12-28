using System;
using BusinessObjects;

namespace DocumentsWeb.Areas.Kb.Models
{
    public static class KnowledgeData
    {
        public static string GetTooltip(string fieldName)
        {
            if (fieldName == GlobalPropertyNames.Name)
                return "Наименование статьи базы знаний является обязательным! Старайтесь задавать значение максимально кратким и понятным. Максимальная длина - 255 символов.";
            
            if (fieldName == GlobalPropertyNames.NameFull)
                return "Наименование статьи базы знаний используемое при печати в документах и отчетах. Если значение не указано используется \"Наименование\". Максимальная длина не ограничена.";
            if (fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание статьи базы знаний используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем. Максимальная длина не ограничена.";


            if (fieldName == GlobalPropertyNames.Code)
                return "Системный код статьи базы знаний используется как самой системой так и пользователем. Рекомендуется использовать уникальные значения. Максимальная длина 50 символов.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "Url навигации статьи базы знаний.";

            if (fieldName == "cmbMyCompanyId")
                return "Компания которой принадлежит статьи базы знаний.";

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
                    return "Введите наименование статьи базы знаний...";
                case GlobalPropertyNames.NameFull:
                    return "Введите печатное наименование статьи базы знаний...";
                case GlobalPropertyNames.Memo:
                    return "Введите примечание или описание статьи базы знаний...";
                case GlobalPropertyNames.Code:
                    return "Введите код статьи базы знаний...";
                case GlobalPropertyNames.CodeFind:
                    return "Введите url статьи базы знаний...";
            }
            return string.Empty;
        }
    }
}