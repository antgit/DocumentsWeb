using System;
using System.ComponentModel.DataAnnotations;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Taxes.Models
{
    public class DocumentDetailTaxModel:DocumentDetailBaseModel
    {
        /// <summary>Идентификатор продукта</summary>
        [Required(ErrorMessage = "Выберите товар")]
        public int ProductId { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName { get; set; }
        /// <summary>Количество</summary>
        public decimal Qty { get; set; }
        /// <summary>Цена</summary>
        public decimal Price { get; set; }
        /// <summary>Сумма</summary>
        [NotZero(ErrorMessage = "Сумма не должна быть 0")]
        public decimal Summa { get; set; }
        /// <summary>Примечание</summary>
        public string Memo { get; set; }

        public DocumentDetailTax ToObject(Workarea workarea, DocumentTaxes owner)
        {
            DocumentDetailTax detail = new DocumentDetailTax
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
                                                    Memo = Memo,
                                                    UnitId = ProductId == 0 ? 0 : WADataProvider.WA.Cashe.GetCasheData<Product>().Item(ProductId).UnitId
                                                    //Date=DateTime.Now, DateModified=DateTime.Now
                                                };
            return detail;
        }

        public static DocumentDetailTaxModel ConvertToModel(DocumentDetailTax value)
        {
            DocumentDetailTaxModel obj = new DocumentDetailTaxModel
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
                                                  Memo = value.Memo
                                              };

            return obj;
        }
    }
}