using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public OrdersController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string _include, [FromQuery] string completed)
        {
            string SqlCommandText = @"
                        SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId                
                        FROM [Order] o";

            if (_include == "products")
            {
                SqlCommandText = @"
                    SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId,
                    op.Id as OrderProductId, op.OrderId as OPOrderId, op.ProductId as OPProductId,
                    p.Id as ProductId, p.[Title] as ProductTitle, p.Description, p.Price, p.Quantity, p.CustomerId, p.ProductTypeId
                    FROM [Order] o
                    LEFT JOIN OrderProduct op on o.Id = op.OrderId
                    LEFT JOIN Product p ON op.ProductId = p.Id";
            }
            else if (_include == "customers")
            {
                SqlCommandText = @"
                     SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId,
                     c.Id as CustomerId, c.FirstName, c.LastName
                     FROM [Order] o
                     LEFT JOIN Customer c ON c.Id = o.CustomerId";
            }
            else if (completed == "true")
            {
                SqlCommandText = @"
                    SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId,
                    op.Id as OrderProductId, op.OrderId as OPOrderId, op.ProductId as OPProductId,
                    p.Id as ProductId, p.[Title] as ProductTitle, p.Description, p.Price, p.Quantity, p.CustomerId as ProductCustomerId, p.ProductTypeId,
                    c.Id as RealCustomerId, c.FirstName, c.LastName
                    FROM [Order] o
                    LEFT JOIN OrderProduct op on o.Id = op.OrderId
                    LEFT JOIN Product p ON op.ProductId = p.Id
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    WHERE o.PaymentTypeId IS NOT NULL";
            }
            else if (completed == "false")
            {
                SqlCommandText = @"
                    SELECT o.Id as OrderId, o.CustomerId, o.PaymentTypeId,
                    op.Id as OrderProductId, op.OrderId as OPOrderId, op.ProductId as OPProductId,
                    p.Id as ProductId, p.[Title] as ProductTitle, p.Description, p.Price, p.Quantity, p.CustomerId as ProductCustomerId, p.ProductTypeId,
                    c.Id as RealCustomerId, c.FirstName, c.LastName
                    FROM [Order] o
                    LEFT JOIN OrderProduct op on o.Id = op.OrderId
                    LEFT JOIN Product p ON op.ProductId = p.Id
                    LEFT JOIN Customer c ON o.CustomerId = c.Id
                    WHERE o.PaymentTypeId IS NULL";
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SqlCommandText;

                    SqlDataReader reader = cmd.ExecuteReader();

                    List<Order> orders = new List<Order>();

                    while (reader.Read())
                    {
                        Order order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("OrderId")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            Completed = true,
                            Products = new List<Product>(),
                            Customers = new List<Customer>()
                            
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                        {
                            order.PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"));
                        }
                        else
                        {
                            order.Completed = false;
                        };                       
                        //orders.Add(order);

                        if (_include == "products")
                        {
                            Product product = new Product();
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                product.Id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                                product.Title = reader.GetString(reader.GetOrdinal("ProductTitle"));
                                product.Description = reader.GetString(reader.GetOrdinal("Description"));
                                product.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                product.ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId"));
                                product.CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                                product.Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                            }
                            else
                            {
                                product = null;
                            };

                            if (orders.Any(o => o.Id == order.Id))
                            {
                                Order existingOrder = orders.Find(o => o.Id == order.Id);
                                existingOrder.Products.Add(product);
                            }
                            else
                            {
                                order.Products.Add(product);
                                orders.Add(order);
                            }
                        }
                        else if (_include == "customers")
                        {
                            Customer customer = new Customer();
                            if (!reader.IsDBNull(reader.GetOrdinal("CustomerId")))
                            {
                                customer.Id = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                                customer.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                customer.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                            }
                            else
                            {
                                customer = null;
                            };

                            if (orders.Any(o => o.Id == order.Id))
                            {
                                Order existingOrder = orders.Find(o => o.Id == order.Id);
                                existingOrder.Customers.Add(customer);
                            }
                            else
                            {
                                order.Customers.Add(customer);
                                orders.Add(order);
                            }
                        }
                        else if (completed == "true")
                        {
                            Product product = new Product();
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                product.Id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                                product.Title = reader.GetString(reader.GetOrdinal("ProductTitle"));
                                product.Description = reader.GetString(reader.GetOrdinal("Description"));
                                product.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                product.ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId"));
                                product.CustomerId = reader.GetInt32(reader.GetOrdinal("ProductCustomerId"));
                                product.Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                            }
                            else
                            {
                                product = null;
                            };

                            Customer customer = new Customer();
                            if (!reader.IsDBNull(reader.GetOrdinal("RealCustomerId")))
                            {
                                customer.Id = reader.GetInt32(reader.GetOrdinal("RealCustomerId"));
                                customer.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                customer.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                            }
                            else
                            {
                                customer = null;
                            };
                            order.Customers.Add(customer);

                            if (orders.Any(o => o.Id == order.Id))
                            {
                                Order existingOrder = orders.Find(o => o.Id == order.Id);
                                existingOrder.Products.Add(product);
                                //existingOrder.Customers.Add(customer);
                            }
                            else
                            {
                                order.Products.Add(product);
                                //order.Customers.Add(customer);
                                orders.Add(order);
                            }
                        }
                        else if (completed == "false")
                        {
                            Product product = new Product();
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                product.Id = reader.GetInt32(reader.GetOrdinal("ProductId"));
                                product.Title = reader.GetString(reader.GetOrdinal("ProductTitle"));
                                product.Description = reader.GetString(reader.GetOrdinal("Description"));
                                product.Price = reader.GetDecimal(reader.GetOrdinal("Price"));
                                product.ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId"));
                                product.CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));
                                product.Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"));
                            }
                            else
                            {
                                product = null;
                            };

                            Customer customer = new Customer();
                            if (!reader.IsDBNull(reader.GetOrdinal("RealCustomerId")))
                            {
                                customer.Id = reader.GetInt32(reader.GetOrdinal("RealCustomerId"));
                                customer.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                customer.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                            }
                            else
                            {
                                customer = null;
                            };
                            order.Customers.Add(customer);

                            if (orders.Any(o => o.Id == order.Id))
                            {
                                Order existingOrder = orders.Find(o => o.Id == order.Id);
                                //existingOrder.Customers.Add(customer);
                                existingOrder.Products.Add(product);
                            }
                            else
                            {
                                //order.Customers.Add(customer);
                                order.Products.Add(product);
                                orders.Add(order);
                            }
                        }
                        else
                        {
                            if (orders.Any(o => o.Id == order.Id))
                            {
                                Order existingOrder = orders.Find(o => o.Id == order.Id);
                            }
                            else
                            {
                                orders.Add(order);
                            }
                        }
                    }
                    reader.Close();
                    return Ok(orders);
                }
            }
        }

        // GET api/orders/1
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT * FROM [Order]";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Order order = null;
                    if (reader.Read())
                    {
                        order = new Order
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                        };
                        if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                        {
                            order.PaymentTypeId = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"));
                        }
                    }
                    reader.Close();
                    return Ok(order);
                }
            }
        }

        // POST api/orders
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Order order)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO [Order] (CustomerId, PaymentTypeId)
                        OUTPUT INSERTED.Id
                        VALUES (@customerId, @paymentTypeId)
                    ";
                    cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                    cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));

                    order.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetOrder", new { id = order.Id }, order);
                }
            }
        }

        // PUT api/orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Order order)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Order
                            SET CustomerId = @customerId,
                                PaymentTypeId = @paymentTypeId
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", order.Id));
                        cmd.Parameters.Add(new SqlParameter("@customerId", order.CustomerId));
                        cmd.Parameters.Add(new SqlParameter("@paymentTypeId", order.PaymentTypeId));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Order WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool OrderExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Order WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}