using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace Db.Services.Models
{
    [DataContract(Name="comment")]
    public class AddCommentDTO
    {
        [DataMember(Name="text")]
        public string Text { get; set; }
    }
}