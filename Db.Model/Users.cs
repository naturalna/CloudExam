using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Db.Model
{
    public class Users
    {
        private ICollection<Posts> posts {get; set;}
        private ICollection<Comments> comments { get; set; }

        public Users()
        {
            this.Posts = new HashSet<Posts>();
            this.Comments = new HashSet<Comments>();
        }

        public int Id { get; set; }

        //unique
        [MaxLength(30)]
        [MinLength(6)]
        public string Username { get; set; }

        public string DisplayName { get; set; }

        [MaxLength(40)]
        [MinLength(40)]
        public string AuthCode { get; set; }

        public string SessionKey { get; set; }

        public virtual ICollection<Posts> Posts
        {
            get { return this.posts; }
            set { this.posts = value; }
        }

        public virtual ICollection<Comments> Comments
        {
            get { return this.comments; }
            set { this.comments = value; }
        } 
    }
}
