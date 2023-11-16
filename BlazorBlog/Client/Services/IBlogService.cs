using BlazorBlog.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazorBlog.Client.Services
{
    interface IBlogService
    {
        public event Func<Task> BlogPostCreated;
        public event Action BlogMenuSwitch;
        Task<List<BlogPost>> GetBlogPosts();
        Task<BlogPost> GetBlogPostByUrl(string url);
        Task<BlogPost> CreateNewBlogPost(BlogPost request);
        public void MenuSwitch();
        Task UpdateBlogPost(BlogPost blogPost);
        Task DeleteBlogPost(int id);

        Task AddBlogPostCharacter(string BlogUrl, List<int> characterIds);
        Task UpdateBlogPostCharacter(int blogId, List<int> characterIds);
        Task<List<Character>> GetBlogPostCharactersById(int id);
        List<Character> GetBlogPostCharactersByIdsync(int id);
        Task<List<Character>> GetAllCharacters();
        Task<List<BlogPost>> GetBlogPostsByCharacterId(int characterId);
        Task<Character> GetCharacterByName(string Name);
        Task DeleteBlogPostCharacters(int blogId, List<int> characterIds);
    }
}
