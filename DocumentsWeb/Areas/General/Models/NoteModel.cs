using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Xml.Serialization;
using BusinessObjects;
using BusinessObjects.Documents;
using BusinessObjects.Security;
using DevExpress.Web.ASPxClasses;
using DevExpress.Web.ASPxGridView;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Models
{
    /// <summary>
    /// Примечание
    /// </summary>
    /// <remarks>К полям добавлен префикс Note чтобы они не пересекались с полями документа </remarks>
    public class NoteModel
    {
        public int NoteId { get; set; }
        public string NoteGroupName { get; set; }
        public string NoteCode { get; set; }
        /// <summary>Тема</summary>
        [Required(ErrorMessage = "Укажите тему")]
        public string NoteName { get; set; }
        /// <summary>Примечание</summary>
        [Required(ErrorMessage = "Введите примечание")]
        public string NoteMemo { get; set; }
        public int NoteOrderNo { get; set; }
        public DateTime? NoteDate { get; set; }
        public string NoteUserOwnerName { get; set; }
        public string NoteWorkerName { get; set; }
        public string NoteRowId { get; set; }
        /// <summary>Идентификатор пользователя-владельца</summary>
        public int NoteUserOwnerId { get; set; }

        [NonSerialized]
        private INotesOwner _noteOwner;

        [XmlIgnore]
        public INotesOwner NoteOwner
        {
            get { return _noteOwner; }
            set { _noteOwner = value; }
        }

        public NoteModel()
        {
            NoteRowId = Guid.NewGuid().ToString();
        }

        public Note ToObject()
        {
            Note obj = new Note {Workarea = WADataProvider.WA};

            if (NoteId != 0)
            {
                obj.Load(NoteId);
            }
            else
            {
                obj.KindId = Note.KINDID_USER;
                obj.StateId = State.STATEACTIVE;
                obj.UserOwnerId = WADataProvider.CurrentUser.Id;
                obj.UserName = WADataProvider.CurrentUser.Name;
            }

            obj.Name = NoteName;
            obj.Memo = NoteMemo;

            return obj;
        }

        public static NoteModel ConvertToModel(NoteValueView obj)
        {
            return new NoteModel
                       {
                           NoteId = obj.Id,
                           NoteDate = obj.Date,
                           NoteName = obj.RightName,
                           NoteMemo = obj.RightMemo,
                           NoteGroupName = obj.GroupName,
                           NoteOrderNo = obj.OrderNo,
                           NoteUserOwnerName = obj.UserOwnerName,
                           NoteWorkerName = obj.WorkerName,
                           NoteCode = obj.Code,
                       };
        }

        public static NoteModel ConvertToModel(Note obj)
        {
            return new NoteModel
            {
                NoteId = obj.Id,
                NoteName = obj.Name,
                NoteMemo = obj.Memo,
                NoteUserOwnerName = obj.UserOwner.Name,
                NoteCode = obj.Code,
            };
        }

        public static NoteModel ConvertToModel(GridViewEditFormTemplateContainer c)
        {
            return c.ItemIndex >= 0
                       ? new NoteModel
                             {
                                 NoteId = (int)DataBinder.Eval(c.DataItem, "NoteId"),
                                 NoteDate = (DateTime?)DataBinder.Eval(c.DataItem, "NoteDate"),
                                 NoteName = (string)DataBinder.Eval(c.DataItem, "NoteName"),
                                 NoteMemo = (string)DataBinder.Eval(c.DataItem, "NoteMemo"),
                                 NoteGroupName = (string)DataBinder.Eval(c.DataItem, "NoteGroupName"),
                                 NoteOrderNo = (int)DataBinder.Eval(c.DataItem, "NoteOrderNo"),
                                 NoteUserOwnerName = (string)DataBinder.Eval(c.DataItem, "NoteUserOwnerName"),
                                 NoteWorkerName = (string)DataBinder.Eval(c.DataItem, "NoteWorkerName"),
                             }
                       : new NoteModel
                             {
                                 NoteId = 0,
                                 NoteDate = DateTime.Now,
                                 NoteName = string.Empty,
                                 NoteMemo = string.Empty,
                                 NoteGroupName = string.Empty,
                                 NoteOrderNo = 0,
                                 NoteUserOwnerName = string.Empty,
                                 NoteWorkerName = string.Empty,
                             };
        }

        public static List<NoteModel> GetCollection(Document doc)
        {
            List<NoteValueView> collNoteView = NoteValueView.GetView<Document>(doc, true);
            return collNoteView.Select(ConvertToModel).ToList();
        }

        public static List<NoteModel> GetCollection(int docId)
        {
            Document doc = WADataProvider.WA.Cashe.GetCasheData<Document>().Item(docId);
            return GetCollection(doc);
        }

        /// <summary>
        /// Определение возможности добавления примечаний для текущего пользователя
        /// </summary>
        /// <param name="owner">Куда добавляется примечание (для объектов, не являющихся наследниками DocumentModel, всегда возвращается true)</param>
        /// <returns></returns>
        public static bool CanCreateNotes(INotesOwner owner)
        {
            DocumentModel doc = owner as DocumentModel;
            if (doc == null)
                return true;

            return WADataProvider.FolderElementRightView.IsAllow(Right.DOCNOTE, doc.FolderId);
        }
    }
}