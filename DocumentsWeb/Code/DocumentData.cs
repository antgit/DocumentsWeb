using System;
using System.Linq;
using System.Collections.Generic;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

namespace DocumentsWeb
{
    /// <summary>
    /// �������������� ����� ������ � �����������
    /// </summary>
    public static class DocumentData
    {
        /// <summary>
        /// ������ �������� ���������� ������� ����� ���� ������� � ��������� �����
        /// </summary>
        /// <param name="folderCode">��� ����� ����������</param>
        /// <returns></returns>
        public static List<Document> GetDocumentTemplates(string folderCode)
        {
            if (string.IsNullOrEmpty(folderCode))
                return new List<Document>();

            Folder fldValue = WADataProvider.WA.GetFolderByCodeFind(folderCode);
            
            if(fldValue!=null && fldValue.DocumentId!=0)
            {
                return WADataProvider.WA.GetTemplates<Document>().Where(
                    s => 
                        s.FolderId == fldValue.Id && 
                        s.KindId == fldValue.Document.KindId && 
                        (WADataProvider.IsCompanyIdAllowCreateToCurrentUser(s.MyCompanyId) || s.MyCompanyId==-100)
                ).ToList();
            }
            return new List<Document>();
        }

        /// <summary>
        /// ������ �������� ���������� ������������ ��� ��������� ��������
        /// </summary>
        /// <returns></returns>
        public static List<Document> GetDocumentTemplatesConfig()
        {
            // ������ ���� ���������� �������� �������� 
            List<Document> collSalesConfig = Document.GetCollectionDocumentTemplatesByKind(WADataProvider.WA, DocumentSales.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collServiceConfig = Document.GetCollectionDocumentTemplatesByKind(WADataProvider.WA, DocumentService.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collTaxConfig = Document.GetCollectionDocumentTemplatesByKind(WADataProvider.WA, DocumentTaxes.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collPriceConfig = Document.GetCollectionDocumentTemplatesByKind(WADataProvider.WA, DocumentPrices.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collFinanceConfig = Document.GetCollectionDocumentTemplatesByKind(WADataProvider.WA, DocumentFinance.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);
            return collSalesConfig.Union(collServiceConfig).Union(collTaxConfig).Union(collPriceConfig).Union(collFinanceConfig).ToList();
            
        }

        /// <summary>
        /// �������� �������� ������������ ��� ���������� ��������� � ��������� �������
        /// </summary>
        /// <param name="sourceDocument">�������� � �������� ����������� �������</param>
        /// <param name="signKind">��� ������� (������, ������ ��� ����� �������) </param>
        private static bool AddCurrentUserAsSelfSign(Document sourceDocument, int signKind)
        {
            if(WADataProvider.CurrentUser.AgentId==0)
                return false;
            DocumentSign sign = sourceDocument.Signs().FirstOrDefault(
                f => f.AgentSignId == WADataProvider.CurrentUser.AgentId && f.Kind == signKind);
            if(sign==null)
            {
                DocumentSign newSign = new DocumentSign{Workarea =sourceDocument.Workarea};
                newSign.Owner = sourceDocument;
                newSign.Kind = signKind;
                newSign.Date = sourceDocument.Date;
                newSign.DateSign = DateTime.Today;
                newSign.AgentSignId = WADataProvider.CurrentUser.AgentId;
                newSign.AgentId = WADataProvider.CurrentUser.AgentId;
                newSign.AgentToId = WADataProvider.CurrentUser.AgentId;
                newSign.GroupNo=0;
                newSign.GroupLevelId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(DocumentSign.SIGN_LEVEL_SIGNING).Id;
                newSign.ResolutionId = WADataProvider.WA.Cashe.GetCasheData<Analitic>().ItemCode<Analitic>(Analitic.SYSTEM_SIGN_YES).Id;
                newSign.DatabaseId = sourceDocument.DatabaseId;
                sourceDocument.Signs().Add(newSign);
                return true;
            }
            return true;
        }

        /// <summary>
        /// ���������� �������� �� ���������� ��������� ��� ��� ����������
        /// </summary>
        /// <param name="doc">������������ ��������</param>
        public static void SignDocumentOnSave(Document doc)
        {
            int correspondenceId = WADataProvider.WA.CollectionDocumentKinds.Find(f => f.Id == doc.KindId).CorrespondenceId;

            switch(correspondenceId)
            {
                case 1:
                    //��������
                    AddCurrentUserAsSelfSign(doc, DocumentSign.KINDID_FIRST);
                    break;
                case 2:
                    //���������
                    AddCurrentUserAsSelfSign(doc, DocumentSign.KINDID_SECOND);
                    break;
                case 3:
                    //����������
                    break;
            }
        }

        /// <summary>
        /// ��������� ���������� � ����������� �������
        /// </summary>
        public static List<Document> CollectionDocumentConfigs()
        {
            // ������ ���� ���������� �������� �������� 
            List<Document> collSalesConfig = Document.GetCollectionDocumentByKind(WADataProvider.WA, DocumentSales.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collServiceConfig = Document.GetCollectionDocumentByKind(WADataProvider.WA, DocumentService.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collTaxConfig = Document.GetCollectionDocumentByKind(WADataProvider.WA, DocumentTaxes.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collPriceConfig = Document.GetCollectionDocumentByKind(WADataProvider.WA, DocumentPrices.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            List<Document> collFinanceConfig = Document.GetCollectionDocumentByKind(WADataProvider.WA, DocumentFinance.KINDID_CONFIG, System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                                                 System.Data.SqlTypes.SqlDateTime.MaxValue.Value, WADataProvider.CurrentUserName);

            return collSalesConfig.Union(collServiceConfig).Union(collTaxConfig).Union(collPriceConfig).Union(collFinanceConfig).ToList();
        }
    }
}