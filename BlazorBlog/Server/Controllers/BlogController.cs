using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using BlazorBlog.Client.Services;
using BlazorBlog.Server.Data;
using BlazorBlog.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlazorBlog.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly DataContext _context;

        public BlogController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<BlogPost>> GimmeAllTheBlogPosts()
        {
            var blogPosts = _context.BlogPosts.OrderByDescending(post => post.DateCreated).ToList();
            if (blogPosts.Count == 0)
            {
                return null;
            }

            return Ok(blogPosts);
        }


        [HttpGet("{url}")]
        public ActionResult<BlogPost> GimmeThatSingleBlogPost(string url)
        {
            var post = _context.BlogPosts.FirstOrDefault(p => p.Url.ToLower().Equals(url.ToLower()));
            if (post == null)
            {
                return NotFound("This post does not exist.");
            }

            return Ok(post);
        }

        [HttpGet("characters/{id}")]
        public async Task<ActionResult<BlogPost>> GetBlogPostCharacters(int id)
        {
            var blogPost = await _context.BlogPosts.FindAsync(id);

            if (blogPost == null)
            {
                return NotFound(); // BlogPost not found
            }

            // Find the associated characters using the junction table (BlogPostCharacter)
            var charactersInBlogPost = _context.Characters
                .Where(c => c.BlogPosts.Any(bp => bp.Id == blogPost.Id))
                .ToList();

            // You can return the characters or do any additional processing here
            return Ok(charactersInBlogPost);

        }

        [HttpGet("characters/{id}/sync")]
        public ActionResult<BlogPost> GetBlogPostCharactersSync(int id)
        {
            var blogPost = _context.BlogPosts.Find(id);

            if (blogPost == null)
            {
                return NotFound(); // BlogPost not found
            }

            // Find the associated characters using the junction table (BlogPostCharacter)
            var charactersInBlogPost = _context.Characters
                .Where(c => c.BlogPosts.Any(bp => bp.Id == blogPost.Id))
                .ToList();

            // You can return the characters or do any additional processing here
            return Ok(charactersInBlogPost);
        }


        [HttpPost]
        public async Task<ActionResult<BlogPost>> CreateNewBlogPost(BlogPost request)
        {
            var existingPost = _context.BlogPosts.FirstOrDefault(p => p.Url.ToLower().Equals(request.Url.ToLower()));
            if (existingPost != null)
            {
                return BadRequest("A post with this URL already exists.");
            }

            _context.Add(request);
            await _context.SaveChangesAsync();

            return request;
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBlogPost(int id, BlogPost updatedPost)
        {
            var existingPost = await _context.BlogPosts.FindAsync(id);
            if (existingPost == null)
            {
                return NotFound();
            }

            existingPost.Title = updatedPost.Title;
            existingPost.Content = updatedPost.Content;
            // Update other properties as needed

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogPost(int id)
        {
            var post = await _context.BlogPosts.FindAsync(id);
            if (post == null)
            {
                return NotFound();
            }

            _context.BlogPosts.Remove(post);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        
    }
}
