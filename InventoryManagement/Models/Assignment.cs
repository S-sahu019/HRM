
namespace InventoryManagement.Models
{
    public class Assignment
    {
        public int AssignmentID { get; set; }
        public int EmployeeID { get; set; }
        public string EmployeeName { get; set; }   
        public int InventoryId { get; set; }
        public string InventoryType { get; set; }  
        public string Brand { get; set; } 
        public DateTime AssignedOn { get; set; }
        public DateTime? UnassignedOn { get; set; }
    }
}
