using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;

namespace MarkdownUserStories.Services
{
    //How to use AppData in ASP.NET Core
    //https://stackoverflow.com/a/39217780/1628707


    public class MarkdownStoryService : IStoryService
    {
        public void DeleteStory(UserStory story)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserStory> GetStories()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserStory> GetStoriesWithStatus(params string[] status)
        {
            throw new NotImplementedException();
        }

        public UserStory GetStory(string role, string want, string why)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<UserStory> SaveStories(IEnumerable<UserStory> stories)
        {
            throw new NotImplementedException();
        }

        public UserStory SaveStory(UserStory story)
        {
            throw new NotImplementedException();
        }
    }
    
}
