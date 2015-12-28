using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DocumentsWeb.Models
{
    public class PeriodModel
    {
        /// <summary>
        /// Действие по изменению периода
        /// </summary>
        public string PeriodAction { get; set; }

        private DateTime _Start;
        /// <summary>
        /// Начало периода
        /// </summary>
        public DateTime periodStart
        {
            get
            {
                return _Start;
            }

            set
            {
                if (value > _End)
                {
                    _Start = _End;
                }
                else
                {
                    _Start = value;
                }
            }
        }

        private DateTime _End;
        /// <summary>
        /// Конец периода
        /// </summary>
        public DateTime periodEnd
        {
            get
            {
                return _End;
            }

            set
            {
                if (value < _Start)
                {
                    _End = _Start;
                }
                else
                {
                    _End = value;
                }
            }
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        public PeriodModel()
        {
            //_Start = DateTime.Today;
            //_End = DateTime.Today;
            this.SetCurYear();
        }

        /// <summary>
        /// Вчера
        /// </summary>
        public void setYesterday()
        {
            _Start = DateTime.Today.AddDays(-1);
            _End = DateTime.Today.AddDays(-1);
        }

        /// <summary>
        /// Сегодня
        /// </summary>
        public void SetToday()
        {
            _Start = DateTime.Today;
            _End = DateTime.Today;
        }

        /// <summary>
        /// Завтра
        /// </summary>
        public void SetTomorrow()
        {
            _Start = DateTime.Today.AddDays(1);
            _End = DateTime.Today.AddDays(1);
        }

        /// <summary>
        /// Текущий месяц
        /// </summary>
        public void SetCurrMonth()
        {
            var yr = DateTime.Today.Year;
            var mth = DateTime.Today.Month;
            _Start = new DateTime(yr, mth, 1).AddMonths(-1);
            _End = new DateTime(yr, mth, 1).AddDays(-1);
        }

        /// <summary>
        /// Текущий год
        /// </summary>
        public void SetCurYear()
        {
            var yr = DateTime.Today.Year;
            _Start = new DateTime(yr, 1, 1);
            _End = new DateTime(yr, 12, 31);
        }

        /// <summary>
        /// Загрузка периода для текущего пользователя
        /// </summary>
        public void Load()
        {
            //TODO: доделать
        }

        /// <summary>
        /// Сохраняет период для текущего пользователя
        /// </summary>
        public void Save()
        {
            //TODO: доделать
        }

        /// <summary>
        /// Период в текстовом виде
        /// </summary>
        /// <returns></returns>
        public string GetTextPeriod()
        {
            if (periodStart == periodEnd)
                return periodStart.ToShortDateString();
            else
                return string.Format("{0} - {1}", periodStart.ToShortDateString(), periodEnd.ToShortDateString());
        }

        public override string ToString()
        {
            string tool_tip = "<b>Рабочий период</b> определяет текущие даты <b>начала</b> и <b>окончания</b> за которые отображается <b>список документов</b>,<br />Рабочий период влияет на построение <b>отчетов</b> в системе:<br>- отчеты могут быть построены  с учетом ограничения периода <b>начала</b> и <b>окончания</b><br />- отчеты могут быть построены с учетом даты <b>начала</b> ограничения периода <br /> - отчеты могут быть построены с учетом даты <b>окончания</b> ограничения периода<br /><br /><b>Текущий рабочий период - {0}</b>";
            return string.Format(tool_tip, GetTextPeriod());
        }
    }
}