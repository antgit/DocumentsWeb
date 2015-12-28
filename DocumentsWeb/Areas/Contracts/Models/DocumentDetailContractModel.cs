using System.ComponentModel.DataAnnotations;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Contracts.Models
{
    public class DocumentDetailContractModel: DocumentDetailBaseModel
    {
        private int _productId;
        /// <summary>Идентификатор продукта</summary>
        [Required(ErrorMessage = "Поле обязательно для заполния")]
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
                        INN = p.Nomenclature;
                    }
                    _productId = value;
                }
            }
        }
        /// <summary>Инвентарный номер</summary>
        public string INN { get; set; }
        /// <summary>Конфигурация</summary>
        public string Config { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName { get; set; }
        /// <summary>Количество</summary>
        [Required(ErrorMessage = "Поле обязательно для заполния")]
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
        [Required(ErrorMessage = "Поле обязательно для заполния")]
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
        /// <summary>
        /// Наименование аналитики
        /// </summary>
        public string AnaliticName { get; set; }

        /// <summary>
        /// Строка №2
        /// </summary>
        public string StringValue2 { get; set; }


        /// <summary>Примечание</summary>
        public string DMemo { get; set; }

        public DocumentDetailContract ToObject(DocumentContract owner)
        {
            DocumentDetailContract detailContract = new DocumentDetailContract
            {
                Workarea = WADataProvider.WA,
                Document = owner,
                Id = Id,
                StateId = StateId,
                Guid=Guid,
                ProductId = ProductId,
                Qty = Qty,
                Price = Price,
                Summa = Summa,
                Memo = DMemo,
                AnaliticId = AnaliticId,
                StringValue2 = StringValue2,
                OwnerId = owner.Id,
                UnitId = ProductId == 0 ? 0 : WADataProvider.WA.Cashe.GetCasheData<Product>().Item(ProductId).UnitId
            };
            //detailContract.Product.Memo = Config;

            return detailContract;
        }

        public static DocumentDetailContractModel ConvertToModel(DocumentDetailContract value)
        {
            DocumentDetailContractModel obj = new DocumentDetailContractModel
            {
                Id = value.Id,
                Guid=value.Guid,
                StateId = value.StateId,
                OwnerId = value.OwnerId,
                ProductId = value.ProductId,
                ProductName = value.Product.Name,
                Qty = value.Qty,
                Price = value.Price,
                Summa = value.Summa,
                DMemo = value.Memo,
                INN = value.Product != null ? value.Product.Nomenclature : "",
                Config = value.Product != null ? value.Product.Memo : "",
                AnaliticId = value.AnaliticId,
                AnaliticName = value.AnaliticId > 0 ? value.Analitic.Name : "",
                StringValue2 = value.StringValue2
            };

            return obj;
        }
    }
}