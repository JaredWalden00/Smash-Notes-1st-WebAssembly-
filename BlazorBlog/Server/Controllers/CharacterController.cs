using BlazorBlog.Server.Data;
using BlazorBlog.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBlog.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly DataContext _context;

        public CharacterController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult<List<BlogPost>> GimmeAllTheCharacters()
        {
            return Ok(_context.Characters);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateBlogPostCharacters([FromBody] BlogPostCharacterRequest2 request)
        {
            var blogPost = await _context.BlogPosts.FindAsync(request.BlogId);
            if (blogPost == null)
            {
                return NotFound(); // Blog post not found, return appropriate HTTP status code
            }

            // Get the characters based on the provided characterIds
            var characters = await _context.Characters
                .Where(c => request.CharacterIds.Contains(c.Id))
                .ToListAsync();

            // Check if all characters are valid
            if (characters.Count != request.CharacterIds.Count)
            {
                return BadRequest("One or more characters not found"); // Return appropriate HTTP status code and error message
            }

            // Replace the blog post's Characters navigation property with the updated characters
            blogPost.Characters = characters;

            // Save changes to the database
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                // Handle any potential database-related errors
                return StatusCode(500, "Database error"); // Return appropriate HTTP status code and error message
            }

            // Return the updated blog post (or any other response as needed)
            return Ok(blogPost);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<IEnumerable<BlogPost>>> GetBlogPostsByCharacterId(int id)
        {
            var blogPosts = await _context.BlogPosts
                .Where(bp => bp.Characters.Any(c => c.Id == id))
                .ToListAsync();

            return blogPosts;
        }

        [HttpGet("single/{name}")]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharacterByName(string Name)
        {
            var character = await _context.Characters
                .FirstOrDefaultAsync(c => c.Name == Name);
            return Ok(character);
        }

        [HttpPost]
        public async Task AddCharactersToBlogPost([FromBody] BlogPostCharacterRequest request)
        {
            // Check if the blog post exists
            var blogPost = _context.BlogPosts.FirstOrDefault(p => p.Url.ToLower().Equals(request.BlogUrl.ToLower()));
            
            // Ensure the Characters property is initialized
            if (blogPost.Characters == null)
            {
                blogPost.Characters = new List<Character>();
            }

            // Get the characters based on the provided characterIds
            var characters = await _context.Characters
                .Where(c => request.CharacterIds.Contains(c.Id))
                .ToListAsync();


            // Add the characters to the blog post's Characters navigation property
            blogPost.Characters.AddRange(characters);

            // Save changes to the database
            await _context.SaveChangesAsync();
            
        }

        [HttpGet("blogpostchars/{id}")]
        public async Task<ActionResult<IEnumerable<Character>>> GetCharactersOfBlogPost(int id)
        {
            var blogPost = await _context.BlogPosts
            .Include(b => b.Characters) // Ensure Characters are loaded with the BlogPost
            .FirstOrDefaultAsync(b => b.Id == id);

            if (blogPost == null)
            {
                return NotFound();
            }

            var characters = blogPost.Characters;
            return Ok(characters);
        }
        [HttpDelete("{blogId}/characters/{characterIds}")]
        public IActionResult DeleteBlogPostCharacters(int blogId, string characterIds)
        {
            try
            {
                List<int> characterIdsList = characterIds.Split(',').Select(int.Parse).ToList();
                // Assuming you have a DbContext to interact with the database.
                var blogPost = _context.BlogPosts.Include(b => b.Characters).FirstOrDefault(b => b.Id == blogId);

                if (blogPost == null)
                {
                    return NotFound();
                }

                // Remove the characters with the specified ids from the blog post's Characters collection.
                blogPost.Characters.RemoveAll(c => characterIdsList.Contains(c.Id));

                _context.SaveChanges();

                return NoContent();
            }
            catch (Exception ex)
            {
                // Handle exceptions if necessary.
                return StatusCode(500, "An error occurred while deleting characters.");
            }
        }

    }
}
