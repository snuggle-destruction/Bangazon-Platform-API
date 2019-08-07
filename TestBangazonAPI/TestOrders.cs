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
    public class TestOrders
    {
        [Fact]
        public async Task Test_Get_All_Orders()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orders");


                string responseBody = await response.Content.ReadAsStringAsync();
                var orders = JsonConvert.DeserializeObject<List<Order>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(orders.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var responseWithAllOrders = await client.GetAsync("/api/orders");


                string responseBodyWithAllOrders = await responseWithAllOrders.Content.ReadAsStringAsync();
                var allorders = JsonConvert.DeserializeObject<List<Order>>(responseBodyWithAllOrders);


                var response = await client.GetAsync("/api/orders/" + allorders.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var order = JsonConvert.DeserializeObject<Order>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(order.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                var newOrder = new Order
                {
                    CustomerId = 1,
                    PaymentTypeId = 1
                };

                var newOrderAsJSON = JsonConvert.SerializeObject(newOrder);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/orders",
                    new StringContent(newOrderAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newOrderReturned = JsonConvert.DeserializeObject<Order>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newOrderReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                int newCustomerId = 3;
                var newOrder = new Order
                {
                    CustomerId = newCustomerId,
                    PaymentTypeId = 4
                };

                var newOrderAsJSON = JsonConvert.SerializeObject(newOrder);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/orders/6",
                    new StringContent(newOrderAsJSON, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Order()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ACT
                */
                var response = await client.DeleteAsync(
                    "/api/orders/6"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
