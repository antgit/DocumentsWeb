using System.ComponentModel.DataAnnotations;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Planing.Models
{
    public class DocumentDetailPlaningModel : DocumentDetailBaseModel
    {
        private int _productId;
        /// <summary>Идентификатор продукта</summary>
        //[Required(ErrorMessage = "Поле обязательно для заполния")]
        public int ProductId
        {
            get
            {
                return _productId;
            }

            set
            {
                if (value > 0)
                {
                    Product p = (Product)WADataProvider.WA.Cashe.GetCasheData<Product>().Item(value);
                    if (p != null)
                    {
                        ProductNom = p.Nomenclature;
                    }
                    _productId = value;
                }
            }
        }

        private int _productId2;
        /// <summary>Идентификатор продукта №2</summary>
        public int ProductId2
        {
            get
            {
                return _productId2;
            }

            set
            {
                if (value > 0)
                {
                    Product p = (Product)WADataProvider.WA.Cashe.GetCasheData<Product>().Item(value);
                    if (p != null)
                    {
                        ProductNom2 = p.Nomenclature;
                    }
                    _productId2 = value;
                }
            }
        }

        private int _productId3;
        /// <summary>Идентификатор продукта №3</summary>
        public int ProductId3
        {
            get
            {
                return _productId3;
            }

            set
            {
                if (value > 0)
                {
                    Product p = (Product)WADataProvider.WA.Cashe.GetCasheData<Product>().Item(value);
                    if (p != null)
                    {
                        ProductNom3 = p.Nomenclature;
                    }
                    _productId3 = value;
                }
            }
        }

        private int _productId4;
        /// <summary>Идентификатор продукта №4</summary>
        public int ProductId4
        {
            get
            {
                return _productId4;
            }

            set
            {
                if (value > 0)
                {
                    Product p = (Product)WADataProvider.WA.Cashe.GetCasheData<Product>().Item(value);
                    if (p != null)
                    {
                        ProductNom4 = p.Nomenclature;
                    }
                    _productId4 = value;
                }
            }
        }
        private int _productId5;
        /// <summary>Идентификатор продукта №5</summary>
        public int ProductId5
        {
            get
            {
                return _productId5;
            }

            set
            {
                if (value > 0)
                {
                    Product p = (Product)WADataProvider.WA.Cashe.GetCasheData<Product>().Item(value);
                    if (p != null)
                    {
                        ProductNom5 = p.Nomenclature;
                    }
                    _productId5 = value;
                }
            }
        }
        /// <summary>Инвентарный номер</summary>
        public string ProductNom { get; set; }
        /// <summary>Инвентарный номер</summary>
        public string ProductNom2 { get; set; }
        /// <summary>Инвентарный номер</summary>
        public string ProductNom3 { get; set; }
        /// <summary>Инвентарный номер</summary>
        public string ProductNom4 { get; set; }
        /// <summary>Инвентарный номер</summary>
        public string ProductNom5 { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName2 { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName3 { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName4 { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName5 { get; set; }
        /// <summary>Количество</summary>
        //[Required(ErrorMessage = "Поле обязательно для заполния")]
        public decimal Qty { get; set; }
        /// <summary>Цена</summary>
        public decimal Price { get; set; }
        /// <summary>Сумма</summary>
        //[NotZero(ErrorMessage = "Сумма не должна быть 0")]
        public decimal Summa { get; set; }

        private int _analiticId;
        /// <summary>
        /// Идентификатор аналитики
        /// </summary>
        //[Required(ErrorMessage = "Поле обязательно для заполния")]
        public int AnaliticId
        {
            get
            {
                return _analiticId;
            }

            set
            {
                if (value > 0)
                {
                    Analitic a = (Analitic)WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(value);
                    if (a != null)
                    {
                        AnaliticName = a.Name;
                    }
                    _analiticId = value;
                }
            }
        }

        private int _analiticId2;
        /// <summary>Идентификатор аналитики №2</summary>
        public int AnaliticId2
        {
            get
            {
                return _analiticId2;
            }

            set
            {
                if (value > 0)
                {
                    Analitic a = (Analitic)WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(value);
                    if (a != null)
                    {
                        Analitic2Name = a.Name;
                    }
                    _analiticId2 = value;
                }
            }
        }

        private int _analiticId3;
        /// <summary>Идентификатор аналитики №3</summary>
        public int AnaliticId3
        {
            get
            {
                return _analiticId3;
            }

            set
            {
                if (value > 0)
                {
                    Analitic a = (Analitic)WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(value);
                    if (a != null)
                    {
                        Analitic3Name = a.Name;
                    }
                    _analiticId3 = value;
                }
            }
        }

        private int _analiticId4;
        /// <summary>Идентификатор аналитики №4</summary>
        public int AnaliticId4
        {
            get
            {
                return _analiticId4;
            }

            set
            {
                if (value > 0)
                {
                    Analitic a = (Analitic)WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(value);
                    if (a != null)
                    {
                        Analitic4Name = a.Name;
                    }
                    _analiticId4 = value;
                }
            }
        }

        private int _analiticId5;
        /// <summary>Идентификатор аналитики №5</summary>
        public int AnaliticId5
        {
            get
            {
                return _analiticId5;
            }

            set
            {
                if (value > 0)
                {
                    Analitic a = (Analitic)WADataProvider.WA.Cashe.GetCasheData<Analitic>().Item(value);
                    if (a != null)
                    {
                        Analitic5Name = a.Name;
                    }
                    _analiticId5 = value;
                }
            }
        }

        /// <summary>Наименование аналитики</summary>
        public string AnaliticName { get; set; }
        /// <summary>Наименование аналитики №2</summary>
        public string Analitic2Name { get; set; }
        /// <summary>Наименование аналитики №3</summary>
        public string Analitic3Name { get; set; }
        /// <summary>Наименование аналитики №4</summary>
        public string Analitic4Name { get; set; }
        /// <summary>Наименование аналитики №5</summary>
        public string Analitic5Name { get; set; }

        /// <summary>Строка №1</summary>
        public string StringValue1 { get; set; }
        /// <summary>Строка №2</summary>
        public string StringValue2 { get; set; }
        /// <summary>Строка №3</summary>
        public string StringValue3 { get; set; }
        /// <summary>Строка №4</summary>
        public string StringValue4 { get; set; }
        /// <summary>Строка №5</summary>
        public string StringValue5 { get; set; }


        /// <summary>Наименование корреспондента №1</summary>
        public string AgentName { get; set; }
        /// <summary>Наименование корреспондента №2</summary>
        public string AgentName2 { get; set; }
        /// <summary>Наименование корреспондента №3</summary>
        public string AgentName3 { get; set; }
        /// <summary>Наименование корреспондента №4</summary>
        public string AgentName4 { get; set; }
        /// <summary>Наименование корреспондента №5</summary>
        public string AgentName5 { get; set; }

        private int _agentId;
        /// <summary>Идентификатор корреспондента №</summary>
        public int AgentId
        {
            get
            {
                return _agentId;
            }

            set
            {
                if (value > 0)
                {
                    Agent p = (Agent)WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    if (p != null)
                    {
                        AgentName = p.Name;
                    }
                    _agentId = value;
                }
            }
        }

        private int _agentId2;
        /// <summary>Идентификатор корреспондента №</summary>
        public int AgentId2
        {
            get
            {
                return _agentId2;
            }

            set
            {
                if (value > 0)
                {
                    Agent p = (Agent)WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    if (p != null)
                    {
                        AgentName2 = p.Name;
                    }
                    _agentId2 = value;
                }
            }
        }
        private int _agentId3;
        /// <summary>Идентификатор корреспондента № 3</summary>
        public int AgentId3
        {
            get
            {
                return _agentId3;
            }

            set
            {
                if (value > 0)
                {
                    Agent p = (Agent)WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    if (p != null)
                    {
                        AgentName3 = p.Name;
                    }
                    _agentId3 = value;
                }
            }
        }

        private int _agentId4;
        /// <summary>Идентификатор корреспондента № 4</summary>
        public int AgentId4
        {
            get
            {
                return _agentId4;
            }

            set
            {
                if (value > 0)
                {
                    Agent p = (Agent)WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    if (p != null)
                    {
                        AgentName4 = p.Name;
                    }
                    _agentId4 = value;
                }
            }
        }

        private int _agentId5;
        /// <summary>Идентификатор корреспондента № 5</summary>
        public int AgentId5
        {
            get
            {
                return _agentId5;
            }

            set
            {
                if (value > 0)
                {
                    Agent p = (Agent)WADataProvider.WA.Cashe.GetCasheData<Agent>().Item(value);
                    if (p != null)
                    {
                        AgentName5 = p.Name;
                    }
                    _agentId5 = value;
                }
            }
        }

        /// <summary>Примечание</summary>
        public string DMemo { get; set; }

        public DocumentDetailPlan ToObject(DocumentPlan owner)
        {
            DocumentDetailPlan detailContract = new DocumentDetailPlan
                                                        {
                                                            Workarea = WADataProvider.WA,
                                                            Document = owner,
                                                            Id = Id,
                                                            StateId = StateId,
                                                            Guid = Guid,
                                                            ProductId = ProductId,
                                                            Qty = Qty,
                                                            Price = Price,
                                                            Summa = Summa,
                                                            Memo = DMemo,
                                                            AnaliticId = AnaliticId,
                                                            StringValue2 = StringValue2,
                                                            OwnerId = owner.Id
                                                        };
            //detailContract.Product.Memo = Config;

            return detailContract;
        }

        public static DocumentDetailPlaningModel ConvertToModel(DocumentDetailPlan value)
        {
            DocumentDetailPlaningModel obj = new DocumentDetailPlaningModel
                                                  {
                                                      Id = value.Id,
                                                      Guid = value.Guid,
                                                      StateId = value.StateId,
                                                      OwnerId = value.OwnerId,
                                                      
                                                      ProductId = value.ProductId,
                                                      ProductName = value.ProductId!=0? value.Product.Name: string.Empty,
                                                      ProductNom = value.Product != null ? value.Product.Nomenclature : string.Empty,
                                                      
                                                      ProductId2 = value.ProductId2,
                                                      ProductName2 = value.ProductId2 != 0 ? value.Product2.Name : string.Empty,
                                                      ProductNom2 = value.Product3 != null ? value.Product2.Nomenclature : string.Empty,
                                                      
                                                      ProductId3 = value.ProductId3,
                                                      ProductName3 = value.ProductId3 != 0 ? value.Product3.Name : string.Empty,
                                                      ProductNom3 = value.Product3 != null ? value.Product3.Nomenclature : string.Empty,
                                                      
                                                      ProductId4 = value.ProductId4,
                                                      ProductName4 = value.ProductId4 != 0 ? value.Product4.Name : string.Empty,
                                                      ProductNom4 = value.Product4 != null ? value.Product4.Nomenclature : string.Empty,
                                                     
                                                      ProductId5 = value.ProductId5,
                                                      ProductName5 = value.ProductId5 != 0 ? value.Product5.Name : string.Empty,
                                                      ProductNom5 = value.Product5 != null ? value.Product5.Nomenclature : string.Empty,

                                                      AgentId = value.AgentId,
                                                      AgentName = value.AgentId != 0 ? value.Agent.Name : string.Empty,

                                                      AgentId2 = value.AgentId2,
                                                      AgentName2 = value.AgentId2 != 0 ? value.Agent2.Name : string.Empty,

                                                      AgentId3 = value.AgentId3,
                                                      AgentName3 = value.AgentId3 != 0 ? value.Agent3.Name : string.Empty,

                                                      AgentId4 = value.AgentId4,
                                                      AgentName4 = value.AgentId4 != 0 ? value.Agent4.Name : string.Empty,

                                                      AgentId5 = value.AgentId5,
                                                      AgentName5 = value.AgentId5 != 0 ? value.Agent5.Name : string.Empty,

                                                      Qty = value.Qty,
                                                      Price = value.Price,
                                                      Summa = value.Summa,
                                                      DMemo = value.Memo,

                                                      AnaliticId = value.AnaliticId,
                                                      AnaliticName = value.AnaliticId > 0 ? value.Analitic.Name : string.Empty,
                                                      StringValue1 = value.StringValue1,
                                                      StringValue2 = value.StringValue2,
                                                      StringValue3 = value.StringValue3,
                                                      StringValue4 = value.StringValue4,
                                                      StringValue5 = value.StringValue5

                                                  };

            return obj;
        }
    }
}