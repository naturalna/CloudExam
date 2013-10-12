using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Model
{
    public class Comments
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Must have content")]
        [Column(TypeName = "ntext")]
        public string Text { get; set; }

        public Posts Post { get; set; }

        public Users CommentedBy { get; set; }

        public DateTime PostDate { get; set; }
    }
}
