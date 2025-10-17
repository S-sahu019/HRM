using InventoryManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryManagement.Controllers
{
    public class InventoryController : Controller
    {
        private readonly string _connectionString;

        public InventoryController(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DbConn");
        }

        // Get all inventories using SP
        public IActionResult Index()
        {
            var inventories = new List<Inventory>();

            using (SqlConnection con = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("sp_GetAllInventory_SS", con))
            {
                cmd.CommandType = System.Data.CommandType.StoredProcedure;

                con.Open();
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        inventories.Add(new Inventory
                        {
                            InventoryID = dr.GetInt32(dr.GetOrdinal("InventoryID")),
                            InventoryName = dr.GetString(dr.GetOrdinal("InventoryName")),
                            InventoryType = dr.GetString(dr.GetOrdinal("InventoryType")),
                            Brand = dr.IsDBNull(dr.GetOrdinal("Brand")) ? null : dr.GetString(dr.GetOrdinal("Brand")),
                            CurrentStatus = dr.GetString(dr.GetOrdinal("CurrentStatus"))
                        });
                    }
                }
            }
            return View(inventories);
        }

        //  Insert inventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Inventory model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_InsertInventory_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InventoryName", model.InventoryName);
                    cmd.Parameters.AddWithValue("@InventoryType", model.InventoryType);
                    cmd.Parameters.AddWithValue("@Brand", string.IsNullOrEmpty(model.Brand) ? DBNull.Value : model.Brand);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        //  Edit inventory
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Inventory model)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection con = new SqlConnection(_connectionString))
                using (SqlCommand cmd = new SqlCommand("sp_UpdateInventory_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InventoryID", model.InventoryID);
                    cmd.Parameters.AddWithValue("@InventoryName", model.InventoryName);
                    cmd.Parameters.AddWithValue("@InventoryType", model.InventoryType);
                    cmd.Parameters.AddWithValue("@Brand", string.IsNullOrEmpty(model.Brand) ? DBNull.Value : model.Brand);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
            return RedirectToAction("Index");
        }

        // DELETE: Inventory/Delete/5
        [HttpPost]
        public IActionResult Delete(int id)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_DeleteInventory_SS", con))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@InventoryId", id);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        TempData["Message"] = "Inventory deleted successfully.";
                    }
                    catch (SqlException ex)
                    {
                        TempData["Error"] = ex.Message; // shows "Cannot delete inventory. Active assignments exist."
                    }
                }
            }
            return RedirectToAction("Index");
        }

    }
}

