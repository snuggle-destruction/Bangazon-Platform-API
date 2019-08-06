using System;
using System.Collections.Generic;
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
    public class TrainingProgramsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public TrainingProgramsController(IConfiguration config)
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
        public async Task<IActionResult> Get([FromRoute] bool completed)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT tp.Id, tp.[Name], tp.StartDate, tp.EndDate, tp.MaxAttendees, e.FirstName, e.LastName FROM TrainingProgram tp
                                        JOIN EmployeeTraining et ON et.TrainingProgramId = tp.id
                                        JOIN Employee e ON e.Id = et.EmployeeId";

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<TrainingProgram> trainingPrograms = new List<TrainingProgram>();
                    while (reader.Read())
                    {

                        int Id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string Name = reader.GetString(reader.GetOrdinal("Name"));
                        DateTime StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate"));
                        DateTime EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate"));
                        int MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"));
                        string FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                        string LastName = reader.GetString(reader.GetOrdinal("LastName"));
                        // You might have more columns

                        Employee employee = new Employee()
                        {
                            FirstName = FirstName,
                            LastName = LastName
                        };

                        if(!trainingPrograms.Any(tp => tp.Id == Id))
                        {
                            TrainingProgram trainingProgram = new TrainingProgram()
                            {
                                Id = Id,
                                Name = Name,
                                StartDate = StartDate,
                                EndDate = EndDate,
                                MaxAttendees = MaxAttendees,
                                AttendingEmployees = new List<Employee>()
                            };

                            trainingProgram.AttendingEmployees.Add(employee);
                            trainingPrograms.Add(trainingProgram);
                        }
                        else
                        {
                            var findProgram = trainingPrograms.Find(tp => tp.Id == Id);
                            findProgram.AttendingEmployees.Add(employee);
                        }
                    }

                    reader.Close();

                    return Ok(trainingPrograms);
                }
            }
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, Name, StartDate, EndDate, MaxAttendees
                        FROM TrainingProgram
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    TrainingProgram trainingProgram = null;
                    if (reader.Read())
                    {
                        trainingProgram = new TrainingProgram
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                            EndDate = reader.GetDateTime(reader.GetOrdinal("EndDate")),
                            MaxAttendees = reader.GetInt32(reader.GetOrdinal("MaxAttendees"))
                            // You might have more columns
                        };
                    }

                    reader.Close();

                    return Ok(trainingProgram);
                }
            }
        }

        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] TrainingProgram trainingProgram)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"INSERT INTO Department (Name, StartDate, EndDate, MaxAttendees)
                                        OUTPUT INSERTED.Id
                                        VALUES (@name, @startDate, @endDate, maxAttendees)";
                    cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                    cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                    cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                    cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));


                    trainingProgram.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetCustomer", new { id = trainingProgram.Id }, trainingProgram);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] TrainingProgram trainingProgram)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE TrainingProgram
                            SET Name = @name
                                StartDate = @startDate
                                EndDate = @endDate
                                MaxAttendees = @maxAttendees
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", trainingProgram.Id));
                        cmd.Parameters.Add(new SqlParameter("@name", trainingProgram.Name));
                        cmd.Parameters.Add(new SqlParameter("@startDate", trainingProgram.StartDate));
                        cmd.Parameters.Add(new SqlParameter("@endDate", trainingProgram.EndDate));
                        cmd.Parameters.Add(new SqlParameter("@maxAttendees", trainingProgram.MaxAttendees));


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
                if (!TrainingProgramExists(id))
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
                        cmd.CommandText = @"DELETE FROM TrainingProgram WHERE Id = @id";
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
                if (!TrainingProgramExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool TrainingProgramExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM TrainingProgram WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}