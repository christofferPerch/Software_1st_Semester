namespace HeartDisease.Models.Webshop {
    public class OrderHistory {
        public int EntryID { get; set; }
        public int OrderID { get; set; }
        public DateTime DateModified { get; set; }
        public bool Status { get; set; }
        public string UserId { get; set; }

    }
}
