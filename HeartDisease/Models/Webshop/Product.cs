namespace HeartDisease.Models.Webshop {
    public class Product {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public string GenericName { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public int ManufacturerID { get; set; }
        public decimal Price { get; set; }


    }
}
