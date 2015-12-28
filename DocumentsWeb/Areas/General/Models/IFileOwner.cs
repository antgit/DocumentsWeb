using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentsWeb.Areas.General.Models
{
    /// <summary>
    /// Интерфейс для классов, содержащих в себе коллекцию фалов
    /// </summary>
    public interface IFileOwner
    {
        int Id { get; set; }
        string ModelId { get; set; }
        List<FileDataModel> Files { get; set; }
    }
}
