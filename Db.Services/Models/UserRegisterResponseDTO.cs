using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace Db.Services.Models
{
    [DataContract(Name="users")]
    public class UserRegisterResponseDTO
    {
        [DataMember(Name = "displayName")]
        public string DisplaName { get; set; }

        [DataMember(Name = "sessionKey")]
        public string Sessionkey { get; set; }
    }
}