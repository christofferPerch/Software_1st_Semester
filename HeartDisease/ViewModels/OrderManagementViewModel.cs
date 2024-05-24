namespace HeartDisease.ViewModels {
    public class OrderManagementViewModel {
        public IEnumerable<Models.Webshop.Order> Orders { get; set; }
        public Dictionary<int, List<Models.Webshop.OrderHistory>> OrderHistories { get; set; }
    }
}