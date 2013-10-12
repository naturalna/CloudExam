using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Db.Services.Models
{
    [DataContract(Name="loginResponse")]
    public class LoginResponseDTO
    {
        [DataMember(Name = "displayName")]
        public string DisplayName { get; set; }

        [DataMember(Name = "sessionKey")]
        public string Sessionkey { get; set; }
    }
}