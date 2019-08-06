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
    public class EmployeesController : ControllerBase
    {
        private readonly IConfiguration _config;

        public EmployeesController(IConfiguration config)
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

        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                var employeesList = new List<Employee>();

                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    cmd.CommandText = @"SELECT e.Id, e.FirstName, e.LastName, e.IsSupervisor, e.DepartmentId, d.Name
                                        FROM Employee e
                                        JOIN Department d
                                        ON e.DepartmentId = d.Id";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();


                    while (reader.Read())
                    {
                        int idColumnPosition = reader.GetOrdinal("Id");
                        int idValue = reader.GetInt32(idColumnPosition);

                        var FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        var LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        var IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor"));
                        var DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"));
                        var Name = reader.GetString(reader.GetOrdinal("Name"));

                        Employee employee = new Employee
                        {
                            Id = idValue,
                            FirstName = FirstName,
                            LastName = LastName,
                            IsSupervisor = IsSupervisor,
                            DepartmentId = DepartmentId,
                            department = new Department { Name = reader.GetString(reader.GetOrdinal("Name"))}
                        };

                        employeesList.Add(employee);
                    }

                    reader.Close();

                    return Ok(employeesList);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (!EmployeeExists(id))
            {
                return new StatusCodeResult(StatusCodes.Status404NotFound);
            }
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, FirstName, LastName, IsSupervisor, DepartmentId 
                                        FROM Employee
                                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    Employee emplopyee = null;
                    if (reader.Read())
                    {
                        emplopyee = new Employee
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            IsSupervisor = reader.GetBoolean(reader.GetOrdinal("IsSupervisor")),
                            DepartmentId = reader.GetInt32(reader.GetOrdinal("DepartmentId"))
                        };
                    }

                    reader.Close();

                    return Ok(emplopyee);
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Employee employee)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"DECLARE @EmployeeTemp TABLE (Id int);
                                        
                                        INSERT INTO Employee (FirstName, LastName, IsSupervisor, DepartmentId)
                                        OUTPUT INSERTED.Id INTO @EmployeeTemp(Id)
                                        VALUES (@firstName, @lastName, @isSupervisor, @departmentId)
                                        
                                        SELECT TOP 1 @ID = Id FROM @EmployeeTemp";

                    SqlParameter outputParam = cmd.Parameters.Add("@ID", SqlDbType.Int);
                    outputParam.Direction = ParameterDirection.Output;

                    cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                    cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                    cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                    cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));

                    cmd.ExecuteNonQuery();

                    var newEmployeeId = (int)outputParam.Value;
                    employee.Id = newEmployeeId;


                    return Ok(employee);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Employee employee)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"UPDATE Employee
                                            SET FirstName = @firstName,
                                                LastName = @lastName,
                                                IsSupervisor = @isSupervisor,
                                                DepartmentId = @departmentId
                                            WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", employee.FirstName));
                        cmd.Parameters.Add(new SqlParameter("@lastName", employee.LastName));
                        cmd.Parameters.Add(new SqlParameter("@isSupervisor", employee.IsSupervisor));
                        cmd.Parameters.Add(new SqlParameter("@departmentId", employee.DepartmentId));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                           return Ok();
                        }
                        else
                        {
                           return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
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
                        cmd.CommandText = @"DELETE FROM Employee WHERE Id = @id";
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

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!EmployeeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool EmployeeExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Employee WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
