using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Db.Model
{
    public class Posts
    {
        private ICollection<Comments> comments { get; set; }
        private ICollection<Tags> tags { get; set; }

        public Posts()
        {
            this.Comments = new HashSet<Comments>();
        }

        public int Id { get; set; }

        [Required(ErrorMessage="Missing title")]
        [MaxLength(50,ErrorMessage="Too long title")]
        public string Title { get; set; }

        public Users PostedBy { get; set; }

        public DateTime PostDate { get; set; }

        [Required(ErrorMessage="Missing text")]
        [Column(TypeName = "ntext")]
        public string Text { get; set; }

        public virtual ICollection<Tags> Tags
        {
            get { return this.tags; }
            set { this.tags = value; }
        }

        public virtual ICollection<Comments> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        }
    }
}
