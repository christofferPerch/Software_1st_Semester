using System.Collections.Generic;
using HeartDisease.Models.Webshop;

namespace HeartDisease.ViewModels
{
    public class ProductDetailsViewModel
    {
        public Product Product { get; set; }
        public string ManufacturerName { get; set; }
        public string SideEffects { get; set; }
        public Review Review { get; set; }
    }
}
