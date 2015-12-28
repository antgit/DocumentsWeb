using BusinessObjects;

namespace DocumentsWeb.Areas.Agents.Models
{
    public static class AgentData
    {
        public static string GetTooltip(string controller, string fieldName)
        {
            if (controller == "MyDepatment" && fieldName == GlobalPropertyNames.Name)
                return "Наименование корреспондента \"Наше предприятие\" является обязательным! Старайтесь задавать значение максимально кратким и понятным. Максимальная длина - 255 символов.";

            if (controller == "Store" && fieldName == GlobalPropertyNames.Name)
                return "Наименование корреспондента \"Склад\" является обязательным! Задавайте значение максимально кратким и понятным, рекомендуется не создавать несколько складов с одинаковым наименованием. Максимальная длина - 255 символов.";
            
            if (controller == "MyDepatment" && fieldName == GlobalPropertyNames.NameFull)
                return "Значение используемое при печати в документах и отчетах. Если значение не указано используется \"Наименование\". Максимальная длина не ограничена.";
            
            if (controller == "MyDepatment" && fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание корреспондента \"Наше предприятие\" используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем, например Вы можете указать девиз вашей компании! Максимальная длина не ограничена.";
            
            if (controller == "Store" && fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание склада используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем, например Вы можете указать девиз сотрудников склада! Максимальная длина не ограничена.";

            if (controller == "Worker" && fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание физического лица используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем, например Вы можете указать жизненное кредо человека! Максимальная длина не ограничена.";

            if (fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание корреспондента используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем, например Вы можете указать девиз вашей компании! Максимальная длина не ограничена.";

            if (controller == "Worker" && fieldName == GlobalPropertyNames.AddressLegal)
                return "Адрес прописки на основе паспортных данных.";
            if (controller == "Worker" && fieldName == GlobalPropertyNames.AddressPhysical)
                return "Фактический адрес проживания на текущий момент времени.";

            if (fieldName == GlobalPropertyNames.AddressLegal)
                return "Юридический адрес предприятия на основе данных о регистрации предприятия.";
            if (fieldName == GlobalPropertyNames.AddressPhysical)
                return "Фактический адрес предприятия на текущий момент времени.";

            if (fieldName == GlobalPropertyNames.AddressLegal)
                return "Юридический адрес предприятия на основе данных о регистрации предприятия.";
            if (fieldName == GlobalPropertyNames.AddressPhysical)
                return "Фактический адрес предприятия на текущий момент времени.";

            if (fieldName == GlobalPropertyNames.NdsPayer)
                return "Является ли предприятие плательщиком НДС";

            if (controller == "Store" && fieldName == GlobalPropertyNames.Phone)
                return "Основной телефон склада";
            if (controller == "Worker" && fieldName == GlobalPropertyNames.Phone)
                return "Основной телефон сотрудника";
            if (fieldName == GlobalPropertyNames.Phone)
                return "Основной телефон предприятия";

            if (fieldName == GlobalPropertyNames.Code)
                return "Системный код корреспондента используется как самой системой так и пользователем. Рекомендуется использовать уникальные значения. Максимальная длина 50 символов.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "Дополнительный код поиска корреспондента: при быстром вводе в соотвествующие поля - помимо наименования анализируется данное свойство. По умолчанию код поиска формируется путем транслитерации наименования и атрибута \"Код\".";


            if (fieldName == GlobalPropertyNames.OwnershipId)
                return "Форма собственности корреспондента. Выбирается из соответствующего справочника аналитики \"Форма собственности\".";

            if (fieldName == GlobalPropertyNames.InternationalName)
                return "Международное наименование корреспондента, в большинстве случаев содержит наименование на английском языке.";

            if (fieldName == GlobalPropertyNames.DateModified)
                return "Дата последнего изменения данных";
            if (fieldName == GlobalPropertyNames.Id)
                return "Уникальный числовой идентификатор";
            if (fieldName == GlobalPropertyNames.UserName)
                return "Имя пользователя создавшего или изменившего данные в последний раз";

            if (controller == "Store" && fieldName == "cmbMyCompanyId")
                return "Компания которой принадлежит склад. Изменить компанию после создания склада невозможно!";            
            if (fieldName == GlobalPropertyNames.MyCompanyId)

                return "Идентификатор компании-владельца данных";
            if (fieldName == GlobalPropertyNames.MyCompanyName)
                return "Наименование компании-владельца данных";
            return string.Empty;
        }
    }
}