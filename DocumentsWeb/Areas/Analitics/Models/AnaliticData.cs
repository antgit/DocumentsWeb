using System;
using BusinessObjects;

namespace DocumentsWeb.Areas.Analitics.Models
{
    public static class AnaliticData
    {
        public static string GetTooltip(string controller, string fieldName)
        {
            if (controller == "AgentCategory" && fieldName == GlobalPropertyNames.Name)
                return "������������ ��������� �������� ����� (������ �����) �������� ������������! ���������� �������� �������� ����������� ������� � ��������. ������������ ����� - 255 ��������.";
            if (controller == "AgentCategory" && fieldName == GlobalPropertyNames.NameFull)
                return "������������ ��������� �������� ����� (������ �����) ������������ ��� ������ � ���������� � �������. ���� �������� �� ������� ������������ \"������������\". ������������ ����� �� ����������.";
            if (fieldName == GlobalPropertyNames.NameFull)
                return "������������ ��������� ������������ ��� ������ � ���������� � �������. ���� �������� �� ������� ������������ \"������������\". ������������ ����� �� ����������.";
            if (controller == "AgentCategory" && fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� ��������� �������� ����� (������ �����) ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������. ������������ ����� �� ����������.";
            if (fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� ��������� ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������. ������������ ����� �� ����������.";


            if (fieldName == GlobalPropertyNames.Code)
                return "��������� ��� ��������� ������������ ��� ����� �������� ��� � �������������. ������������� ������������ ���������� ��������. ������������ ����� 50 ��������.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "�������������� ��� ������ ���������: ��� ������� ����� � �������������� ���� - ������ ������������ ������������� ������ ��������. �� ��������� ��� ������ ����������� ����� �������������� ������������ � �������� \"���\".";

            if (fieldName == GlobalPropertyNames.DateModified)
                return "���� ���������� ��������� ������";
            if (fieldName == GlobalPropertyNames.Id)
                return "���������� �������� �������������";
            if (fieldName == GlobalPropertyNames.UserName)
                return "��� ������������ ���������� ��� ����������� ������ � ��������� ���";
            if (fieldName == GlobalPropertyNames.MyCompanyId)
                return "������������� ��������-��������� ������";
            if (fieldName == GlobalPropertyNames.MyCompanyName)
                return "������������ ��������-��������� ������";
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
                            return "������� ������������ ������ �����...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������ ������ �����...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� �������� ������ �����...";
                        case GlobalPropertyNames.Code:
                            return "������� ��� ������ �����...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������ ������ �����...";
                    }
                    break;
                case "AgentTypeOutlet":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������ ���� �������� �����...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������ ���� �������� �����...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� �������� ���� �������� �����...";
                        case GlobalPropertyNames.Code:
                            return "������� ��� ���� �������� �����...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������ ���� �������� �����...";
                    }
                    break;
                case "AnaliticCfo":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������ ������ ���������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������ ������ ���������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� �������� ������ ���������������...";
                        case GlobalPropertyNames.Code:
                            return "������� ��� ������ ���������������...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������ ������ ���������������...";
                    }
                    break;
                case "AnaliticContractKind":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������ ���� �������� ��� ���� ���������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������ ���� �������� ��� ���� ���������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� �������� ���� �������� ��� ���� ���������...";
                        case GlobalPropertyNames.Code:
                            return "������� ��� ���� �������� ��� ���� ���������...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������ ���� �������� ��� ���� ���������...";
                    }
                    break;

                case "AnaliticContractPriority":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "AnaliticContractState":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "AnaliticPaymentType":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "AnaliticPlanKind":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������ ...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "AnaliticPlanResult":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "Brand":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "DeliveryCondition":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "EquipmentPlacement":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "MetricArea":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "OutletLocation":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "OwnerShip":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "PaymentType":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "ProductType":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "ReasonTradepoint":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "ReturnReason":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "TradeGroup":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
                case "Workpost":
                    switch (fieldName)
                    {
                        case GlobalPropertyNames.Name:
                            return "������� ������������...";
                        case GlobalPropertyNames.NameFull:
                            return "������� �������� ������������...";
                        case GlobalPropertyNames.Memo:
                            return "������� ���������� ��� ��������...";
                        case GlobalPropertyNames.Code:
                            return "������� ���...";
                        case GlobalPropertyNames.CodeFind:
                            return "������� ��� ������...";
                    }
                    break;
            }

            throw new Exception("����������� NullText");
        }
    }
}