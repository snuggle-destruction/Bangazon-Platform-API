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
    public class DepartmentsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public DepartmentsController(IConfiguration config)
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
        public async Task<IActionResult> Get([FromQuery] string _include, string _filter, int? _gt)
        {
            string SqlCommandText = @"SELECT d.Id as DepartmentId, d.Name, d.Budget
                                FROM Department d";

            if (_include == "employees")
            {
                SqlCommandText = @"SELECT d.Id as DepartmentId, d.Name, d.Budget,
                                e.Id as EmployeeId, e.FirstName, e.LastName, e.IsSupervisor, e.DepartmentId
                                FROM Department d
                                LEFT JOIN Employee e ON d.ID = e.DepartmentId";
            }
            if (_filter == "budget" && _gt != null)
            {
                SqlCommandText = $"{SqlCommandText} WHERE d.Budget >= @gt";
            }

            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = SqlCommandText;

                    if (_filter == "budget" && _gt != null)
                    {
                        cmd.Parameters.Add(new SqlParameter("@gt", _gt));

                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Department> departments = new List<Department>();
                    while (reader.Read())
                    {
                        Department department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("DepartmentId")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                        };

                        if (_include == "employees")
                        {
                            Employee employee = new Employee();
                            if (!reader.IsDBNull(reader.GetOrdinal("EmployeeId")))
                            {
                                employee.Id = reader.GetInt32(reader.GetOrdinal("EmployeeId"));
                                employee.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                employee.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                employee.IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"));
                                employee.DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                            }
                            else
                            {
                                employee = null;
                            };

                            if (departments.Any(d => d.Id == department.Id))
                            {
                                Department existingDepartment = departments.Find(d => d.Id == department.Id);
                                existingDepartment.Employees.Add(employee);
                            }
                            else
                            {
                                department.Employees.Add(employee);
                                departments.Add(department);
                            }
                        }
                        else
                        {
                            if (departments.Any(d => d.Id == department.Id))
                            {
                                Department existingDepartment = departments.Find(d => d.Id == department.Id);
                            }
                            else
                            {
                                departments.Add(department);
                            }
                        }
                    }

                    reader.Close();

                    return Ok(departments);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}", Name = "GetDepartment")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Name, Budget
                        FROM Department
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Department department = null;
                    if (reader.Read())
                    {
                        department = new Department
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            Budget = reader.GetInt32(reader.GetOrdinal("Budget")),
                            // You might have more columns
                        };
                    }

                    reader.Close();

                    return Ok(department);
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Department department)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"INSERT INTO Department (Name, Budget)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @budget)";
                    cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                    cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                    department.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetDepartment", new { id = department.Id }, department);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Department department)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Department
                            SET Name = @name
                                Budget = @budget
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", department.Id));
                        cmd.Parameters.Add(new SqlParameter("@name", department.Name));
                        cmd.Parameters.Add(new SqlParameter("@budget", department.Budget));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return OK();
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private IActionResult OK()
        {
            throw new NotImplementedException();
        }

        // DELETE api/values/5
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
                        cmd.CommandText = @"DELETE FROM Department WHERE Id = @id";
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
                if (!DepartmentExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool DepartmentExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Department WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
