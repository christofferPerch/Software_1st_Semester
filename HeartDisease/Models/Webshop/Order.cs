namespace HeartDisease.Models.Webshop {
    public class Order {
        public int OrderID { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
