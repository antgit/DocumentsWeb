using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Models
{
    public class FileVersionModel
    {
        public int Id { get; set; }
        public DateTime? DateModified { get; set; }
        public string UserName { get; set; }

        private byte[] _streamData;
        public byte[] StreamData 
        { 
            get
            {
                if(_streamData==null && Id!=0)
                {
                    FileDataVersion version = new FileDataVersion {Workarea = WADataProvider.WA};
                    version.Load(Id);
                    _streamData = version.StreamData;
                }
                return _streamData;
            }
            set
            {
                _streamData = value;
            }
        }

        public string RowId { get; set; }
        /// <summary>Файл, которому пренадлежит данная версия</summary>
        public FileDataModel Owner { get; set; }

        public FileVersionModel()
        {
            RowId = Guid.NewGuid().ToString();
        }

        public static FileVersionModel ConvertToModel(FileDataVersion value, FileDataModel owner)
        {
            return new FileVersionModel
                       {
                           Id = value.Id,
                           DateModified = value.DateModified,
                           UserName = value.UserName,
                           Owner = owner
                       };
        }

        /// <summary>
        /// Поиск версии файла в памяти по идентификатру её модели
        /// </summary>
        /// <param name="rowId"></param>
        public static FileVersionModel GetByModelId(string rowId)
        {
            foreach (IFileOwner doc in WADataProvider.ModelsCache.Values.Where(s => s.Value is IFileOwner).Select(s => s.Value))
            {
                foreach (var file in doc.Files)
                {
                    FileVersionModel version = null;
                    try
                    {
                        version = file.Versions.FirstOrDefault(s => s.RowId == rowId);
                    }
                    catch { version = null; }
                    if (version != null) return version;
                }
            }
            return null;
        }
    }
}