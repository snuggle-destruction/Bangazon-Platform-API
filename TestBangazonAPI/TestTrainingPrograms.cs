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
    public class TestTrainingPrograms
    {
        [Fact]
        public async Task Test_Get_All_Training_Programs()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/trainingprograms");


                string responseBody = await response.Content.ReadAsStringAsync();
                List<TrainingProgram> trainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingPrograms.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_One_Training_Program()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var responseWithAllTrainingPrograms = await client.GetAsync("/api/trainingprograms");


                string responseBodyWithAllTrainingPrograms = await responseWithAllTrainingPrograms.Content.ReadAsStringAsync();
                var allTrainingPrograms = JsonConvert.DeserializeObject<List<TrainingProgram>>(responseBodyWithAllTrainingPrograms);


                var response = await client.GetAsync("/api/trainingprograms/" + allTrainingPrograms.First().Id);

                string responseBody = await response.Content.ReadAsStringAsync();
                var trainingProgram = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(trainingProgram.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Add_One_Training_Program()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newTrainingProgram = new TrainingProgram
                {
                    Name = "The C Sharp Wetstone",
                    StartDate = DateTime.Now.AddDays(2),
                    EndDate = DateTime.Now.AddDays(10),
                    MaxAttendees = 26
                };

                var newTrainingProgramAsJson = JsonConvert.SerializeObject(newTrainingProgram);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/trainingprograms",
                    new StringContent(newTrainingProgramAsJson, Encoding.UTF8, "application/json")
                );

                string responseBody = await response.Content.ReadAsStringAsync();

                TrainingProgram newTrainingProgramReturned = JsonConvert.DeserializeObject<TrainingProgram>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(newTrainingProgramReturned.Id > 0);
            }
        }

        [Fact]
        public async Task Test_Update_One_Training_Program()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                var newStartDate = DateTime.Now.AddDays(20);
                var newEndDate = DateTime.Now.AddDays(30);
                var newMaxAttendees = 77;

                var newTrainingProgram = new TrainingProgram
                {
                    Name = "The C Sharp Wetstone",
                    StartDate = newStartDate,
                    EndDate = newEndDate,
                    MaxAttendees = newMaxAttendees
                };

                var newTrainingProgramAsJson = JsonConvert.SerializeObject(newTrainingProgram);

                /*
                    ACT
                */
                var response = await client.PutAsync(
                    "/api/trainingprograms/7",
                    new StringContent(newTrainingProgramAsJson, Encoding.UTF8, "application/json")
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_One_Training_Program()
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
                    "/api/trainingprograms/6"
                );

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            }
        }
    }
}