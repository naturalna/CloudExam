using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Db.Services.Models
{
    [DataContract(Name="postResponse")]
    public class CreatePostResponse
    {
        [DataMember(Name = "title")]
        public string Title { get; set; }

        [DataMember(Name = "id")]
        public int Id { get; set; }
    }
}