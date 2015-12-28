using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GPSRobot.Models
{
    public class MessageStateModel
    {
        /// <summary>
        /// Состояние
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }
    }
}