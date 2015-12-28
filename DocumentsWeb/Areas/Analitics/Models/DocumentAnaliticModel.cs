using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects.Documents;
using BusinessObjects;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Analitics.Models
{
    /// <summary>
    /// Модель аналитики документа
    /// </summary>
    public class DocumentAnaliticModel
    {
        /// <summary>
        /// Идентификатор строки
        /// </summary>
        public string RowId { get; set; }

        /// <summary>
        /// Идентификатор аналитики документа
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Дополнительное целое 1
        /// </summary>
        public int? IntValue1 { get; set; }
        //public string IntValue1Name { get; set; }
        public int? IntValue2 { get; set; }
        //public string IntValue2Name { get; set; }
        public int? IntValue3 { get; set; }
        //public string IntValue3Name { get; set; }
        public int? IntValue4 { get; set; }
        //public string IntValue4Name { get; set; }
        public int? IntValue5 { get; set; }
        //public string IntValue5Name { get; set; }

        /// <summary>
        /// Дополнительное дробное
        /// </summary>
        public decimal? SummValue1 { get; set; }
        public decimal? SummValue2 { get; set; }
        public decimal? SummValue3 { get; set; }
        public decimal? SummValue4 { get; set; }
        public decimal? SummValue5 { get; set; }

        /// <summary>
        /// Дополнительная строка
        /// </summary>
        public string StringValue1 { get; set; }
        public string StringValue2 { get; set; }
        public string StringValue3 { get; set; }
        public string StringValue4 { get; set; }
        public string StringValue5 { get; set; }

        private int _GroupId;
        private int _Group2Id;
        private int _Group3Id;
        private int _Group4Id;
        private int _Group5Id;
        /// <summary>
        /// Идентификатор группы
        /// </summary>
        public int? GroupId 
        { 
            get { return _GroupId; }
            set
            {
                _GroupId = (value == null ? 0 : (int)value);
                if (_GroupId > 0)
                {
                    GroupName = AnaliticGroupModel.GetGroups().First(w => w.Id == _GroupId).Name;
                }
            }
        }
        public string GroupName { get; set; }
        public int? Group2Id
        {
            get { return _Group2Id; }
            set
            {
                _Group2Id = (value == null ? 0 : (int)value);
                if (_Group2Id > 0)
                {
                    Group2Name = AnaliticGroupModel.GetGroups().First(w => w.Id == _Group2Id).Name;
                }
            }
        }
        public string Group2Name { get; set; }
        public int? Group3Id
        {
            get { return _Group3Id; }
            set
            {
                _Group3Id = (value == null ? 0 : (int)value);
                if (_Group3Id > 0)
                {
                    Group3Name = AnaliticGroupModel.GetGroups().First(w => w.Id == _Group3Id).Name;
                }
            }
        }
        public string Group3Name { get; set; }
        public int? Group4Id
        {
            get { return _Group4Id; }
            set
            {
                _Group4Id = (value == null ? 0 : (int)value);
                if (_Group4Id > 0)
                {
                    Group4Name = AnaliticGroupModel.GetGroups().First(w => w.Id == _Group4Id).Name;
                }
            }
        }
        public string Group4Name { get; set; }
        public int? Group5Id
        {
            get { return _Group5Id; }
            set
            {
                _Group5Id = (value == null ? 0 : (int)value);
                if (_Group5Id > 0)
                {
                    Group5Name = AnaliticGroupModel.GetGroups().First(w => w.Id == _Group5Id).Name;
                }
            }
        }
        public string Group5Name { get; set; }

        private int _AnaliticId;
        private int _Analitic2Id;
        private int _Analitic3Id;
        private int _Analitic4Id;
        private int _Analitic5Id;
        /// <summary>
        /// Идентификатор аналитики
        /// </summary>
        public int? AnaliticId
        {
            get
            {
                return _AnaliticId;
            }
            set
            {
                if (value != null)
                {
                    _AnaliticId = (int)value;
                    if (_AnaliticId > 0)
                    {
                        AnaliticName = AnaliticModel.GetObject(_AnaliticId).Name;
                    }
                }
            }
        }
        public string AnaliticName { get; set; }
        public int? Analitic2Id
        {
            get
            {
                return _Analitic2Id;
            }
            set
            {
                if (value != null)
                {
                    _Analitic2Id = (int)value;
                    if (_Analitic2Id > 0)
                    {
                        Analitic2Name = AnaliticModel.GetObject(_Analitic2Id).Name;
                    }
                }
            }
        }
        public string Analitic2Name { get; set; }
        public int? Analitic3Id
        {
            get
            {
                return _Analitic3Id;
            }
            set
            {
                if (value != null)
                {
                    _Analitic3Id = (int)value;
                    if (_Analitic3Id > 0)
                    {
                        Analitic3Name = AnaliticModel.GetObject(_Analitic3Id).Name;
                    }
                }
            }
        }
        public string Analitic3Name { get; set; }
        public int? Analitic4Id
        {
            get
            {
                return _Analitic4Id;
            }
            set
            {
                if (value != null)
                {
                    _Analitic4Id = (int)value;
                    if (_Analitic4Id > 0)
                    {
                        Analitic4Name = AnaliticModel.GetObject(_Analitic4Id).Name;
                    }
                }
            }
        }
        public string Analitic4Name { get; set; }
        public int? Analitic5Id
        {
            get
            {
                return _Analitic5Id;
            }
            set
            {
                if (value != null)
                {
                    _Analitic5Id = (int)value;
                    if (_Analitic5Id > 0)
                    {
                        Analitic5Name = AnaliticModel.GetObject(_Analitic5Id).Name;
                    }
                }
            }
        }
        public string Analitic5Name { get; set; }

        /// <summary>
        /// Примечание
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// Идентификатор состояния
        /// </summary>
        public int StateId { get; set; }

        public DocumentAnaliticModel()
        {
            RowId = Guid.NewGuid().ToString();
            IntValue1 = 0;
            IntValue2 = 0;
            IntValue3 = 0;
        }

        public static DocumentAnaliticModel ConvertToModel(DocumentAnalitic da)
        {
            DocumentAnaliticModel model = new DocumentAnaliticModel
            {
                Id = da.Id,
                AnaliticId = (da.AnaliticId == 1 ? 0 : da.AnaliticId),
                Analitic2Id = da.AnaliticId2,
                Analitic3Id = da.AnaliticId3,
                Analitic4Id = da.AnaliticId4,
                Analitic5Id = da.AnaliticId5,
                Memo = da.Memo,
                StateId = da.StateId,
                IntValue1 = da.IntValue1,
                IntValue2 = da.IntValue2,
                IntValue3 = da.IntValue3,
                IntValue4 = da.IntValue4,
                IntValue5 = da.IntValue5,
                SummValue1 = da.SummValue1,
                SummValue2 = da.SummValue2,
                SummValue3 = da.SummValue3,
                SummValue4 = da.SummValue4,
                SummValue5 = da.SummValue5,
                GroupId = da.GroupNo,
                Group2Id = da.GroupNo2,
                Group3Id = da.GroupNo3,
                Group4Id = da.GroupNo4,
                Group5Id = da.GroupNo5,
                StringValue1 = da.StringValue1,
                StringValue2 = da.StringValue2,
                StringValue3 = da.StringValue3,
                StringValue4 = da.StringValue4,
                StringValue5 = da.StringValue5
            };
            return model;
        }

        /// <summary>
        /// Документ по идентификатору
        /// </summary>
        public static DocumentAnaliticModel GetObject(int id)
        {
            DocumentAnalitic obj = WADataProvider.WA.GetObject<DocumentAnalitic>(id);            
            return ConvertToModel(obj);
        }

        public DocumentAnalitic ToObject(Workarea wa)
        {
            DocumentAnalitic da = new DocumentAnalitic
            {
                Id = Id,
                Workarea = wa,
                AnaliticId = ((int)AnaliticId == 0 ? 1 : (int)AnaliticId),
                AnaliticId2 = (int)Analitic2Id,
                AnaliticId3 = (int)Analitic3Id,
                AnaliticId4 = (int)Analitic4Id,
                AnaliticId5 = (int)Analitic5Id,
                Memo = Memo,
                StateId = StateId,
                IntValue1 = (IntValue1 == null ? 0 : (int)IntValue1),
                IntValue2 = (IntValue2 == null ? 0 : (int)IntValue2),
                IntValue3 = (IntValue3 == null ? 0 : (int)IntValue3),
                IntValue4 = (IntValue3 == null ? 0 : (int)IntValue4),
                IntValue5 = (IntValue3 == null ? 0 : (int)IntValue5),
                SummValue1 = SummValue1 == null ? 0 : (decimal)SummValue1,
                SummValue2 = SummValue2 == null ? 0 : (decimal)SummValue2,
                SummValue3 = SummValue3 == null ? 0 : (decimal)SummValue3,
                SummValue4 = SummValue4 == null ? 0 : (decimal)SummValue4,
                SummValue5 = SummValue5 == null ? 0 : (decimal)SummValue5,
                StringValue1 = StringValue1,
                StringValue2 = StringValue2,
                StringValue3 = StringValue3,
                StringValue4 = StringValue4,
                StringValue5 = StringValue5,
                GroupNo = (int)GroupId,
                GroupNo2 = (int)Group2Id,
                GroupNo3 = (int)Group3Id,
                GroupNo4 = (int)Group4Id,
                GroupNo5 = (int)Group5Id
            };
            return da;
        }

        public DocumentAnalitic ToObject(Workarea wa, DocumentAnalitic NewRow)
        {
            NewRow.Id = Id;
            NewRow.Workarea = wa;
            NewRow.AnaliticId = ((int)AnaliticId == 0 ? 1 : (int)AnaliticId);
            NewRow.AnaliticId2 = (int)Analitic2Id;
            NewRow.AnaliticId3 = (int)Analitic3Id;
            NewRow.AnaliticId4 = (int)Analitic4Id;
            NewRow.AnaliticId5 = (int)Analitic5Id;
            NewRow.Memo = Memo;
            NewRow.StateId = StateId;
            NewRow.IntValue1 = (IntValue1 == null ? 0 : (int)IntValue1);
            NewRow.IntValue2 = (IntValue2 == null ? 0 : (int)IntValue2);
            NewRow.IntValue3 = (IntValue3 == null ? 0 : (int)IntValue3);
            NewRow.IntValue4 = (IntValue4 == null ? 0 : (int)IntValue4);
            NewRow.IntValue5 = (IntValue5 == null ? 0 : (int)IntValue5);
            NewRow.SummValue1 = SummValue1 == null ? 0 : (decimal)SummValue1;
            NewRow.SummValue2 = SummValue2 == null ? 0 : (decimal)SummValue2;
            NewRow.SummValue3 = SummValue3 == null ? 0 : (decimal)SummValue3;
            NewRow.SummValue4 = SummValue4 == null ? 0 : (decimal)SummValue4;
            NewRow.SummValue5 = SummValue5 == null ? 0 : (decimal)SummValue5;
            NewRow.StringValue1 = StringValue1;
            NewRow.StringValue2 = StringValue2;
            NewRow.StringValue3 = StringValue3;
            NewRow.StringValue4 = StringValue4;
            NewRow.StringValue5 = StringValue5;
            NewRow.GroupNo = (int)GroupId;
            NewRow.GroupNo2 = (int)Group2Id;
            NewRow.GroupNo3 = (int)Group3Id;
            NewRow.GroupNo4 = (int)Group4Id;
            NewRow.GroupNo5 = (int)Group5Id;
            return NewRow;
        }
    }
}