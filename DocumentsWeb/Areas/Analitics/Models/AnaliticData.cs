using System;
using BusinessObjects;

namespace DocumentsWeb.Areas.Analitics.Models
{
    public static class AnaliticData
    {
        public static string GetTooltip(string controller, string fieldName)
        {
            if (controller == "AgentCategory" && fieldName == GlobalPropertyNames.Name)
                return "Наименование категории торговой точки (канала сбыта) является обязательным! Старайтесь задавать значение максимально кратким и понятным. Максимальная длина - 255 символов.";
            if (controller == "AgentCategory" && fieldName == GlobalPropertyNames.NameFull)
                return "Наименование категории торговой точки (канала сбыта) используемое при печати в документах и отчетах. Если значение не указано используется \"Наименование\". Максимальная длина не ограничена.";
            if (fieldName == GlobalPropertyNames.NameFull)
                return "Наименование аналитики используемое при печати в документах и отчетах. Если значение не указано используется \"Наименование\". Максимальная длина не ограничена.";
            if (controller == "AgentCategory" && fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание категории торговой точки (канала сбыта) используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем. Максимальная длина не ограничена.";
            if (fieldName == GlobalPropertyNames.Memo)
                return "Примечание или описание аналитики используется для детального описания, призвано помочь Вам в правильном выборе данных в дальнейшем. Максимальная длина не ограничена.";


            if (fieldName == GlobalPropertyNames.Code)
                return "Системный код аналитики используется как самой системой так и пользователем. Рекомендуется использовать уникальные значения. Максимальная длина 50 символов.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "Дополнительный код поиска аналитики: при быстром вводе в соотвествующие поля - помимо наименования анализируется данное свойство. По умолчанию код поиска формируется путем транслитерации наименования и атрибута \"Код\".";

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

        public static string GetNullText(string controller, string fieldName)
        {

            switch (controller)
            {
                case "AgentCategory":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование канала сбыта...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование канала сбыта...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание канала сбыта...";
                        case GlobalPropertyNames.Code:
                            return "Введите код канала сбыта...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска канала сбыта...";
                    }
                    break;
                case "AgentTypeOutlet":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование типа торговой точки...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование типа торговой точки...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание типа торговой точки...";
                        case GlobalPropertyNames.Code:
                            return "Введите код типа торговой точки...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска типа торговой точки...";
                    }
                    break;
                case "AnaliticCfo":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование центра ответственности...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование центра ответственности...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание центра ответственности...";
                        case GlobalPropertyNames.Code:
                            return "Введите код центра ответственности...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска центра ответственности...";
                    }
                    break;
                case "AnaliticContractKind":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование типа договора или типа документа...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование типа договора или типа документа...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание типа договора или типа документа...";
                        case GlobalPropertyNames.Code:
                            return "Введите код типа договора или типа документа...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска типа договора или типа документа...";
                    }
                    break;

                case "AnaliticContractPriority":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "AnaliticContractState":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "AnaliticPaymentType":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "AnaliticPlanKind":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование ...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "AnaliticPlanResult":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "Brand":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "DeliveryCondition":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "EquipmentPlacement":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "MetricArea":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "OutletLocation":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "OwnerShip":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "PaymentType":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "ProductType":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "ReasonTradepoint":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "ReturnReason":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "TradeGroup":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
                case "Workpost":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "Введите наименование...";
                        case GlobalPropertyNames.NameFull:
                            return "Введите печатное наименование...";
                        case GlobalPropertyNames.Memo:
                            return "Введите примечание или описание...";
                        case GlobalPropertyNames.Code:
                            return "Введите код...";
                        case GlobalPropertyNames.CodeFind:
                            return "Введите код поиска...";
                    }
                    break;
            }

            throw new Exception("Неизвестный NullText");
        }
    }
}