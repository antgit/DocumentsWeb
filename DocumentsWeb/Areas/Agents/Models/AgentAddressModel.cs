using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BusinessObjects;
using System.ComponentModel.DataAnnotations;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Agents.Models
{
    public class AgentAddressModel
    {
        public string ModelId { get; set; }

        public int Id { get; set; }
        public string StreetName { get; set; }
        public string HouseNumber { get; set; }
        public string PostIndex { get; set; }

        private int _CountryId;
        public int? CountryId { 
            get {
                if (_CountryId == 0) {
                    return null;
                } else {
                    return _CountryId;
                }
            }
            set {
                if (value == null)
                {
                    _CountryId = 0;
                }
                else
                {
                    _CountryId = (int)value;
                }
            }
        }
        public string CountryName { get; set; }

        private int _TerritoryId;
        public int? TerritoryId {
            get
            {
                if (_TerritoryId == 0)
                {
                    return null;
                }
                else
                {
                    return _TerritoryId;
                }
            }
            set
            {
                if (value == null)
                {
                    _TerritoryId = 0;
                }
                else
                {
                    _TerritoryId = (int)value;
                }
            }
        }
        public string TerritoryName { get; set; }

        private int _TownId;
        public int? TownId {
            get
            {
                if (_TownId == 0)
                {
                    return null;
                }
                else
                {
                    return _TownId;
                }
            }
            set
            {
                if (value == null)
                {
                    _TownId = 0;
                }
                else
                {
                    _TownId = (int)value;
                }
            }
        }
        public string TownName { get; set; }

        private int _RegionId;
        public int? RegionId {
            get
            {
                if (_RegionId == 0)
                {
                    return null;
                }
                else
                {
                    return _RegionId;
                }
            }
            set
            {
                if (value == null)
                {
                    _RegionId = 0;
                }
                else
                {
                    _RegionId = (int)value;
                }
            }
        }
        public int RegionName { get; set; }

        public int OwnId { get; set; }
        public decimal? X { get; set; }
        public decimal? Y { get; set; }
        public int ZoneRadius { get; set; }
        
        public int AddressKindId { get; set; }
        public string AddressKindName { get; set; }

        public int StateId { get; set; }

        public AgentAddressModel()
        {
            StateId = State.STATEACTIVE;
            ModelId = Guid.NewGuid().ToString();
        }

        /// <summary>Координата X для отображения адреса на карте. Если адрес не задан, возвращается координата города</summary>
        public decimal NavigateX
        {
            get { return (X ?? 0) == 0 ? ToObject().Town.X : X ?? 0; }
        }
        /// <summary>Координата Y для отображения адреса на карте. Если адрес не задан, возвращается координата города</summary>
        public decimal NavigateY
        {
            get { return (Y ?? 0) == 0 ? ToObject().Town.Y : Y ?? 0; }
        }
        /// <summary>Высота над картой</summary>
        public decimal NavigateZ
        {
            get { return (X ?? 0) == 0 && (Y ?? 0) == 0 ? 22 : 18; }
        }

        /// <summary>
        /// Проверят правельность введенного адреса
        /// </summary>
        /// <returns></returns>
        public bool Validate()
        {
            bool isValid = true;

            if (StreetName == null || StreetName.Length < 3)
            {
                isValid = false;
            }
            /*else if (HouseNumber == null || HouseNumber.Length < 1)
            {
                isValid = false;
            }*/

            return isValid;
        }

        public string GetActualName()
        {
            Country c = new Country { Workarea = WADataProvider.WA };
            c.Load(_CountryId);
            Territory t = new Territory { Workarea = WADataProvider.WA };
            t.Load(_TerritoryId);
            Territory r = new Territory { Workarea = WADataProvider.WA };
            r.Load(_RegionId);
            Town tw = new Town { Workarea = WADataProvider.WA };
            tw.Load(_TownId);
            string actual_name = c.Name + " " + t.Name + " " + r.Name + " " + tw.Name + " " + PostIndex + " " + StreetName + " " + HouseNumber;
            return actual_name;
        }

        public string GetShortName()
        {
            Town tw = new Town { Workarea = WADataProvider.WA };
            tw.Load(_TownId);
            string short_name = tw.Name + " " + PostIndex + " " + StreetName + " " + HouseNumber;
            return short_name;
        }

        public static AgentAddressModel ConvertToModel(AgentAddress value)
        {
            return new AgentAddressModel
            {
                Id = value.Id,
                StreetName = value.Name,
                HouseNumber = value.Code,
                PostIndex =value.PostIndex,
                CountryId = value.CountryId,
                CountryName = value.Country == null ? "" : value.Country.Name,
                TerritoryId = value.TerritoryId,
                TerritoryName = value.Territory == null ? "" : value.Territory.Name,
                TownId = value.TownId,
                TownName = value.Town == null ? "" : value.Town.Name,
                OwnId = value.OwnId,
                X = value.X,
                Y = value.Y,
                ZoneRadius = value.ZoneRadius,
                AddressKindId = value.KindId,
                AddressKindName = value.KindName,
                RegionId = value.RegionId,
                StateId = value.StateId
            };
        }

        public static AgentAddressModel GetMktgAddressByAgentId(int ag_id)
        {
            Agent ag = new Agent { Workarea = WADataProvider.WA };
            ag.Load(ag_id);
            if (ag.Id > 0)
            {
                for (int i = 0; i < ag.AddressCollection.Count; i++)
                {
                    if (ag.AddressCollection[i].KindId == AgentAddress.KINDID_ACTUALADDRESS)
                    {
                        return AgentAddressModel.ConvertToModel(ag.AddressCollection[i]);
                    }
                }
            }
            return null;
        }

        public AgentAddress ToObject()
        {
            AgentAddress address = new AgentAddress {Workarea = WADataProvider.WA};
            address.Load(Id);
            if(Id==0)
            {
                address.IsNew = true;
                address.OwnId = OwnId;
            }
            address.StateId = StateId;
            address.KindId = AddressKindId == 0 ? AgentAddress.KINDID_ACTUALADDRESS : AddressKindId;
            //address.KindName = KindId > 0 ? address.Entity.EntityKinds.First(w => w.Id == KindId).Name : string.Empty;
            address.CountryId = _CountryId;
            address.TerritoryId = _TerritoryId;
            address.TownId = _TownId;
            address.Name = StreetName;
            address.Code = HouseNumber;
            address.PostIndex = PostIndex;
            address.NameFull = address.Country.Name + address.Territory.Name + address.Town.Name;
            address.X = X ?? 0;
            address.Y = Y ?? 0;
            address.ZoneRadius = ZoneRadius;
            address.RegionId = _RegionId;
            return address;
        }
    }

    public class AgentAddressValidator : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value == null)
            {
                return true;
            }
            else
            {
                int id = Convert.ToInt32(value);
                AgentAddressModel model = AgentAddressModel.GetMktgAddressByAgentId(id);
                if (model == null)
                    return false;
                else
                    return model.Validate();
            }
        }
    }
}