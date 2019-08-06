using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestEmployees
    {
        [Fact]
        public async Task Test_Get_All_Employees()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/employees");


                string responseBody = await response.Content.ReadAsStringAsync();
                var employees = JsonConvert.DeserializeObject<List<Employee>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employees.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllEmployees = await client.GetAsync("/api/employees");


                string responseBodyWithAllEmployees = await responseWithAllEmployees.Content.ReadAsStringAsync();
                var allemployees = JsonConvert.DeserializeObject<List<Employee>>(responseBodyWithAllEmployees);


                var response = await client.GetAsync("/api/employees/" + allemployees.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var employee = JsonConvert.DeserializeObject<Employee>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(employee.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newEmployee = new Employee
                {
                    FirstName = "Someguy",
                    LastName = "Fred",
                    IsSupervisor = false,
                    DepartmentId = 1
                };

                var newEmployeeAsJSON = JsonConvert.SerializeObject(newEmployee);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/employees",
                    new StringContent(newEmployeeAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newEmployeeReturned = JsonConvert.DeserializeObject<Employee>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newEmployeeReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newName = "Tobey";

                var newEmployee = new Employee
                {
                    FirstName = "Someguy",
                    LastName = newName,
                    IsSupervisor = false,
                    DepartmentId = 1
                };

                var newEmployeeAsJSON = JsonConvert.SerializeObject(newEmployee);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/employees/5",
                    new StringContent(newEmployeeAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Employee()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*C:\Users\shell\group-projects\Bangazon-Platform-API\BangazonAPI\Controllers\EmployeesController.cs
                    ACT
                */
                var response = await client.DeleteAsync(
                    "/api/employees/8"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    }
}
