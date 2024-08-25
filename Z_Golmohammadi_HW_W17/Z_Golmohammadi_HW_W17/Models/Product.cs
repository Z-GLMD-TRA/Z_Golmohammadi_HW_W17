namespace Z_Golmohammadi_HW_W17.Models
{
    public class Product
    {
        public int ProductId {  get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
        public decimal Payment { get; set; }
    }
}
