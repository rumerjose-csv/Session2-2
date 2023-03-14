using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
[assembly: Parallelize(Workers = 10, Scope = ExecutionScope.MethodLevel)]
namespace Session2_2
{
    [TestClass]
    public class RestSharpTests
    {
        private static RestClient restClient;

        private static readonly string BaseURL = "https://petstore.swagger.io/v2/";

        private static readonly string UserEndpoint = "pet";
        private static string GetURL(string enpoint) => $"{BaseURL}{enpoint}";

        private static Uri GetURI(string endpoint) => new Uri(GetURL(endpoint));

        private readonly List<PetModel> cleanUpList = new List<PetModel>();
        
        [TestInitialize]
        public async Task TestInitialize()
        {
            restClient = new RestClient();
        }
        [TestCleanup]
        public async Task TestCleanup()
        {
            foreach (var data in cleanUpList)
            {
                var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{data.Id}"));
                var restResponse = await restClient.DeleteAsync(restRequest);
            }
        }

        [TestMethod]
        public async Task PostMethod()
        {
            // Create Pet object model
            #region Create Pet Model

            List<Tag> tags = new List<Tag>();
            tags.Add(new Tag()
            {
                Id = 10,
                Name = "TAG Heuer"
            }); 
            
            PetModel petModel = new PetModel()
            {
                Id = 20,
                
                Category = new Category()
                {
                    Id = 10,
                    Name = "Hunter"
                },

                Name = "Hound",
                PhotoUrls = new List<string> { "" },
                Tags = tags,
                Status = "Ready"
            };             
            
            // Send POST Request
            var postRestRequest = new RestRequest(GetURI(UserEndpoint)).AddJsonBody(petModel);
            var postRestResponse = await restClient.ExecutePostAsync(postRestRequest);             
            
            //Verify POST request status code
            Assert.AreEqual(HttpStatusCode.OK, postRestResponse.StatusCode, "Status not 200");
            #endregion             
            
            #region GetUser

            var restRequest = new RestRequest(GetURI($"{UserEndpoint}/{petModel.Id}"), Method.Get);
            var restResponse = await restClient.ExecuteAsync<PetModel>(restRequest);

            #endregion             
            #region Assertions

            Assert.AreEqual(HttpStatusCode.OK, restResponse.StatusCode, "Status code not 200");
            Assert.AreEqual(petModel.Name, restResponse.Data.Name, "Pet Name not matched.");
            Assert.AreEqual(petModel.Category.Id, restResponse.Data.Category.Id, "Category not match.");
            Assert.AreEqual(petModel.PhotoUrls[0], restResponse.Data.PhotoUrls[0], "PhotoUrls not match.");
            Assert.AreEqual(petModel.Tags[0].Name, restResponse.Data.Tags[0].Name, "Tags not match.");
            Assert.AreEqual(petModel.Status, restResponse.Data.Status, "Status not match.");
            #endregion             
            
            #region CleanUp
            //cleanUpList.Add(petModel);
            #endregion         
        }
        }
    }