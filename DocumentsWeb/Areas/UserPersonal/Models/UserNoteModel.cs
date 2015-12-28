using System;
using System.Collections.Generic;
using System.Linq;
using BusinessObjects;
using BusinessObjects.Security;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.UserPersonal.Models
{
    /// <summary>������ ����������� ������� ������������</summary>
    public class UserNoteModel
    {
        #region ��������
        /// <summary>������������� ������</summary>
        public string ModelId { get; set; }
        /// <summary>�������������</summary>
        public int Id { get; set; }
        public DateTime? DateModified { get; set; }

        /// <summary>������������</summary>
        public string Name { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        public string Memo { get; set; }
        /// <summary>������������� ������������-���������</summary>
        public int UserOwnerId { get; set; }
        /// <summary>������������� �������</summary>
        public int TemplateId { get; set; }
        public int KindId { get; set; }
        #endregion
        /// <summary>
        /// �����������
        /// </summary>
        public UserNoteModel()
        {
            ModelId = Guid.NewGuid().ToString();
            Note tml = WADataProvider.WA.GetTemplates<Note>().FirstOrDefault(f => f.KindId == Note.KINDVALUE_PERSONAL);
            if(tml!=null)
            {
                TemplateId = tml.Id;
                Name = tml.Name;
                KindId = tml.KindId;
            }
            UserOwnerId = WADataProvider.CurrentUser.Id;
        }
        public static UserNoteModel GetObject(int id)
        {
            Note obj = null;
            if (id != 0)
            {
                obj = WADataProvider.WA.Cashe.GetCasheData<Note>().Item(id);
            }
            return ConvertToModel(obj);
        }
        /// <summary>������������� � ������</summary>
        /// <param name="workarea">������� �������</param>
        /// <returns></returns>
        public Note ToObject()
        {
            Note obj = new Note {Workarea = WADataProvider.WA};
            if(Id!=0)
            {
                obj = WADataProvider.WA.Cashe.GetCasheData<Note>().Item(Id);
            }
            if (obj.Id == 0)
            {
                obj.IsNew = true;
                obj.StateId = State.STATEACTIVE;
                obj.KindId = Note.KINDID_PERSONAL;
                obj.UserOwnerId = WADataProvider.CurrentUser.Id;
                obj.MyCompanyId = WADataProvider.CurrentUser.MyCompanyId;
            }
            
            obj.Name = Name;

            if (string.IsNullOrEmpty(Memo))
            {
                obj.Memo = "����� �����������...";
                obj.Name = obj.Memo;

            }
            else
            {
                obj.Memo = Memo;
                if (Memo.Length > 100)
                    obj.Name = Memo.Substring(0, 100);
                else
                    obj.Name = Memo;
            }
            return obj;
        }
        /// <summary>������������� � ������</summary>
        /// <param name="value">�������</param>
        /// <returns></returns>
        public static UserNoteModel ConvertToModel(Note value)
        {
            UserNoteModel messageModel = new UserNoteModel
            {
                Id = value.Id,
                DateModified = value.DateModified,
                Name = value.Name,
                Memo = value.Memo,
                UserOwnerId = value.UserOwnerId,
                KindId = value.KindId
            };
            
            return messageModel;
        }
        /// <summary>������ ����������� �������</summary>
        /// <param name="refresh">��������� ������ �� ���� ������</param>
        /// <returns></returns>
        public static List<UserNoteModel> GetMyNotes(bool refresh=false)
        {
            return WADataProvider.WA.GetCollection<Note>(refresh).Where(
                f => f.UserOwnerId == WADataProvider.CurrentUser.Id && f.IsStateAllow && f.KindId== Note.KINDID_PERSONAL).Select(ConvertToModel).ToList();
        }
        public static void ToTrash(int id)
        {
            Note obj = WADataProvider.WA.Cashe.GetCasheData<Note>().Item(id);
            obj.Remove();
        }
    }
}