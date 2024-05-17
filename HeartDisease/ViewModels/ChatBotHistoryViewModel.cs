using HeartDisease.Models;
using System.Collections.Generic;

namespace HeartDisease.ViewModels
{
    public class ChatBotHistoryViewModel
    {
        public List<ChatBotHistory> ChatHistories { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }

        public int TotalPages => (int)Math.Ceiling((decimal)TotalItems / PageSize);
    }
}
