using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Workflow.Activities.Rules;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;
using BusinessObjects;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Models;
using BusinessObjects.Security;

namespace DocumentsWeb.Code
{
    public static class Extensions
    {
        public static RuleSet DeserializeRuleSet(this Ruleset value)
        {
            if (!String.IsNullOrEmpty(value.Value))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                StringReader stringReader = new StringReader(value.Value);
                XmlTextReader reader = new XmlTextReader(stringReader);
                return serializer.Deserialize(reader) as RuleSet;
            }
            return null;
        }
        /// <summary>
        /// Сохранение связангных фалов
        /// </summary>
        /// <typeparam name="T">Тип сущности, к которой привязаны файлы</typeparam>
        /// <param name="owner">Сущность</param>
        /// <param name="linkedFiles">Исходная коллекция связанных файлов</param>
        public static void SaveFiles<T>(this IFileOwner owner, List<IChainAdvanced<T, FileData>> linkedFiles) where T : class, ICoreObject, new()
        {
            if (owner == null) return;
            if (linkedFiles == null) return;

            T item = new T {Workarea = WADataProvider.WA};
            item.Load(owner.Id);
            List<IChainAdvanced<T, FileData>> fileLinks = linkedFiles.Where(s => s.StateId != State.STATEDELETED).ToList();
            if (owner.Files == null)
                return;
            //Insert
            foreach (var fileModel in owner.Files.Where(s => s.Id == 0))
            {
                //Данные
                FileData fileData = fileModel.ToObject(WADataProvider.WA);
                fileData.Save();

                //Связь
                ChainAdvanced<T, FileData> link =
                    new ChainAdvanced<T, FileData>(item) { Right = fileData, StateId = State.STATEACTIVE, KindId = 13 };
                link.Save();

                //Добавление в иерархию
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>("CONTRACTSFILES");
                h.ContentAdd(fileData);
            }

            //Update
            foreach (var fileModel in owner.Files.Where(s => s.Id != 0 && s.IsChanged))
            {
                FileData fileData = fileModel.ToObject(WADataProvider.WA);
                fileData.Save();
            }

            //Дополнительные коды
            foreach (FileDataModel fileModel in owner.Files.Where(s => s.IsChanged))
            {
                fileModel.SaveCodes();
            }

            //Delete
            foreach (var link in fileLinks.Where(link => !owner.Files.Exists(s => s.Id == link.Right.Id)))
            {
                link.StateId = State.STATEDELETED;
                link.Save();
            }
        }

        public static void SaveNotes<T>(this INotesOwner owner, List<IChainAdvanced<T, Note>> linkedNotes) where T : class, IBase, new()
        {
            if (owner == null) return;
            if (linkedNotes == null) return;
            T item = new T { Workarea = WADataProvider.WA };
            item.Load(owner.Id);
            List<IChainAdvanced<T, Note>> fileLinks = linkedNotes.Where(s => s.StateId != State.STATEDELETED).ToList();

            if (owner.Notes == null)
                return;
            //Insert
            int chainKindId = WADataProvider.WA.CollectionChainKinds.Find(f => f.Code == ChainKind.NOTES & f.FromEntityId == item.EntityId).Id;
            foreach (NoteModel noteModel in owner.Notes.Where(s => s.NoteId == 0))
            {
                //Данные
                Note noteData = noteModel.ToObject();
                noteData.Save();

                //Связь
                ChainNotes<T> link =
                    new ChainNotes<T>(item) { Right = noteData, StateId = State.STATEACTIVE, KindId = chainKindId };
                link.UserOwnerId = WADataProvider.CurrentUser.Id;
                link.Save();

                
                //Добавление в иерархию
                Hierarchy h = WADataProvider.WA.Cashe.GetCasheData<Hierarchy>().ItemCode<Hierarchy>(Hierarchy.GetSystemFavoriteCodeValue(WhellKnownDbEntity.Note));
                h.ContentAdd(noteData);
            }

            //Update
            foreach (var noteModel in owner.Notes.Where(s => s.NoteId != 0 /*&& s.IsChanged*/))
            {
                Note noteData = noteModel.ToObject();
                noteData.Save();
            }

            //Delete
            foreach (var link in fileLinks.Where(link => !owner.Notes.Exists(s => s.NoteId == link.Right.Id)))
            {
                link.StateId = State.STATEDELETED;
                link.Save();
            }
        }

        /// <summary>
        /// Возвращает список компаний доступных для просмотра пользователю
        /// </summary>
        /// <param name="?"></param>
        /// <returns></returns>
        public static List<int> GetCompaniesList(this Uid owner)
        {
            List<int> userScopeView = DocumentsWeb.Models.WADataProvider.WA.Access.GetCompanyScopeView(HttpContext.Current.User.Identity.Name);
            return userScopeView;
        }
    }
}