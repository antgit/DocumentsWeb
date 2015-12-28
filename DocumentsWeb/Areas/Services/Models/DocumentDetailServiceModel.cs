using System.ComponentModel.DataAnnotations;
using BusinessObjects;
using BusinessObjects.Documents;
using DocumentsWeb.Code;
using DocumentsWeb.Models;

namespace DocumentsWeb.Areas.Services.Models
{
    public class DocumentDetailServiceModel: DocumentDetailBaseModel
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

        public DocumentDetailService ToObject(Workarea workarea, DocumentService owner)
        {
            DocumentDetailService detailService = new DocumentDetailService
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
            };
            return detailService;
        }

        public static DocumentDetailServiceModel ConvertToModel(DocumentDetailService value)
        {
            DocumentDetailServiceModel obj = new DocumentDetailServiceModel
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

        ///// <summary>
        ///// Получение ИД документа по ИД детализации
        ///// </summary>
        ///// <param name="detailId">ИД детализации</param>
        ///// <returns>ИД документа</returns>
        //[Obsolete]
        //public static int GetDocumentIdByDetailId(int detailId)
        //{
        //    using (SqlConnection cnn = WADataProvider.WA.GetDatabaseConnection())
        //    {
        //        if (cnn.State != ConnectionState.Open)
        //            cnn.Open();

        //        using (SqlCommand cmd = cnn.CreateCommand())
        //        {
        //            cmd.CommandText = string.Format("SELECT s.OwnId FROM Sales.DocumentDetails s WHERE s.Id={0}", detailId);
        //            cmd.CommandType = CommandType.Text;
        //            return (int)cmd.ExecuteScalar();
        //        }
        //    }
        //    //return 0;
        //}

        //public void Update()
        //{
        //    DocumentSales documentSales=new DocumentSales{Workarea=WADataProvider.WA};
        //    documentSales.Load(OwnerId);
        //    DocumentDetailSale detail = documentSales.Details.FirstOrDefault(s => s.Id == Id);

        //    //new
        //    if(detail==null)
        //    {
        //        detail = new DocumentDetailSale {Workarea = WADataProvider.WA, OwnerId = OwnerId, Document = documentSales};
        //        documentSales.Details.Add(detail);
        //    }

        //    detail.ProductId = ProductId;
        //    detail.Qty = Qty;
        //    detail.Price = Price;
        //    detail.Summa = Summa;
        //    detail.Memo = Memo;

        //    documentSales.Save();
        //}

        //public void Delete()
        //{
        //    int ownId = GetDocumentIdByDetailId(Id);

        //    DocumentSales documentSales = new DocumentSales { Workarea = WADataProvider.WA };
        //    documentSales.Load(ownId);

        //    DocumentDetailSale detail = documentSales.Details.FirstOrDefault(s => s.Id == Id);
        //    detail.StateId = State.STATEDELETED;
        //    documentSales.Save();
        //}
    }
}