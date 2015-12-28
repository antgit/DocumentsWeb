using System.Collections.Generic;
using System.Web.Security;
using BusinessObjects;

namespace DocumentsWeb.Areas.Admins.Models
{
    public static class UserData
    {
        public static string GetTooltip(string controller, string fieldName)
        {
            if (fieldName == "Config.Out.AllowNumberEdit")
                return "���� �������������� ������ ��������� ����� ��������� ����� ������������� �� �������� ������������� ����������";
            if (fieldName == "Config.Out.AllowAgenToEdit")
                return "���� �������������� ��������� � ������ �������� ��������� �������� ������ �� �������������� �����������";
            if (fieldName == "Config.Out.AllowProductEdit")
                return "���� �������������� ��������� � ������ �������� ��������� �������� ������ �� �������������� ������� �����";
            if (fieldName == "Config.Out.AllowAgenToSearch")
                return "���� ����� �������� � ������ ������ ����������� ����������� ����������� ������ ��������� ������ ������ ���������������";
            if (fieldName == "Config.Out.AllowProductSearch")
                return "���� ����� �������� � ������ ������ ������� ����� ����������� ����������� ������ ��������� ������ ������ �������� �����";
            if (fieldName == "Config.Out.AllowAgenToCreate")
                return "���� ��������� �������� ������������ � ������ �������� ��������� �������� ������ �� �������� ������ �����������";
            if (fieldName == "Config.Out.AllowProductCreate")
                return "���� ��������� �������� ������� � ������ �������� ��������� �������� ������ �� �������� ������ ������";

            if (fieldName == "Config.Out.DecimalPlacesQty")
                return "��������� ���������� ���������� ������ ��� �������������� ���������� � ������ ����������� ���������";
            if (fieldName == "Config.Out.DecimalPlacesPrice")
                return "��������� ���������� ���������� ������ ��� �������������� ���� � ������ ����������� ���������";
            if (fieldName == "Config.Out.DecimalPlacesSumma")
                return "��������� ���������� ���������� ������ ��� �������������� ����� � ������ ����������� ���������";

            if (fieldName == "Config.Out.DistpalyFormatQty")
                return "��������� ������� ����������� ������ � ���������� � ��������� ����� ���������";
            if (fieldName == "Config.Out.DisplayFormatPrice")
                return "��������� ������� ����������� ������ � ���� � ��������� ����� ���������";
            if (fieldName == "Config.Out.DisplayFormatSumma")
                return "��������� ������� ����������� ������ � ����� � ��������� ����� ���������";

            if (fieldName == GlobalPropertyNames.Name)
                return "����� ������������ �������� ������������! ���������� �������� �������� ����������� ������� � ��������, ����� �������� �������� ����� ����������!. ����������� ����� ����� ���� ���������� �������� ���������� �������! ������������ ����� - 255 ��������.";
            if (fieldName == GlobalPropertyNames.NameFull)
                return "������������ ������������ �� ������ ������ � ����������";
            if (fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� ������������ ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������, �������� �� ������ ������� ������� �������������� ��� ��������! ������������ ����� �� ����������.";
            if (fieldName == GlobalPropertyNames.Email)
                return "�������� ����������� ����� ������������, ������������ �������� ��� ������������� �����������, ������������� ������, �������� ����������� ��������� ��������. �������� ������������!";
            if (fieldName == "IsActive")
                return "�� ��������� ��������. ��� ���������� ������������ ����������� � �� ����� ����������� ���� � �������.";
            if (fieldName == "IsReadOnly")
                return "�� ��������� ���������. ��� ��������� ������������ �� ����� ������ ����������� ������, ������� ������.";
            
            if (fieldName == "IsAdmin")
                return "�� ��������� ���������. ��� ��������� ������������ �������� ��������������� �������.";
            if (fieldName == "CompanyId")
                return "�������� �������� ������� ����������� ������������. �������� ������������!";
            if (fieldName == GlobalPropertyNames.EmployerId)
                return "��������� ��������������� � ������ �������������. �������� ������������!";
            if (fieldName == GlobalPropertyNames.Password)
                return string.Format("������ �������� ������������! ����������� ����� ������ ����� ���� ���������� ����������� ������� (������� ����������� ����� {0} ��������).", Membership.Provider.MinRequiredPasswordLength);

            if (fieldName == GlobalPropertyNames.Code)
                return "��������� ��� �������������� ������������ ��� ����� �������� ��� � �������������. ������������� ������������ ���������� ��������. ������������ ����� 50 ��������.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "�������������� ��� ������ ��������������: ��� ������� ����� � �������������� ���� - ������ ������������ ������������� ������ ��������. �� ��������� ��� ������ ����������� ����� �������������� ������������ � �������� \"���\".";
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
    }
}