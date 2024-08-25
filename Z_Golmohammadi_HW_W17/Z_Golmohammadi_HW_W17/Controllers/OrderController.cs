using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;
using Z_Golmohammadi_HW_W17.Models;

namespace Z_Golmohammadi_HW_W17.Controllers
{
    public class OrderController : Controller
    {
        private List<Order> GetOrderDataFromDatabase(int orderId)
        {
            List<Order> ordersList = new List<Order>();

            string conn = "Data Source=.;Initial Catalog=BikeStore;TrustServerCertificate=True;Integrated Security=SSPI";

            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                SqlCommand cmd = sqlConnection.CreateCommand();
                cmd.CommandText = @$"SELECT so.[order_id],sc.[first_name],sc.[last_name],sc.[street]+', '+sc.[city]+', '+sc.[state] AS [customer_address],sc.[phone],so.order_date,so.required_date,so.shipped_date,ss.first_name + ' '+ss.last_name AS [staff_name] 
                                    FROM sales.orders so
                                    LEFT JOIN sales.customers sc ON so.customer_id=sc.customer_id
                                    LEFT JOIN sales.staffs ss ON so.staff_id=ss.staff_id 
                                    where so.order_id={orderId}";
                cmd.Connection = sqlConnection;

                try
                {
                    sqlConnection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var order = new Order();
                        order.OrderId = Convert.ToInt32(reader["order_id"]);
                        order.CustomerFirstName = reader["first_name"].ToString();
                        order.CustomerLastName = reader["last_name"].ToString();
                        order.CustomerAddress = reader["customer_address"].ToString();
                        order.CustomerPhone = reader["phone"].ToString();
                        order.OrderDate = Convert.ToDateTime(reader["order_date"]);
                        order.RequiredDate = Convert.ToDateTime(reader["required_date"]);
                        order.ShippedDate = Convert.ToDateTime(reader["shipped_date"]);
                        order.StaffName = reader["staff_name"].ToString();
                        ordersList.Add(order);
                    }
                    sqlConnection.Close();
                }
                catch (Exception ex)
                {
                    ViewBag.Errors = "An error occurred: " + ex.Message;
                }
            }
            return ordersList;
        }

        public IActionResult Index()
        {
            return View("OrderList", new OrderProductViewModel());
        }
        public IActionResult GetFilteredOrdersList(int orderId)
        {
            var viewModel = new OrderProductViewModel
            {
                OrderInfo = GetOrderDataFromDatabase(orderId)
            };
            return View("OrderList", viewModel);
        }

        public IActionResult MoreInfoOrderData(int id)
        {
            var viewModel = new OrderProductViewModel
            {
                OrderInfo = GetOrderDataFromDatabase(id),
                SelectedOrder = GetOrderDetail(id),
                ProductInfo = GetProductDataForOrder(id)
            };
            var totalDis = viewModel.ProductInfo.Sum(p => p.Price*p.Discount);
            var totalprice = viewModel.ProductInfo.Sum(p => p.Price);
            viewModel.TotalPayment = totalprice-totalDis;

            return View("OrderList", viewModel);

        }
        private Order GetOrderDetail(int orderId)
        {
            return GetOrderDataFromDatabase(orderId).FirstOrDefault();
        }

        private List<Product> GetProductDataForOrder(int orderId)
        {
            List<Product> productsList = new List<Product>();
            string conn = "Data Source=.;Initial Catalog=BikeStore;TrustServerCertificate=True;Integrated Security=SSPI";

            using (SqlConnection sqlConnection = new SqlConnection(conn))
            {
                SqlCommand cmd = sqlConnection.CreateCommand();
                cmd.CommandText = @"SELECT p.product_id, p.product_name, oi.quantity, oi.list_price, oi.discount,
                            (oi.list_price * (1 - oi.discount)) as payment
                            FROM sales.order_items oi
                            JOIN production.products p ON oi.product_id = p.product_id
                            WHERE oi.order_id = @OrderId";
                cmd.Parameters.AddWithValue("@OrderId", orderId);

                try
                {
                    sqlConnection.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            productsList.Add(new Product
                            {
                                ProductId = Convert.ToInt32(reader["product_id"]),
                                ProductName = reader["product_name"].ToString(),
                                Quantity = Convert.ToInt32(reader["quantity"]),
                                Price = Convert.ToDecimal(reader["list_price"]),
                                Discount = Convert.ToDecimal(reader["discount"]),
                                Payment = Convert.ToDecimal(reader["payment"]),
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                }
            }
            return productsList;
        }
    }
}
