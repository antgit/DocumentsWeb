namespace DocumentsWeb.Models
{
    public class BankAccountModel
    {
        /// <summary>
        /// �������������
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// ������������
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// ����� �����
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        public string Memo { get; set; }
        /// <summary>
        /// ������������� ������
        /// </summary>
        public int CurrencyId { get; set; }
        /// <summary>
        /// �������� ��� ������
        /// </summary>
        public int CurrencyIntCode { get; set; }
        /// <summary>
        /// ��������� ��� ������
        /// </summary>
        public string CurrencyCode { get; set; }
    }
}