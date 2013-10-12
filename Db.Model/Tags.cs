using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Db.Model
{
    public class Tags
    {
        public int Id { get; set; }

        [Required(ErrorMessage="Can not create empty tag"), MaxLength(25,ErrorMessage="Tag name must be smaller than 25 chars")]
        public string Name { get; set; }

        private ICollection<Posts> posts;

        public ICollection<Posts> Posts
        {
            get { return this.posts; }
            set { this.posts = value; }
        }

        public Tags()
        {
            this.Posts = new HashSet<Posts>();
        }
        
    }
}
