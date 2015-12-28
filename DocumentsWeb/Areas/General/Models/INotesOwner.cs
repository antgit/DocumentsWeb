using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentsWeb.Areas.General.Models
{
    public interface INotesOwner
    {
        int Id { get; set; }
        string ModelId { get; set; }
        List<NoteModel> Notes { get; set; }
    }
}
