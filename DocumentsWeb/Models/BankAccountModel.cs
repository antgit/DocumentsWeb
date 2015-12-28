namespace DocumentsWeb.Models
{
    public class BankAccountModel
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Номер счета
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// Примечание
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// Идентификатор валюты
        /// </summary>
        public int CurrencyId { get; set; }
        /// <summary>
        /// Цифровой код валюты
        /// </summary>
        public int CurrencyIntCode { get; set; }
        /// <summary>
        /// Строковый код валюты
        /// </summary>
        public string CurrencyCode { get; set; }
    }
}