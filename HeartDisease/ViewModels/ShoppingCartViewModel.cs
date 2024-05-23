namespace HeartDisease.ViewModels {
    public class ShoppingCartViewModel {
        public int OrderID { get; set; }
        public List<ShoppingCartItem> Items { get; set; } = new List<ShoppingCartItem>();
        public decimal Total { get; set; }
    }

    public class ShoppingCartItem {
        public int OrderItemID { get; set; }
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string ImageURL { get; set; }
    }
}
