using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Db.Services.Controllers;
using System.Transactions;
using Db.Services.Models;
using System.Collections.Generic;
using System.Web.Http;
using System.Net;
using Newtonsoft.Json;
using DbForum.Tests;

namespace UnitTestProject1
{
    [TestClass]
    public class UserTests
    {
        static TransactionScope tran;
        private InMemoryHttpServer httpServer;

        [TestInitialize]
        public void TestInit()
        {

            var type = typeof(UsersController);
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
        public void RegisterValidUser()
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

            Assert.AreEqual(testUser.DisplayName, answer.DisplaName);
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
        }

        [TestMethod]
        public void RegisterInvalidUser_usernameInvalidSmaller()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "small",
                DisplayName = "TestDisplayName",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.IsSuccessStatusCode == false);
        }

        [TestMethod]
        public void RegisterInvalidUser_usernameNull()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = null,
                DisplayName = "TestDisplayName",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void RegisterInvalidUser_usernameBiggerUsername()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa",
                DisplayName = "TestDisplayName",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void RegisterInvalidUser_displayNameSmaller()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "aaaaaaaa",
                DisplayName = "Test",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void RegisterInvalidUser_displayNameNull()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "aaaaaaaa",
                DisplayName = null,
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void RegisterInvalidUser_displayNameBigger()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "aaaaaaaa",
                DisplayName = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxx",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void RegisterInvalidUser_WrongAuthCodeSmaller()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "Test111",
                DisplayName = "Test111",
                AuthCode = new string('b', 30),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);

        }

        [TestMethod]
        public void RegisterInvalidUser_WrongAuthCodeBigger()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "Test111",
                DisplayName = "Test111",
                AuthCode = new string('b', 50),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        //empty string and wrong charechter

        [TestMethod]
        public void RegisterInvalidUser_UsernameEmpty()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "",
                DisplayName = "Test111",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void RegisterInvalidUser_DisplayNameEmptyString()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "Test111",
                DisplayName = "",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void RegisterInvalidUser_UsernameWrongCharecter()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "Test111<>",
                DisplayName = "Test111",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        [TestMethod]
        public void RegisterInvalidUser_DisplaynameWrongCharecter()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "Test111<>",
                DisplayName = "Test111",
                AuthCode = new string('b', 40),
            };
            //make the reques with httpServer
            var response = httpServer.Post("api/users/register", testUser);

            Assert.IsTrue(response.StatusCode == HttpStatusCode.BadRequest);
        }

        //api/users/logout
        [TestMethod]
        public void TestMethod2()
        {
            //create new user
            var testUser = new UserRegisterDTO()
            {
                Username = "VALIDUSER",
                DisplayName = "VALIDNICK",
                AuthCode = new string('b', 40)
            };
            //make the reques with httpServer
            httpServer.Post("api/users/register/", testUser);
            //username

            var testLogin = new LoginDTO()
            {
                Username = "VALIDUSER",
                AuthCode = new string('b', 40)
            };

            var response = httpServer.Post("api/users/login/", testLogin);

            string content = response.Content.ReadAsStringAsync().Result;
            LoginResponseDTO answer = JsonConvert.DeserializeObject<LoginResponseDTO>(content);

            string sessionKey = answer.Sessionkey;
            var headers = new Dictionary<string, string>();
            headers["X-sessionKey"] = sessionKey;

            //error from .Put request
            var threadsResponse = httpServer.Post("api/users/logout", headers);

            Assert.IsNotNull(answer.Sessionkey);
            Assert.IsTrue(HttpStatusCode.OK == threadsResponse.StatusCode);
        }
    }
}
