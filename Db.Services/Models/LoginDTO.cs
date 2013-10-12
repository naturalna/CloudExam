using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Db.Services.Models
{
    [DataContract(Name="login")]
    public class LoginDTO
    {
        [DataMember(Name="username")]
        public string Username { get; set; }

        [DataMember(Name = "authCode")]
        public string AuthCode { get; set; }
    }
}