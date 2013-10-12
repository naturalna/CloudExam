using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Db.Services.Models
{
    [DataContract(Name="post")]
    public class CreatePostDTO
    {
        [DataMember(Name="title")]
        public string Title { get; set; }

        [DataMember(Name="tags")]
        public IEnumerable<string> Tags { get; set; }

        [DataMember(Name="text")]
        public string Text { get; set; }
    }
}