using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;

namespace BlazorBlog.Shared
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Url { get; set; }

        [Required]
        public string Title { get; set; }
        [Required]
        [MaxLength(int.MaxValue)]
        public string Content { get; set; }
		public List<Character>? Characters { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
