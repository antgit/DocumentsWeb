using System.Linq;
using System.Web.Mvc;
using BusinessObjects.Security;
using DevExpress.Web.Mvc;
using DocumentsWeb.Areas.General.Models;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.General.Controllers
{
    [MultiAuthorize(Roles = new[] { Uid.GROUP_GROUPWEBUSER, Uid.GROUP_GROUPWEBADMIN })]
    public class NoteController : Controller
    {
        public ActionResult NotesGrid(string modelId)
        {
            INotesOwner owner = (INotesOwner) WADataProvider.ModelsCache.Get(modelId);
            return View(owner);
        }

        public ActionResult AddNote([ModelBinder(typeof(DevExpressEditorsBinder))]NoteModel model, string modelId)
        {
            INotesOwner owner = (INotesOwner)WADataProvider.ModelsCache.Get(modelId);
            if (ModelState.IsValid)
            {
                model.NoteOwner = owner;
                model.NoteWorkerName = WADataProvider.CurrentUser.Agent == null ? string.Empty : WADataProvider.CurrentUser.Agent.Name;
                model.NoteUserOwnerId = WADataProvider.CurrentUser.Id;
                owner.Notes.Add(model);
            }
            return View("NotesGrid", owner);
        }

        public ActionResult UpdateNote([ModelBinder(typeof(DevExpressEditorsBinder))]NoteModel model, string modelId)
        {
            INotesOwner owner = (INotesOwner)WADataProvider.ModelsCache.Get(modelId);
            if (ModelState.IsValid)
            {
                NoteModel note = owner.Notes.FirstOrDefault(n => n.NoteRowId == model.NoteRowId);
                if (note == null)
                    return View("NotesGrid", owner);

                //noteModel.NoteGroupName = note.NoteGroupName;
                note.NoteName = model.NoteName;
                note.NoteDate = model.NoteDate;
                note.NoteMemo = model.NoteMemo;
                note.NoteOrderNo = model.NoteOrderNo;
                note.NoteUserOwnerName = model.NoteUserOwnerName;
                note.NoteWorkerName = model.NoteWorkerName;
            }
            return View("NotesGrid", owner);
        }

        public ActionResult DeleteNote(string noteRowId, string modelId)
        {
            INotesOwner owner = (INotesOwner)WADataProvider.ModelsCache.Get(modelId);
            owner.Notes.RemoveAll(n => n.NoteRowId == noteRowId);
            return View("NotesGrid", owner);
        }
    }
}
