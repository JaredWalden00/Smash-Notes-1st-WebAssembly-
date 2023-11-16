using BlazorBlog.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace BlazorBlog.Client.Services
{
    public class BlogService : IBlogService
    {
        private readonly HttpClient _http;
        public BlogService(HttpClient http)
        {
            _http = http;
        }
        public event Func<Task> BlogPostCreated;
        public event Action BlogMenuSwitch;
        public async Task<BlogPost> CreateNewBlogPost(BlogPost request)
        {
            var result = await _http.PostAsJsonAsync("api/Blog", request);
            var newPost = await result.Content.ReadFromJsonAsync<BlogPost>();
            await BlogPostCreated?.Invoke();
            return newPost;
        }

        public void MenuSwitch()
        {
            BlogMenuSwitch?.Invoke();
        }

        public async Task<BlogPost> GetBlogPostByUrl(string url)
        {
            //var post = await _http.GetFromJsonAsync<BlogPost>($"api/Blog/{url}");
            //return post;

            var result = await _http.GetAsync($"api/Blog/{url}");
            if (result.StatusCode != System.Net.HttpStatusCode.OK)
            {
                var message = await result.Content.ReadAsStringAsync();
                Console.WriteLine(message);
                return new BlogPost { Title = message };
            }
            else
            {
                return await result.Content.ReadFromJsonAsync<BlogPost>();
            }
        }

        public async Task<List<BlogPost>> GetBlogPostsByCharacterId(int characterId)
        {
            return await _http.GetFromJsonAsync<List<BlogPost>>($"api/character/{characterId}");
        }

        public async Task<List<BlogPost>> GetBlogPosts()
        {
            var response = await _http.GetFromJsonAsync<List<BlogPost>>("api/Blog");
            if (response == null)
            {
                // Handle the case when no blog posts are found (e.g., return an empty list).
                return new List<BlogPost>();
            }

            return response;
        }


        public async Task UpdateBlogPost(BlogPost blogPost)
        {
            var updateUrl = $"api/Blog/{blogPost.Id}";
            var result = await _http.PutAsJsonAsync(updateUrl, blogPost);

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update the blog post.");
            }
        }

        public async Task DeleteBlogPost(int id)
        {
            await _http.DeleteAsync($"api/blog/{id}");
        }

        public async Task AddBlogPostCharacter(string blogUrl, List<int> characterIds)
        {
            // Create an anonymous type with the required parameters
            var data = new { BlogUrl = blogUrl, CharacterIds = characterIds };

            // Send the POST request to the API endpoint
            await _http.PostAsJsonAsync("api/character", data);
        }

        public async Task UpdateBlogPostCharacter(int blogId, List<int> characterIds)
        {
            var data = new { BlogId = blogId, CharacterIds = characterIds };
            var response = await _http.PutAsJsonAsync("api/character", data);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update the blog post.");
            }
        }

        public async Task<List<Character>> GetAllCharacters()
        {
            return await _http.GetFromJsonAsync<List<Character>>("api/character");
        }

        public async Task<List<Character>> GetBlogPostCharactersById(int id)
        {
            return await _http.GetFromJsonAsync<List<Character>>($"api/blog/characters/{id}");
        }
        public List<Character> GetBlogPostCharactersByIdsync(int id)
        {
            return Task.Run(async () => await _http.GetFromJsonAsync<List<Character>>($"api/blog/characters/{id}/sync")).Result;
        }


        public async Task<Character> GetCharacterByName(string Name)
        {
            var result = await _http.GetAsync($"api/character/single/{Name}");
            return await result.Content.ReadFromJsonAsync<Character>();
        }

        public async Task DeleteBlogPostCharacters(int blogId, List<int> characterIds)
        {
            string characterIdsString = string.Join(",", characterIds);
            await _http.DeleteAsync($"api/character/{blogId}/characters/{characterIdsString}");
        }
    }
}
