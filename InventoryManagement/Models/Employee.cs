﻿namespace InventoryManagement.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
        public DateTime DateOfJoining { get; set; }
    }
}
