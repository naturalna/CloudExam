using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ValueProviders;
using Db.Data;
using Db.Model;
using Db.Services.Models;
using Db.Services.SessionKeyAttributes;

namespace Db.Services.Controllers
{
    public class TagsController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage Get([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
            {
                if (ModelState.IsValid)
                {
                    var context = new ExamContext();

                    IsUserLoged(sessionKey, context);

                    var tags =
                        from t in context.Tags
                        select new TagsDTO()
                        {
                            Id = t.Id,
                            Name = t.Name,
                            Posts = t.Posts.Count,
                        };

                    var orderdTags = tags.OrderBy(x => x.Name);

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, orderdTags);
                    return response;
                }
                else
                {
                    var errors = String.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    var errorMessage = string.Format("User input validation failed. Errors: {0}", errors);
                    throw new ArgumentException(errorMessage);
                }
            });

            return responseMsg;
        }

        [HttpGet]
        [ActionName("posts")]
        public HttpResponseMessage Get([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey, 
            int tagId)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
            {
                if (ModelState.IsValid)
                {
                    var context = new ExamContext();

                    IsUserLoged(sessionKey, context);

                    var allPosts =
                        (from ps in context.Posts
                         where ps.Tags.Any(x => x.Id == tagId)
                        select new PostsForATagDTO()
                        {
                            Id = ps.Id,
                            Text = ps.Text,
                            
                            PostDate = ps.PostDate,
                            Title = ps.Title,
                            PostedBy = ps.PostedBy.Username,

                            Comments =
                            (from comment in ps.Comments
                             select new CommentsByPostsDTO()
                             {
                                 Text = comment.Text,
                                 CommentBy = comment.CommentedBy.Username,
                                 PostDate = comment.PostDate,
                             }),

                            Tags =
                            (from tag in ps.Tags
                             select tag.Name).AsEnumerable(),
                        });

                    var orderdTags = allPosts.OrderBy(x => x.PostDate);

                    var response = this.Request.CreateResponse(HttpStatusCode.OK, orderdTags);
                    return response;
                }
                else
                {
                    var errors = String.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                    var errorMessage = string.Format("User input validation failed. Errors: {0}", errors);
                    throw new ArgumentException(errorMessage);
                }
            });

            return responseMsg;
        }

        private static void IsUserLoged(string sessionKey, ExamContext context)
        {
            Users user =
            (from u in context.Users
             where u.SessionKey == sessionKey
             select u).FirstOrDefault();

            if (user == null)
            {
                throw new ArgumentException("User is not found in database");
            }
        }
    }
}
