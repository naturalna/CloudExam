using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Db.Model;
using Db.Data;
using Db.Services.Models;
using Db.Services.SessionKeyAttributes;
using System.Web.Http.ValueProviders;

namespace Db.Services.Controllers
{
    public class PostsController : BaseApiController
    {
        [HttpGet]
        public HttpResponseMessage GetAllThreads([ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
            {
                IQueryable<PostsDTO> allPosts;
                var context = new ExamContext();

                IsUserLoged(sessionKey, context);

                allPosts = FindAllPosts(context);

                var response = this.Request.CreateResponse(HttpStatusCode.OK, allPosts.OrderByDescending(x => x.PostDate));
                return response;
               // return allPosts.OrderByDescending(x=>x.PostDate);
            });

            return responseMsg;
        }

        private static IQueryable<PostsDTO> FindAllPosts(ExamContext context)
        {
            IQueryable<PostsDTO> allPosts;
            allPosts =
               from ps in context.Posts
               select new PostsDTO()
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
               };
            return allPosts;
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

        [HttpPost]
        public HttpResponseMessage PostAddPost(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey, CreatePostDTO value)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
            {
                var context = new ExamContext();

                IsUserLoged(sessionKey, context);

                Posts newPost = CreatePost(value, context);

                CreatePostResponse responsJson = new CreatePostResponse()
                {
                    Id = newPost.Id,
                    Title = newPost.Title
                };

                var response = this.Request.CreateResponse(HttpStatusCode.Created, responsJson);
                return response;
               
            });

            return responseMsg;
        }

        private static Posts CreatePost(CreatePostDTO value, ExamContext context)
        {
            Posts newPost = new Posts()
            {
                Title = value.Title,
                Text = value.Text,
            };

            newPost.PostDate = DateTime.Now;
            ICollection<Tags> allTags = CreateOrLoadTags(value, context);

            newPost.Tags = allTags;
            context.Posts.Add(newPost);
            context.SaveChanges();
            return newPost;
        }

        private static ICollection<Tags> CreateOrLoadTags(CreatePostDTO value, ExamContext context)
        {
            ICollection<Tags> allTags = new List<Tags>();

            foreach (var tag in value.Tags)
            {
                var existingTag = context.Tags.FirstOrDefault(t => t.Name == tag);

                if (existingTag == null)
                {
                    context.Tags.Add(new Tags() { Name = tag });
                    context.SaveChanges();
                }
                allTags.Add(existingTag);
            }
            return allTags;
        }

        //posts?page=0&count=2
        [HttpGet]
        public HttpResponseMessage GetPage(
           [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey, int page, int count)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
            {
                IQueryable<PostsDTO> allPosts;
                var context = new ExamContext();

                IsUserLoged(sessionKey, context);

                allPosts = FindAllPosts(context);
                //not very right but ..
                var postsOnPage = allPosts.OrderByDescending(x=>x.PostDate).Skip(page * count).Take(count);
                var response = this.Request.CreateResponse(HttpStatusCode.OK, postsOnPage.OrderByDescending(x => x.PostDate));
                return response;
            });

            return responseMsg;
        }

        //GET api/posts?keyword=webapi

        [HttpGet]
        public HttpResponseMessage GetPage(
           [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey, string keyword)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
            {
                IEnumerable<PostsDTO> allPosts;
                var context = new ExamContext();

                IsUserLoged(sessionKey, context);

                allPosts = FindAllPostsByKeyword(context, keyword);

                var response = this.Request.CreateResponse(HttpStatusCode.OK, allPosts);
                return response;
            });

            return responseMsg;
        }

        private static IQueryable<PostsDTO> FindAllPostsByKeyword(ExamContext context, string keyword)
        {
            IQueryable<PostsDTO> allPosts;
            allPosts =
               from ps in context.Posts
               where ps.Title.Contains(keyword)
               select new PostsDTO()
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
               };

           // var searchedPosts = allPosts.Where(x => x.Title.Split(new char[] { ' ' }).All(y => y.ToLower() == keyword));
            return allPosts;
        }

        //api/posts/{postId}/comment

        [HttpPut]
        [ActionName("comment")]
        public HttpResponseMessage PutComment(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey,
            int postId, AddCommentDTO commentJson)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
            {
                var context = new ExamContext();

                Users user = FindUsernameForComment(sessionKey, context);

                var post = context.Posts.FirstOrDefault(x => x.Id == postId);
                if (post == null)
                {
                    throw new ArgumentNullException("Invalid post");
                }

                Comments newComment = CreateComment(context, commentJson, user);
                post.Comments.Add(newComment);
                context.SaveChanges();

                var response = this.Request.CreateResponse(HttpStatusCode.OK);
                return response;
            });

            return responseMsg;
        }

        private static Users FindUsernameForComment(string sessionKey, ExamContext context)
        {
            Users user =
            (from u in context.Users
             where u.SessionKey == sessionKey
             select u).FirstOrDefault();

            if (user == null)
            {
                throw new ArgumentException("User is not found in database");
            }
            return user;
        }

        private Comments CreateComment(ExamContext context, AddCommentDTO commentJson, Users user)
        {
            Comments newComment = new Comments()
            {
                Text = commentJson.Text,
            };

            newComment.PostDate = DateTime.Now;
            newComment.CommentedBy = user;
            context.Comments.Add(newComment);
            context.SaveChanges();
            return newComment;
        }
    }
}
