namespace Eshop.Models
{
    public class ProductType
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public bool Status { get; set; }

        public List<Product> Products { get; set; }     // Collection reference property
    }
}
