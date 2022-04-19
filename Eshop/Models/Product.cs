using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Eshop.Models
{
    public class Product
    {
        public int Id { get; set; }

        public string SKU { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:#,##0}")]
        public int Price { get; set; }

        public int Stock { get; set; }

        public int ProductTypeId { get; set; }

        public ProductType ProductType { get; set; }    // Navigation reference property

        public string Image { get; set; }

        public bool Status { get; set; }

        public List<Cart> Carts { get; set; }

        public List<InvoiceDetail> InvoiceDetails { get; set; }
    }
}
