using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

using Db.Services.Models;
using Db.Data;
using Db.Model;
using System.Text;
using Db.Services.SessionKeyAttributes;
using System.Web.Http.ValueProviders;

namespace Db.Services.Controllers
{
    public class UsersController : BaseApiController
    {
        private const int UsernameMinLength = 6;
        private const int UsernameMaxLength = 15;

        private const string ValidUsernameCharacters =
           "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_.";
        private const string ValidDisplayNameCharacters =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM1234567890_. -";

        private const string SessionKeyChars =
            "qwertyuioplkjhgfdsazxcvbnmQWERTYUIOPLKJHGFDSAZXCVBNM";
        private static readonly Random rand = new Random();

        private const int SessionKeyLength = 50;

        private const int Sha1Length = 40;

        
        [HttpPost]
        [ActionName("register")]
        public HttpResponseMessage PostRegisterUser(UserRegisterDTO userJSON)// don't need sessionkey for registration
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(
             () =>
             {
                 if (ModelState.IsValid && userJSON != null)
                 {
                     var context = new ExamContext();

                     using (context)
                     {
                         this.ValidateUsername(userJSON.Username);
                         this.ValidateDisplayName(userJSON.DisplayName);
                         this.ValidAuthCode(userJSON.AuthCode);

                         IsUserExisting(userJSON, context);

                         Users createdUser = CreateNewUSer(userJSON, context);

                         string sessionKey = this.GenerateSessionKey(createdUser.Id);

                         createdUser.SessionKey = sessionKey;
                         context.Entry(createdUser).State = System.Data.EntityState.Modified;
                         context.SaveChanges();

                         UserRegisterResponseDTO user = new UserRegisterResponseDTO()
                         {
                             DisplaName = userJSON.DisplayName,
                             Sessionkey = sessionKey,
                         };

                         var response = this.Request.CreateResponse(HttpStatusCode.Created, user);
                         return response;
                     }
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

        private static Users CreateNewUSer(UserRegisterDTO userJSON, ExamContext context)
        {
            // when the user is created
            Users createdUser = new Users()
            {
                DisplayName = userJSON.DisplayName,
                Username = userJSON.Username,
                AuthCode = userJSON.AuthCode,
            };
            //we can make sessionkey

            context.Users.Add(createdUser);
            context.SaveChanges();
            return createdUser;
        }

        private static void IsUserExisting(UserRegisterDTO userJSON, ExamContext context)
        {
            //if there is already such user
            Users foundUser = context.Users.FirstOrDefault(x =>
                x.Username == userJSON.Username ||
                x.DisplayName == userJSON.DisplayName);

            if (foundUser != null)
            {
                throw new ArgumentException("User already exists");
            }
        }

        private void ValidateUsername(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("Username is requerd");
            }
            if (username == string.Empty)
            {
                throw new ArgumentException("Username is requerd");
            }
            if (username.Length < UsernameMinLength)
            {
                throw new ArgumentException("Username must be at least 6 charecters long");
            }
            if (username.Length > UsernameMaxLength)
            {
                throw new ArgumentException("Username must be less than 15 charecters");
            }
            if (username.Any(ch => !ValidUsernameCharacters.Contains(ch)))
            {
                throw new ArgumentException("wrong charecter");
            }
        }

        private void ValidateDisplayName(string username)
        {
            if (username == null)
            {
                throw new ArgumentNullException("DisplayName is requerd");
            }
            if (username == null)
            {
                throw new ArgumentException("DisplayName is requerd");
            }
            if (username.Length < UsernameMinLength)
            {
                throw new ArgumentException("DisplayName must be at least 6 charecters long");
            }
            if (username.Length > UsernameMaxLength)
            {
                throw new ArgumentException("DisplayName must be less than 15 charecters");
            }
            if (username.Any(ch => !ValidDisplayNameCharacters.Contains(ch)))
            {
                throw new ArgumentException("wrong charecter!");
            }
        }

        private void ValidAuthCode(string authcode)
        {
            if (authcode.Length != 40)
            {
                throw new ArgumentException("Password should be encrypted");
            }
        }

        private string GenerateSessionKey(int userId)
        {
            StringBuilder skeyBuilder = new StringBuilder(SessionKeyLength);
            skeyBuilder.Append(userId);
            while (skeyBuilder.Length < SessionKeyLength)
            {
                var index = rand.Next(SessionKeyChars.Length);
                skeyBuilder.Append(SessionKeyChars[index]);
            }
            return skeyBuilder.ToString();
        }

        [HttpPost]
        [ActionName("login")]
        public HttpResponseMessage PostLoginUser(LoginDTO userJSON)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
             {
                 if (ModelState.IsValid)
                 {
                     var context = new ExamContext();
                     using (context)
                     {
                         var foundUser = context.Users.FirstOrDefault(u =>
                              u.Username == userJSON.Username &&
                              u.AuthCode == userJSON.AuthCode
                              );

                         if (foundUser == null)
                         {
                             throw new ArgumentOutOfRangeException("Wrong username or password");
                         }
                         else
                         {
                             string sessionKey = AddSessionKey(context, foundUser);
                             //crateREsponse
                             LoginResponseDTO user = CreateResponseLogin(foundUser, sessionKey);

                             var response = this.Request.CreateResponse(HttpStatusCode.Created, user);
                             return response;
                         }
                     }//end using
                 }//end validstate
                 else
                 {
                     var errors = String.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                     var errorMessage = string.Format("User input validation failed. Errors: {0}", errors);
                     throw new ArgumentException(errorMessage);
                 }
             });

            return responseMsg;
        }

        private static LoginResponseDTO CreateResponseLogin(Users foundUser, string sessionKey)
        {
            LoginResponseDTO user = new LoginResponseDTO()
            {
                DisplayName = foundUser.DisplayName,
                Sessionkey = sessionKey,
            };
            return user;
        }

        private string AddSessionKey(ExamContext context, Users foundUser)
        {
            string sessionKey = this.GenerateSessionKey(foundUser.Id);
            foundUser.SessionKey = sessionKey;

            context.Entry(foundUser).State = System.Data.EntityState.Modified;
            context.SaveChanges();
            return sessionKey;
        }

        [HttpPut]
        [ActionName("logout")]
        public HttpResponseMessage PutLogoutUser(
            [ValueProvider(typeof(HeaderValueProviderFactory<string>))] string sessionKey)
        {
            var responseMsg = base.PerformOperationAndHandleExceptions(() =>
             {
                 if (ModelState.IsValid)
                 {
                     var context = new ExamContext();

                     using (context)
                     {
                         if (sessionKey == null)
                         {
                             throw new ArgumentNullException("Missing sessionKey");
                         }

                         var user = context.Users.FirstOrDefault(x => x.SessionKey == sessionKey);

                         if (user == null)
                         {
                             throw new ArgumentNullException("Not found user with that sessionkey");
                         }

                         user.SessionKey = null;
                         context.Entry(user).State = System.Data.EntityState.Modified;
                         context.SaveChanges();

                         var response = this.Request.CreateResponse(HttpStatusCode.OK);
                         return response;

                     }//end using
                 }//end validstate
                 else
                 {
                     var errors = String.Join(" ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                     var errorMessage = string.Format("User input validation failed. Errors: {0}", errors);
                     throw new ArgumentException(errorMessage);
                 }
             });

            return responseMsg;
        }
    }
}
