using System.ComponentModel.DataAnnotations;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Prices.Models
{
    public class DocumentDetailPriceListModel: DocumentDetailBaseModel
    {
        /// <summary>Идентификатор продукта</summary>
        [Required(ErrorMessage = "Выберите товар")]
        public int ProductId { get; set; }
        /// <summary>Наименование продукта</summary>
        public string ProductName { get; set; }
        /// <summary>Цена</summary>
        public decimal Price { get; set; }
        /// <summary>Примечание</summary>
        public string DetailMemo { get; set; }

        public DocumentDetailPrice ToObject(Workarea workarea, DocumentPrices owner)
        {
            DocumentDetailPrice detailPrice = new DocumentDetailPrice
            {
                Workarea = WADataProvider.WA,
                Document = owner,
                Id = Id,
                StateId = StateId,
                Guid=Guid,
                ProductId = ProductId,
                Memo = DetailMemo,
                Value = Price
            };
            return detailPrice;
        }

        public static DocumentDetailPriceListModel ConvertToModel(DocumentDetailPrice value)
        {
            DocumentDetailPriceListModel obj = new DocumentDetailPriceListModel
            {
                Id = value.Id,
                Guid=value.Guid,
                StateId = value.StateId,
                OwnerId = value.OwnerId,
                ProductId = value.ProductId,
                ProductName = value.Product.Name,
                Price = value.Value,
                DetailMemo = value.Memo
            };

            return obj;
        }
    }
}