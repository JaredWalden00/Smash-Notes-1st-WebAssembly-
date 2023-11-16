using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBlog.Shared
{
    public class BlogPostCharacterRequest
    {
        public string BlogUrl { get; set; }
        public List<int> CharacterIds { get; set; }
    }
}
