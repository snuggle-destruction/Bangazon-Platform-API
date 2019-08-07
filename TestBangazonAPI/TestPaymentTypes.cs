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
    public class TestPaymentTypes
    {
            [Fact]
            public async Task Test_Get_All_Payment_Types()
            {
                using (var client = new APIClientProvider().Client)
                {
                    /*
                        ARRANGE
                    */


                    /*
                        ACT
                    */
                    var response = await client.GetAsync("/api/paymenttypes");


                    string responseBody = await response.Content.ReadAsStringAsync();
                    List<PaymentType> paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                    /*
                        ASSERT
                    */
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(paymentTypes.Count > 0);
                }
            }

            [Fact]
            public async Task Test_Get_One_Payment_Type()
            {
                using (var client = new APIClientProvider().Client)
                {
                    /*
                        ARRANGE
                    */


                    /*
                        ACT
                    */
                    var responseWithAllPaymentTypes = await client.GetAsync("/api/paymenttypes");


                    string responseBodyWithAllPaymentTypes = await responseWithAllPaymentTypes.Content.ReadAsStringAsync();
                    var allPaymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBodyWithAllPaymentTypes);


                    var response = await client.GetAsync("/api/paymenttypes/" + allPaymentTypes.First().Id);

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                    /*
                        ASSERT
                    */
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(paymentType.Id > 0);
                }
            }

            [Fact]
            public async Task Test_Add_One_Payment_Type()
            {
                using (var client = new APIClientProvider().Client)
                {
                    /*
                        ARRANGE
                    */

                    var newPaymentType = new PaymentType
                    {
                        AcctNumber = 224500,
                        Name = "Bank Of America",
                        CustomerId = 3
                    };

                    var newPaymentTypeAsJson = JsonConvert.SerializeObject(newPaymentType);

                    /*
                        ACT
                    */
                    var response = await client.PostAsync(
                        "/api/paymenttypes",
                        new StringContent(newPaymentTypeAsJson, Encoding.UTF8, "application/json")
                    );

                    string responseBody = await response.Content.ReadAsStringAsync();

                    PaymentType newPaymentTypeReturned = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                    /*
                        ASSERT
                    */
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                    Assert.True(newPaymentTypeReturned.Id > 0);
                }
            }

            [Fact]
            public async Task Test_Update_One_Payment_Type()
            {
                using (var client = new APIClientProvider().Client)
                {
                    /*
                        ARRANGE
                    */


                    var newName = "bank-of-america";
                    

                    var newPaymentType = new PaymentType
                    {
                        AcctNumber = 224500,
                        Name = newName,
                        CustomerId = 3
                    };

                    var newPaymentTypeAsJson = JsonConvert.SerializeObject(newPaymentType);

                    /*
                        ACT
                    */
                    var response = await client.PutAsync(
                        "/api/paymenttypes/5",
                        new StringContent(newPaymentTypeAsJson, Encoding.UTF8, "application/json")
                    );

                    /*
                        ASSERT
                    */
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }

            [Fact]
            public async Task Test_Delete_One_Payment_Type()
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
                        "/api/paymenttypes/6"
                    );

                    /*
                        ASSERT
                    */
                    Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                }
            }
        }
    }