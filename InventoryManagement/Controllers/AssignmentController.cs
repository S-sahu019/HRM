using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace InventoryManagement.Controllers
{
    public class AssignmentController : Controller
    {
        private readonly string _connectionString;

        public AssignmentController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConn");
        }

        // GET: /Assignment
        public IActionResult Index()
        {
            var assignments = new List<Assignment>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllAssignments_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            assignments.Add(new Assignment
                            {
                                AssignmentID = dr.GetInt32(0),
                                EmployeeID = dr.GetInt32(1),
                                EmployeeName = dr.GetString(2),
                                InventoryId = dr.GetInt32(3),
                                InventoryType = dr.GetString(4),
                                Brand = dr.GetString(5),
                                AssignedOn = dr.GetDateTime(6),
                                UnassignedOn = dr.IsDBNull(7) ? null : dr.GetDateTime(7)
                            });
                        }
                    }
                }
            }

            // Fetch Employee list
            var employees = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT EmployeeID, Name FROM Employee_SS", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            employees.Add(new SelectListItem
                            {
                                Value = dr["EmployeeID"].ToString(),
                                Text = $"{dr["Name"]} ({dr["EmployeeID"]})"
                            });
                        }
                    }
                }
            }

            // Fetch Inventory list
            var inventories = new List<SelectListItem>();
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT InventoryId, Brand, InventoryType FROM Inventory_SS WHERE InventoryId NOT IN (SELECT InventoryId FROM Assigned_SS WHERE UnassignedOn IS NULL)", con))
                {
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            inventories.Add(new SelectListItem
                            {
                                Value = dr["InventoryId"].ToString(),
                                Text = $"{dr["Brand"]} - {dr["InventoryType"]} ({dr["InventoryId"]})"
                            });
                        }
                    }
                }
            }

            ViewBag.Employees = employees;
            ViewBag.Inventories = inventories;

            return View(assignments);
        }




        [HttpPost]
        public IActionResult Add(int employeeId, int inventoryId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_InsertAssignment_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeID", employeeId);
                    cmd.Parameters.AddWithValue("@InventoryId", inventoryId);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Assignment created successfully.";
                    }
                    catch (SqlException ex)
                    {
                        TempData["Error"] = ex.Message; // "Inventory already assigned."
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // POST: Assignment/Unassign
        [HttpPost]
        public IActionResult Unassign(int assignmentId)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_UnassignInventory_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@AssignmentID", assignmentId);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Inventory unassigned successfully.";
                    }
                    catch (SqlException ex)
                    {
                        TempData["Error"] = ex.Message;
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}
