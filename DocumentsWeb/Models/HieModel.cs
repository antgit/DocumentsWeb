using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Models
{
    public class HieModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }

        [Required]
        [Display(Name = "Имя")]
        public String Name { get; set; }

        [Display(Name = "Код")]
        public String Code { get; set; }

        [Display(Name = "Примечание")]
        public String Memo { get; set; }
    }
}