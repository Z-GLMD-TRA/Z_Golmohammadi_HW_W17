using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Z_Golmohammadi_HW_W17.Models;

namespace Z_Golmohammadi_HW_W17.Controllers
{

    public class StaffListController : Controller
    {
        List<Staff> staffsList = new List<Staff>();
        private List<Staff> GetStaffDataFromDatabase()
        {
            string conn = "Data Source=.;Initial Catalog=BikeStore;TrustServerCertificate=True;Integrated Security=SSPI";

            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                SqlCommand cmd = sqlConnection.CreateCommand();
                cmd.CommandText = @"SELECT sta.first_name, sta.last_name, sta.email, sta.phone, sta.store_id, sto.store_name, ssta.first_name + ' ' + ssta.last_name AS manager_name  ,sta.manager_id
                            FROM sales.staffs sta
                            LEFT JOIN sales.stores sto ON sta.store_id = sto.store_id
                            LEFT JOIN sales.staffs ssta ON sta.manager_id = ssta.staff_id";
                cmd.Connection = sqlConnection;

                try
                {
                    sqlConnection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var staff = new Staff();
                        staff.FirstName = reader["first_name"].ToString();
                        staff.LastName = reader["last_name"].ToString();
                        staff.Email = reader["email"].ToString();
                        staff.Phone = reader["phone"].ToString();
                        staff.StoreId = Convert.ToInt32(reader["store_id"]);
                        staff.StoreName = reader["store_name"].ToString();
                        staff.ManagerName = reader["manager_name"].ToString();
                        staff.ManagerId= reader["manager_id"].ToString();
                        staffsList.Add(staff);
                    }
                    sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    ViewBag.Errors = "An error occurred: " + ex.Message;
                }
            }
            return staffsList;
        }
        public IActionResult GetAll()
        {
            var allStaffs = GetStaffDataFromDatabase();
            var staffModel = new StaffViewModel { StaffsInfo = allStaffs };
            return View(staffModel);
        }

        public IActionResult FilteredData(StaffViewModel st, string managerId, string storeName)
        {
            List<Staff> filteredStaffs = GetStaffDataFromDatabase(); ;
            if (!string.IsNullOrEmpty(managerId))
            {
                filteredStaffs = filteredStaffs.Where(s => s.ManagerId == managerId).ToList();
            }

            if (!string.IsNullOrEmpty(storeName))
            {
                filteredStaffs = filteredStaffs.Where(s => s.StoreName.Contains(storeName)).ToList();
            }
            st.StaffsInfo = filteredStaffs;
            return View("GetAll", st);
        }
    }
}
