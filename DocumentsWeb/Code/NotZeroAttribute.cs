using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Code
{
    public class NotZeroAttribute : ValidationAttribute
    {
        public NotZeroAttribute()
        {
            
        }

        public override bool IsValid(object value)
        {
            if(value is int)
                return ((int) value) != 0;
            if (value is decimal)
                return ((decimal)value) != 0;
            return true;
        }
    }
}