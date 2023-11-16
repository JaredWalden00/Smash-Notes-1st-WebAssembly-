using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBlog.Shared
{
    public class BlogPostCharacterRequest2
    {
        public int BlogId { get; set; }
        public List<int> CharacterIds { get; set; }
    }
}
