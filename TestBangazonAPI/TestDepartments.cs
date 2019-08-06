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
    public class TestDepartments
    {
        [Fact]
        public async Task Test_Get_All_Departments()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/departments");


                string responseBody = await response.Content.ReadAsStringAsync();
                var departments = JsonConvert.DeserializeObject<List<Department>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(departments.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllDepartments = await client.GetAsync("/api/departments");


                string responseBodyWithAllDepartments = await responseWithAllDepartments.Content.ReadAsStringAsync();
                var allDepartments = JsonConvert.DeserializeObject<List<Department>>(responseBodyWithAllDepartments);


                var response = await client.GetAsync("/api/departments/" + allDepartments.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var department = JsonConvert.DeserializeObject<Computer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(department.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                var newDepartment = new Department
                {
                    Name = "Human Resources",
                    Budget = 4578
                };

                var newDepartmentAsJSON = JsonConvert.SerializeObject(newDepartment);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/departments",
                    new StringContent(newDepartmentAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newDepartmentReturned = JsonConvert.DeserializeObject<Department>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(newDepartment.Name, newDepartmentReturned.Name);
                Assert.Equal(newDepartment.Budget, newDepartmentReturned.Budget);
                Assert.True(newDepartmentReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Department()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newName = "Human Dumbsources";

                int newBudget = 69;

                var newDepartment = new Department
                {
                    Name = newName,
                    Budget = newBudget
                };

                var newDepartmentAsJSON = JsonConvert.SerializeObject(newDepartment);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/departments/2",
                    new StringContent(newDepartmentAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
