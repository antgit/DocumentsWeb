using System;
using BusinessObjects;

namespace DocumentsWeb.Areas.Kb.Models
{
    public static class KnowledgeData
    {
        public static string GetTooltip(string fieldName)
        {
            if (fieldName == GlobalPropertyNames.Name)
                return "������������ ������ ���� ������ �������� ������������! ���������� �������� �������� ����������� ������� � ��������. ������������ ����� - 255 ��������.";
            
            if (fieldName == GlobalPropertyNames.NameFull)
                return "������������ ������ ���� ������ ������������ ��� ������ � ���������� � �������. ���� �������� �� ������� ������������ \"������������\". ������������ ����� �� ����������.";
            if (fieldName == GlobalPropertyNames.Memo)
                return "���������� ��� �������� ������ ���� ������ ������������ ��� ���������� ��������, �������� ������ ��� � ���������� ������ ������ � ����������. ������������ ����� �� ����������.";


            if (fieldName == GlobalPropertyNames.Code)
                return "��������� ��� ������ ���� ������ ������������ ��� ����� �������� ��� � �������������. ������������� ������������ ���������� ��������. ������������ ����� 50 ��������.";
            if (fieldName == GlobalPropertyNames.CodeFind)
                return "Url ��������� ������ ���� ������.";

            if (fieldName == "cmbMyCompanyId")
                return "�������� ������� ����������� ������ ���� ������.";

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

        public static string GetNullText(string fieldName)
        {
            switch (fieldName)
            {
                case GlobalPropertyNames.Name:
                    return "������� ������������ ������ ���� ������...";
                case GlobalPropertyNames.NameFull:
                    return "������� �������� ������������ ������ ���� ������...";
                case GlobalPropertyNames.Memo:
                    return "������� ���������� ��� �������� ������ ���� ������...";
                case GlobalPropertyNames.Code:
                    return "������� ��� ������ ���� ������...";
                case GlobalPropertyNames.CodeFind:
                    return "������� url ������ ���� ������...";
            }
            return string.Empty;
        }
    }
}