using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBlog.Shared
{
    public class Character
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<BlogPost>? BlogPosts { get; set; }
    }

}
