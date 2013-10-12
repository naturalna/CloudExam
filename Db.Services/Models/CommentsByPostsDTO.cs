using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Db.Services.Models
{
    [DataContract(Name="text")]
    public class CommentsByPostsDTO
    {
        [DataMember(Name="text")]
        public string Text { get; set; }

        [DataMember(Name = "commentedBy")]
        public string CommentBy { get; set; }

        [DataMember(Name = "postDate")]
        public DateTime PostDate { get; set; }
    }
}
