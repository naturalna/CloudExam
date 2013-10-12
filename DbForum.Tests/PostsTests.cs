using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Db.Services.Models;
using System.Transactions;
using Db.Services.Controllers;
using System.Web.Http;
using Newtonsoft.Json;
using System.Net;

namespace DbForum.Tests
{
    /// <summary>
    /// Summary description for PospsTests
    /// </summary>
    [TestClass]
    public class PostsTests
    {
        static TransactionScope tran;
        private InMemoryHttpServer httpServer;

        [TestInitialize]
        public void TestInit()
        {

            var type = typeof(PostsController);
            tran = new TransactionScope();

            var routes = new List<DbForum.Tests.InMemoryHttpServer.Route>
            {
                new DbForum.Tests.InMemoryHttpServer.Route("TagsApi", "api/tags/{tagId}/posts",
                                                                                   new
                                                                                  {
                                                                                      controller = "tags",
                                                                                      action = "posts"
                                                                                  }),
                new DbForum.Tests.InMemoryHttpServer.Route("PostApi", "api/posts/{postId}/comment",
                                                                                   new
                                                                                   {
                                                                                      controller = "posts",
                                                                                      action = "comment"
                                                                                   }),
                 new DbForum.Tests.InMemoryHttpServer.Route("UsersApi", "api/users/{action}",
                                                                                   new
                                                                                   {
                                                                                      controller = "users"
                                                                                   }),
                new DbForum.Tests.InMemoryHttpServer.Route("DefaultApi", "api/{controller}/{id}",
                                                                                    new { id = RouteParameter.Optional }),
            };

            this.httpServer = new InMemoryHttpServer("http://localhost/", routes);
        }

        [TestCleanup]
        public void TearDown()
        {
            tran.Dispose();
        }

        [TestMethod]
        public void ValidPost()
        {
            //create new user
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "TESTUsername",
                DisplayName = "TestDisplayName",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            string content = response.Content.ReadAsStringAsync().Result;
            UserRegisterResponseDTO answer = JsonConvert.DeserializeObject<UserRegisterResponseDTO>(content);

            string sessionKey = answer.Sessionkey;

            var headers = new Dictionary<string, string>();
            headers["X-sessionKey"] = sessionKey;

            CreatePostDTO newPost = new CreatePostDTO()
            {
                Title = "Some text",
                Text = "Ather text",
                Tags = new string[] { "tag1", "tag2" },
            };

            var postResponse = httpServer.Post("api/posts",newPost, headers);
            string contentResult = postResponse.Content.ReadAsStringAsync().Result;
            CreatePostResponse post = JsonConvert.DeserializeObject<CreatePostResponse>(contentResult);

            Assert.AreEqual(HttpStatusCode.Created, postResponse.StatusCode);
            Assert.IsNotNull(postResponse.Content);
            Assert.IsTrue(post.Title == "Some text");
        }

        [TestMethod]
        public void InvalidPost_PostNull()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "TESTUsername",
                DisplayName = "TestDisplayName",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            string content = response.Content.ReadAsStringAsync().Result;
            UserRegisterResponseDTO answer = JsonConvert.DeserializeObject<UserRegisterResponseDTO>(content);

            string sessionKey = answer.Sessionkey;

            var headers = new Dictionary<string, string>();
            headers["X-sessionKey"] = sessionKey;

            CreatePostDTO newPost = null;
           
            var postResponse = httpServer.Post("api/posts", newPost, headers);
            string contentResult = postResponse.Content.ReadAsStringAsync().Result;
            CreatePostResponse post = JsonConvert.DeserializeObject<CreatePostResponse>(contentResult);

            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public void InvalidPost_WithouthSessionKeyNull()
        {
            var headers = new Dictionary<string, string>();
            headers["X-sessionKey"] = null;

            CreatePostDTO newPost = new CreatePostDTO()
            {
                Title = "Some text",
                Text = "Ather text",
                Tags = new string[] { "tag1", "tag2" },
            };

            var postResponse = httpServer.Post("api/posts", newPost, headers);
            string contentResult = postResponse.Content.ReadAsStringAsync().Result;
            CreatePostResponse post = JsonConvert.DeserializeObject<CreatePostResponse>(contentResult);

            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public void InvalidPost_WithouthSessionKeyEmpty()
        {
            var headers = new Dictionary<string, string>();
            headers["X-sessionKey"] = "";

            CreatePostDTO newPost = new CreatePostDTO()
            {
                Title = "Some text",
                Text = "Ather text",
                Tags = new string[] { "tag1", "tag2" },
            };

            var postResponse = httpServer.Post("api/posts", newPost, headers);
            string contentResult = postResponse.Content.ReadAsStringAsync().Result;
            CreatePostResponse post = JsonConvert.DeserializeObject<CreatePostResponse>(contentResult);

            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public void InvalidPost_TitleNull()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "TESTUsername",
                DisplayName = "TestDisplayName",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            string content = response.Content.ReadAsStringAsync().Result;
            UserRegisterResponseDTO answer = JsonConvert.DeserializeObject<UserRegisterResponseDTO>(content);

            string sessionKey = answer.Sessionkey;

            var headers = new Dictionary<string, string>();
            headers["X-sessionKey"] = sessionKey;

            CreatePostDTO newPost = new CreatePostDTO()
            {
                Title = null,
                Text = "Ather text",
                Tags = new string[] { "tag1", "tag2" },
            };

            var postResponse = httpServer.Post("api/posts", newPost, headers);
            string contentResult = postResponse.Content.ReadAsStringAsync().Result;
            CreatePostResponse post = JsonConvert.DeserializeObject<CreatePostResponse>(contentResult);

            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }

        [TestMethod]
        public void InvalidPost_TextNull()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "TESTUsername",
                DisplayName = "TestDisplayName",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            string content = response.Content.ReadAsStringAsync().Result;
            UserRegisterResponseDTO answer = JsonConvert.DeserializeObject<UserRegisterResponseDTO>(content);

            string sessionKey = answer.Sessionkey;

            var headers = new Dictionary<string, string>();
            headers["X-sessionKey"] = sessionKey;

            CreatePostDTO newPost = new CreatePostDTO()
            {
                Title = "some text",
                Text = null,
                Tags = new string[] { "tag1", "tag2" },
            };

            var postResponse = httpServer.Post("api/posts", newPost, headers);
            string contentResult = postResponse.Content.ReadAsStringAsync().Result;
            CreatePostResponse post = JsonConvert.DeserializeObject<CreatePostResponse>(contentResult);

            Assert.AreEqual(HttpStatusCode.BadRequest, postResponse.StatusCode);
        }
    }
}
