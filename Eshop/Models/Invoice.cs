using System.ComponentModel.DataAnnotations;

namespace Eshop.Models
{
    public class Invoice
    {
        public int Id { get; set; }

        public string Code { get; set; }

        public int AccountId { get; set; }

        public Account Account { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy - HH:mm:ss}")]
        public DateTime IssuedDate { get; set; }

        public string ShippingAddress { get; set; }

        public string ShippingPhone { get; set; }

        public int Total { get; set; }

        public bool Status { get; set; }

        public List<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
