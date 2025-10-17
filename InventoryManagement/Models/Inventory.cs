using System.ComponentModel.DataAnnotations;

namespace InventoryManagement.Models
{
    public class Inventory
    {
        public int InventoryID { get; set; }

        [Required]
        public string InventoryName { get; set; }

        [Required]
        public string InventoryType { get; set; }

        public string Brand { get; set; }

        public string CurrentStatus { get; set; } = "Available";
    }
}
