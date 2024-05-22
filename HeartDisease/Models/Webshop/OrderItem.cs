namespace HeartDisease.Models.Webshop {
    public class OrderItem {
        public int OrderItemID { get; set; }
        public int OrderID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        public decimal PriceAtTimeOrder { get; set; }


    }
}
