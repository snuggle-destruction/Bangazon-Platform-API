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
    public class TestProductTypes
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/productTypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllProductTypes = await client.GetAsync("/api/productTypes");


                string responseBodyWithAllProductTypes = await responseWithAllProductTypes.Content.ReadAsStringAsync();
                var allProductTypes = JsonConvert.DeserializeObject<List<Employee>>(responseBodyWithAllProductTypes);


                var response = await client.GetAsync("/api/productTypes/" + allProductTypes.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productType.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                var newProductType = new ProductType
                {
                    Name = "Mabel the pug"
                };

                var newProductTypeAsJSON = JsonConvert.SerializeObject(newProductType);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/productTypes",
                    new StringContent(newProductTypeAsJSON, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                var newProductTypeReturned = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.True(newProductTypeReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newName = "A mayonnaise";

                var newProductType = new ProductType
                {
                    Name = newName
                };

                var newProductTypeAsJSON = JsonConvert.SerializeObject(newProductType);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/productTypes/1",
                    new StringContent(newProductTypeAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                var GetProductType = await client.GetAsync("/api/productTypes/1");
                GetProductType.EnsureSuccessStatusCode();

                string GetProductTypeBody = await GetProductType.Content.ReadAsStringAsync();
                ProductType ModifiedProductType = JsonConvert.DeserializeObject<ProductType>(GetProductTypeBody);

                Assert.Equal(HttpStatusCode.OK, GetProductType.StatusCode);
                Assert.Equal(newName, ModifiedProductType.Name);
            }
        }

        [Fact]
        public async Task Test_Delete_One_ProductType()
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
                    "/api/productTypes/8"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

    }
}
