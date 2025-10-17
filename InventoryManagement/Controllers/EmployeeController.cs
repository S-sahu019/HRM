using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace InventoryManagement.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly string _connectionString;

        public EmployeeController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConn");
        }

        // GET: Employee/Index
        public IActionResult Index()
        {
            List<Employee> employees = new();

            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GetAllEmployee_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {
                            employees.Add(new Employee
                            {
                                EmployeeID = dr.GetInt32(0),
                                Name = dr.GetString(1),
                                Role = dr.GetString(2),
                                Department = dr.GetString(3),
                                DateOfJoining = dr.GetDateTime(4)
                            });
                        }
                    }
                }
            }

            return View(employees);
        }

        // POST: Employee/Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Employee model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_InsertEmployee_SS", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@Name", model.Name);
                        cmd.Parameters.AddWithValue("@Role", model.Role);
                        cmd.Parameters.AddWithValue("@Department", model.Department);
                        cmd.Parameters.AddWithValue("@DateOfJoining", model.DateOfJoining);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // POST: Employee/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Employee model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("sp_UpdateEmployee_SS", con))
                    {
                        cmd.CommandType = System.Data.CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@EmployeeID", model.EmployeeID);
                        cmd.Parameters.AddWithValue("@Name", model.Name);
                        cmd.Parameters.AddWithValue("@Role", model.Role);
                        cmd.Parameters.AddWithValue("@Department", model.Department);
                        cmd.Parameters.AddWithValue("@DateOfJoining", model.DateOfJoining);

                        con.Open();
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // DELETE: Employee/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteEmployee_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@EmployeeID", id);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Employee deleted successfully.";
                    }
                    catch (SqlException ex)
                    {
                        TempData["Error"] = ex.Message; // shows "Cannot delete employee. Active assignments exist."
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}
