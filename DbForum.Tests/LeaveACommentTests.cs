using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Transactions;
using System.Web.Http;
using Db.Services.Controllers;
using Db.Services.Models;
using Newtonsoft.Json;
using System.Net;

namespace DbForum.Tests
{
    /// <summary>
    /// api/posts/{postId}/comment
    /// </summary>
    [TestClass]
    public class LeaveACommentTests
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
        //AddCommentDTO

        [TestMethod]
        public void LeaveACommentValid()
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
                Title = "Some text",
                Text = "Ather text",
                Tags = new string[] { "tag1", "tag2" },
            };

            var postResponse = httpServer.Post("api/posts",newPost, headers);
            string contentResult = postResponse.Content.ReadAsStringAsync().Result;
            CreatePostResponse post = JsonConvert.DeserializeObject<CreatePostResponse>(contentResult);

            var comment = new AddCommentDTO()
            {
                Text = "textttttttttttttt"
            };

            var commentResponse = httpServer.Post("api/posts/"+ post.Id +"/comment", comment, headers);
            string commentString = commentResponse.Content.ReadAsStringAsync().Result;

            Assert.AreEqual(HttpStatusCode.OK, commentResponse.StatusCode);
        }
    }
}
