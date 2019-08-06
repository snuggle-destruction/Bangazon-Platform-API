using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;

namespace TestBangazonAPI
{
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var responseWithAllCustomers = await client.GetAsync("/api/customers");


                string responseBodyWithAllCustomers = await responseWithAllCustomers.Content.ReadAsStringAsync();
                var allcustomers = JsonConvert.DeserializeObject<List<Customer>>(responseBodyWithAllCustomers);


                var response = await client.GetAsync("/api/customers/" + allcustomers.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customer.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                var newCustomer = new Customer
                {
                    FirstName = "Someguy",
                    LastName = "Fred",
                    Products = new List<Product>(),
                    PaymentTypes = new List<PaymentType>()
                };

                var newCustomerAsJSON = JsonConvert.SerializeObject(newCustomer);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/customers",
                    new StringContent(newCustomerAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newCustomerReturned = JsonConvert.DeserializeObject<Customer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newCustomerReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                string newName = "Jeremy";
                var newCustomer = new Customer
                {
                    FirstName = "Somebody",
                    LastName = newName
                };

                var newCustomerAsJSON = JsonConvert.SerializeObject(newCustomer);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/customers/5",
                    new StringContent(newCustomerAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.DeleteAsync(
                    "/api/customers/5"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
