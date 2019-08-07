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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
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

        // GET api/customers
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string q, string _include)
        {

            string SqlCommandText = @"
                        SELECT c.Id as CustomerId, c.FirstName, c.LastName                 
                        FROM Customer c";

            if (_include == "products")
            {
                SqlCommandText = @"
                    SELECT c.Id as CustomerId, c.FirstName, c.LastName,
                    p.Id as ProductId, p.[Title] as ProductTitle, p.Description, p.Price, p.Quantity, p.CustomerId, p.ProductTypeId
                    FROM Customer c
                    LEFT JOIN Product p ON c.Id = p.CustomerId";
            }
            else if (_include == "payments")
            {
                SqlCommandText = @"
                     SELECT c.Id as CustomerId, c.FirstName, c.LastName,
                     p.Id as PaymentTypeId, p.[Name] as PaymentTypeName, p.AcctNumber, p.CustomerId
                     FROM Customer c
                     LEFT JOIN PaymentType p ON c.Id = p.CustomerId";
            }

            if (q != null)
            {
                SqlCommandText = $@"{SqlCommandText} WHERE (
                    c.FirstName LIKE @q
                    OR c.LastName LIKE @q
                    )
                    ";
            }


            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SqlCommandText;

                    if (q != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@q", $"%{q}%"));
                    }

                    SqlDataReader reader = cmd.ExecuteReader();
                    List<Customer> customers = new List<Customer>();

                    while (reader.Read())
                    {
                        Customer customer = new Customer
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            Products = new List<Product>(),
                            PaymentTypes = new List<PaymentType>()
                        };

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

                            if (customers.Any(c => c.Id == customer.Id))
                            {
                                Customer existingCustomer = customers.Find(c => c.Id == customer.Id);
                                existingCustomer.Products.Add(product);                          
                            }
                            else
                            {
                                customer.Products.Add(product);
                                customers.Add(customer);
                            }
                        }
                        else if (_include == "payments")
                        {
                            PaymentType paymentType = new PaymentType();
                            if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                            {
                                paymentType.Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId"));
                                paymentType.Name = reader.GetString(reader.GetOrdinal("PaymentTypeName"));
                                paymentType.AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber"));                    
                                paymentType.CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"));                    
                            }
                            else
                            {
                                paymentType = null;
                            };

                            if (customers.Any(c => c.Id == customer.Id))
                            {
                                Customer existingCustomer = customers.Find(c => c.Id == customer.Id);
                                existingCustomer.PaymentTypes.Add(paymentType);
                            }
                            else
                            {
                                customer.PaymentTypes.Add(paymentType);
                                customers.Add(customer);
                            }
                        }
                        else
                        {
                            if (customers.Any(c => c.Id == customer.Id))
                            {
                                Customer existingCustomer = customers.Find(c => c.Id == customer.Id);
                            }
                            else
                            {
                                customers.Add(customer);
                            }
                        }

                    }
                    reader.Close();

                    return Ok(customers);
                }
            }
        }

        // GET api/customers/5
        [HttpGet("{id}")]
        public IActionResult Get([FromRoute] int id, string _include)
        {
            string SqlCommandText;

            if (_include == "products")
            {
                SqlCommandText = @"
                    SELECT c.Id as CustomerId, c.FirstName, c.LastName,
                    p.Id as ProductId, p.[Title] as ProductTitle, p.Description, p.Price, p.Quantity, p.CustomerId, p.ProductTypeId
                    FROM Customer c
                    LEFT JOIN Product p ON c.Id = p.CustomerId";
            }
            else if (_include == "payments")
            {
                SqlCommandText = @"
                     SELECT c.Id as CustomerId, c.FirstName, c.LastName,
                     p.Id as PaymentTypeId, p.[Name] as PaymentTypeName, p.AcctNumber, p.CustomerId
                     FROM Customer c
                     LEFT JOIN PaymentType p ON c.Id = p.CustomerId";
            }
            else
            {
                 SqlCommandText = @"
                     SELECT c.Id as CustomerId, c.FirstName, c.LastName                 
                     FROM Customer c";
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = $"{SqlCommandText} WHERE c.id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = cmd.ExecuteReader();

                    Customer customer = null;

                    while (reader.Read())
                    {
                        if (customer == null)
                        {
                            customer = new Customer
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                Products = new List<Product>(),
                                PaymentTypes = new List<PaymentType>()
                            };
                        }

                        if (_include == "products")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("ProductId")))
                            {
                                customer.Products.Add(
                                    new Product
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                        Title = reader.GetString(reader.GetOrdinal("ProductTitle")),
                                        Description = reader.GetString(reader.GetOrdinal("Description")),
                                        Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                        ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                        Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                                    }
                                );
                            }
                        }
                        else if (_include == "payments")
                        {
                            if (!reader.IsDBNull(reader.GetOrdinal("PaymentTypeId")))
                            {
                                customer.PaymentTypes.Add(
                                    new PaymentType
                                    {
                                        Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                        Name = reader.GetString(reader.GetOrdinal("PaymentTypeName")),
                                        AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                        CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId"))
                                    }
                                );
                            }
                        }
                    };
                    reader.Close();
                    return Ok(customer);
                }
            }
        }

        // POST api/customers
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DECLARE @TempCustomerTable TABLE (Id int)
                        INSERT INTO Customer (FirstName, LastName)
                        OUTPUT INSERTED.Id INTO @TempCustomerTable(Id)
                        VALUES (@firstName, @lastName)
                        SELECT TOP 1 @ID = Id FROM @TempCustomerTable
                    ";

                    SqlParameter outputParam = cmd.Parameters.Add("@ID", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));

                    cmd.ExecuteNonQuery();

                    var newCustomerId = (int)outputParam.Value;
                    customer.Id = newCustomerId;

                    return Ok(customer);
                }
            }
        }

        // PUT api/customer/3
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Customer
                            SET FirstName = @firstName,
                                LastName = @lastName
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", customer.LastName));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return Ok();
                        }
                        else
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No update made to customer.");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
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
                        cmd.CommandText = @"DELETE FROM Customer WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();
                        if (rowsAffected > 0)
                        {
                            return Ok();
                        }
                        else
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);

                        }
                        throw new Exception("No rows were deleted from customers.");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}