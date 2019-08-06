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
    public class TestComputers
    {
        [Fact]
        public async Task Test_Get_All_Computers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/computers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var computers = JsonConvert.DeserializeObject<List<Computer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllComputers = await client.GetAsync("/api/computers");


                string responseBodyWithAllComputers = await responseWithAllComputers.Content.ReadAsStringAsync();
                var allComputers = JsonConvert.DeserializeObject<List<Employee>>(responseBodyWithAllComputers);


                var response = await client.GetAsync("/api/computers/" + allComputers.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var computer = JsonConvert.DeserializeObject<Computer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(computer.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                var nowDate = DateTime.Now;

                var newComputer = new Computer
                {
                    PurchaseDate = nowDate.AddDays(-10),
                    DecomissionDate = nowDate,
                    Make = "Craptastic 3000",
                    Manufacturer = "Dell"
                };

                var newComputerAsJSON = JsonConvert.SerializeObject(newComputer);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/computers",
                    new StringContent(newComputerAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newComputerReturned = JsonConvert.DeserializeObject<Computer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newComputerReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newMake = "Crappy Craptastic 3001";

                var nowDate = DateTime.Now;

                var newComputer = new Computer
                {
                    PurchaseDate = nowDate.AddDays(-10),
                    DecomissionDate = nowDate,
                    Make = newMake,
                    Manufacturer = "Dell"
                };

                var newComputerAsJSON = JsonConvert.SerializeObject(newComputer);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/computers/5",
                    new StringContent(newComputerAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Computer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var response = await client.DeleteAsync(
                    "/api/computers/8"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    }
}
