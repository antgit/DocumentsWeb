using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Models
{
    public class FileDataModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FileExtention { get; set; }
        public bool IsChanged { get; set; }
        public bool AllowVersion { get; set; }

        private byte[] _streamData;
        public byte[] StreamData {
            get
            {
                if(Id!=0 && _streamData==null)
                {
                    FileData fileData = new FileData {Workarea = WADataProvider.WA};
                    fileData.Load(Id);
                    _streamData = fileData.StreamData;
                }
                return _streamData;
            }
            set { _streamData = value; }
        }

        public string RowId { get; set; }
        public string UserName { get; set; }
        public string Category { get; set; }
        public string ContentType { get; set; }
        public string Theme { get; set; }

        private List<FileVersionModel> _versions;
        public List<FileVersionModel> Versions 
        { 
            get
            {
                if (_versions == null && Id!=0)
                {
                    FileData fileData = new FileData {Workarea = WADataProvider.WA};
                    fileData.Load(Id);
                    _versions = fileData.GetAllVersions(true).Select(s => FileVersionModel.ConvertToModel(s, this)).ToList();
                }
                return _versions;
            }
        }

        public FileDataModel()
        {
            RowId = Guid.NewGuid().ToString();
            IsChanged = false;
        }

        public static FileDataModel ConvertToModel(FileData value)
        {
            //Дополнительные коды
            List<CodeValueView> collCodeView = value.GetView(false);
            List<CodeName> codeNames = WADataProvider.WA.GetCollection<CodeName>().Where(s=>s.ToEntityId==(int)WhellKnownDbEntity.FileData).ToList();
            CodeValueView category = collCodeView.FirstOrDefault(s => s.CodeNameId == codeNames.FirstOrDefault(cn => cn.Code == "CATEGORY").Id);
            CodeValueView contentType = collCodeView.FirstOrDefault(s => s.CodeNameId == codeNames.FirstOrDefault(cn => cn.Code == "CONTENTTYPE").Id);
            CodeValueView theme = collCodeView.FirstOrDefault(s => s.CodeNameId == codeNames.FirstOrDefault(cn => cn.Code == "THEME").Id);

            return new FileDataModel
                       {
                           Id = value.Id,
                           Name = value.NameFull,
                           UserName = value.UserName,
                           AllowVersion = value.AllowVersion,
                           Category = category == null ? "" : category.Value,
                           ContentType = contentType == null ? "" : contentType.Value,
                           Theme = theme == null ? "" : theme.Value,
                           FileExtention = value.FileExtention
                       };
        }

        public FileData ToObject(Workarea workarea)
        {
            FileData fileData = new FileData {Workarea = workarea};
            fileData.Load(Id);

            if(Id==0)
            {
                //New
                UserName = HttpContext.Current.User.Identity.Name;
            }

            fileData.Name = Path.GetFileNameWithoutExtension(Name);
            fileData.AllowVersion = AllowVersion;
            fileData.FileExtention = Path.GetExtension(Name).Length > 1
                                         ? Path.GetExtension(Name).Substring(1)
                                         : string.Empty;
            fileData.KindId = FileData.KINDID_FILEDATA;
            fileData.StreamData = StreamData;
            fileData.UserName = UserName;
            return fileData;
        }

        private void SaveCode(FileData fileData, IEnumerable<CodeValueView> codeList, int codeNameId, string value)
        {
            CodeValueView codeValueView = codeList.FirstOrDefault(s => s.CodeNameId == codeNameId);
            CodeValue<FileData> codeValue  = new CodeValue<FileData> {Workarea = WADataProvider.WA, Element = fileData};

            if(codeValueView!=null)
                codeValue.Load(codeValueView.Id);

            codeValue.ElementId = Id;
            codeValue.CodeNameId = codeNameId;
            codeValue.Value = value;
            if (value != null && value.Length > 0)
            {
                codeValue.Save();
            }
        }

        public void SaveCodes(string saveParam = "_default")
        {
            FileData fileData = WADataProvider.WA.GetObject<FileData>(Id);
            List<CodeName> codeNames = WADataProvider.WA.GetCollection<CodeName>().Where(s => s.ToEntityId == (int)WhellKnownDbEntity.FileData).ToList();
            List<CodeValueView> collCodeView = fileData.GetView(false);

            if(saveParam =="_dafault") //"file-to-document" chain
            {
                SaveCode(fileData, collCodeView, codeNames.FirstOrDefault(cn => cn.Code == "CATEGORY").Id, Category);
                SaveCode(fileData, collCodeView, codeNames.FirstOrDefault(cn => cn.Code == "CONTENTTYPE").Id, ContentType);
                SaveCode(fileData, collCodeView, codeNames.FirstOrDefault(cn => cn.Code == "THEME").Id, Theme);    
            }

            if (saveParam == "MESSAGE") //"file-to-message" chain
            {                
                //SaveCode(fileData, collCodeView, codeNames.FirstOrDefault(cn => cn.Code == "CONTENTTYPE").Id, ContentType);                
            }
            
        }
        
        /// <summary>
        /// Поиск файла в памяти по идентификатру его модели
        /// </summary>
        /// <param name="rowId"></param>
        public static FileDataModel GetByModelId(string rowId)
        {
            foreach (IFileOwner doc in WADataProvider.ModelsCache.Values.Where(s => s.Value is IFileOwner).Select(s => s.Value))
            {
                FileDataModel fileDataModel = doc.Files.FirstOrDefault(s => s.RowId == rowId);
                if (fileDataModel != null) return fileDataModel;
            }
            return null;
        }

        public static List<FileDataModel> GetCollection(string HierarchyCode, bool Refresh = false)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(HierarchyCode);
            List<FileData> coll = h.GetTypeContents<FileData>(true, Refresh);
            if (Refresh) WADataProvider.RefreshLibrariesElementRightView(HttpContext.Current.User.Identity.Name);
            return coll.Select(ConvertToModel).OrderBy(o => o.Name).ToList();
        }
        /// <summary>
        /// Коллекция файлов для связывания
        /// </summary>
        /// <param name="HierarchyCode">Код иерархии</param>
        /// <returns></returns>
        public static List<FileDataModel> GetCollectionBind(string HierarchyCode, int? currentSelectedId = null)
        {
            Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(HierarchyCode);
            List<FileData> coll = h.GetTypeContents<FileData>(true);

            if (currentSelectedId.HasValue && currentSelectedId.Value != 0)
            {
                if (!coll.Exists(f => f.Id == currentSelectedId.Value))
                {
                    FileData objCurrent = WADataProvider.WA.Cashe.GetCasheData<FileData>().Item(currentSelectedId.Value);
                    if (objCurrent.Id!=0)
                        coll.Add(objCurrent);
                }
            }
            return coll.Select(ConvertToModel).OrderBy(o => o.Name).ToList();
        }

        /// <summary>
        /// Получение типа содержимого фала по его расширению
        /// </summary>
        /// <param name="fileExtension">Расширение файла</param>
        /// <returns>Тип содержимого</returns>
        public static string GetContentType(string fileExtension)
        {
            switch (fileExtension)
            {
                case "pdf":
                    return "application/pdf";
                case "djv":
                case "djvu":
                    return "image/vnd.djvu";
                case "doc":
                    return "application/msword";
                case "docx":
                    return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
                case "xls":
                    return "application/vnd.ms-excel";
                case "xlsx":
                    return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                case "jpeg":
                case "jpg":
                    return "image/jpeg";
                case "png":
                    return "image/png";
                case "html":
                case "htm":
                    return "text/html";
                default:
                    return "application/octet-stream";
            }
        }
    }
}