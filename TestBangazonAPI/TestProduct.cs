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
    public class TestProduct
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products");


                string responseBody = await response.Content.ReadAsStringAsync();
                List<Product> products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllProducts = await client.GetAsync("/api/products");


                string responseBodyWithAllProducts = await responseWithAllProducts.Content.ReadAsStringAsync();
                var allProducts = JsonConvert.DeserializeObject<List<Product>>(responseBodyWithAllProducts);


                var response = await client.GetAsync("/api/products/" + allProducts.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(product.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newProduct = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    // price is giving me trouble because it's a decimal and a double??
                    Price = 58.5500M,
                    Title = "Goof Troop 2",
                    Description = "It's goof troop",
                    Quantity = 555
                };

                var newProductAsJson = JsonConvert.SerializeObject(newProduct);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/products",
                    new StringContent(newProductAsJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                Product newProductReturned = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newProductReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                string newTitle = "The Goofest Troop";
                string newDescription = "It's the goofest troop";
                decimal newPrice = 650.34M;

                var newProduct = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = newPrice,
                    Title = newTitle,
                    Description = newDescription,
                    Quantity = 555
                };

                var newProductAsJson = JsonConvert.SerializeObject(newProduct);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/products/6",
                    new StringContent(newProductAsJson, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Product()
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
                    "/api/Product/6"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}
