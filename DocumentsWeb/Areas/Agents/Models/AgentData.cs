using BusinessObjects;

namespace DocumentsWeb.Areas.Agents.Models
{
    public static class AgentData
    {
        public static string GetTooltip(string controller, string fieldName)
        {
            if (controller == "MyDepatment" && fieldName == GlobalPropertyNames.Name)
                return "������������ �������������� \"���� �����������\" �������� ������������! ���������� �������� �������� ����������� ������� � ��������. ������������ ����� - 255 ��������.";

            if (controller == "Store" && fieldName == GlobalPropertyNames.Name)
                return "������������ �������������� \"�����\" �������� ������������! ��������� �������� ����������� ������� � ��������, ������������� �� ��������� ��������� ������� � ���������� �������������. ������������ ����� - 255 ��������.";
            
            if (controller == "MyDepatment" && fieldName == GlobalPropertyNames.NameFull)
                return "�������� ������������ ��� ������ � ���������� � �������. ���� �������� �� ������� ������������ \"������������\". ������������ ����� �� ����������.";
            
            if (controller == "MyDepatment" && fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� �������������� \"���� �����������\" ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������, �������� �� ������ ������� ����� ����� ��������! ������������ ����� �� ����������.";
            
            if (controller == "Store" && fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� ������ ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������, �������� �� ������ ������� ����� ����������� ������! ������������ ����� �� ����������.";

            if (controller == "Worker" && fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� ����������� ���� ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������, �������� �� ������ ������� ��������� ����� ��������! ������������ ����� �� ����������.";

            if (fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� �������������� ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������, �������� �� ������ ������� ����� ����� ��������! ������������ ����� �� ����������.";

            if (controller == "Worker" && fieldName == GlobalPropertyNames.AddressLegal)
                return "����� �������� �� ������ ���������� ������.";
            if (controller == "Worker" && fieldName == GlobalPropertyNames.AddressPhysical)
                return "����������� ����� ���������� �� ������� ������ �������.";

            if (fieldName == GlobalPropertyNames.AddressLegal)
                return "����������� ����� ����������� �� ������ ������ � ����������� �����������.";
            if (fieldName == GlobalPropertyNames.AddressPhysical)
                return "����������� ����� ����������� �� ������� ������ �������.";

            if (fieldName == GlobalPropertyNames.AddressLegal)
                return "����������� ����� ����������� �� ������ ������ � ����������� �����������.";
            if (fieldName == GlobalPropertyNames.AddressPhysical)
                return "����������� ����� ����������� �� ������� ������ �������.";

            if (fieldName == GlobalPropertyNames.NdsPayer)
                return "�������� �� ����������� ������������ ���";

            if (controller == "Store" && fieldName == GlobalPropertyNames.Phone)
                return "�������� ������� ������";
            if (controller == "Worker" && fieldName == GlobalPropertyNames.Phone)
                return "�������� ������� ����������";
            if (fieldName == GlobalPropertyNames.Phone)
                return "�������� ������� �����������";

            if (fieldName == GlobalPropertyNames.Code)
                return "��������� ��� �������������� ������������ ��� ����� �������� ��� � �������������. ������������� ������������ ���������� ��������. ������������ ����� 50 ��������.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "�������������� ��� ������ ��������������: ��� ������� ����� � �������������� ���� - ������ ������������ ������������� ������ ��������. �� ��������� ��� ������ ����������� ����� �������������� ������������ � �������� \"���\".";


            if (fieldName == GlobalPropertyNames.OwnershipId)
                return "����� ������������� ��������������. ���������� �� ���������������� ����������� ��������� \"����� �������������\".";

            if (fieldName == GlobalPropertyNames.InternationalName)
                return "������������� ������������ ��������������, � ����������� ������� �������� ������������ �� ���������� �����.";

            if (fieldName == GlobalPropertyNames.DateModified)
                return "���� ���������� ��������� ������";
            if (fieldName == GlobalPropertyNames.Id)
                return "���������� �������� �������������";
            if (fieldName == GlobalPropertyNames.UserName)
                return "��� ������������ ���������� ��� ����������� ������ � ��������� ���";

            if (controller == "Store" && fieldName == "cmbMyCompanyId")
                return "�������� ������� ����������� �����. �������� �������� ����� �������� ������ ����������!";            
            if (fieldName == GlobalPropertyNames.MyCompanyId)

                return "������������� ��������-��������� ������";
            if (fieldName == GlobalPropertyNames.MyCompanyName)
                return "������������ ��������-��������� ������";
            return string.Empty;
        }
    }
}