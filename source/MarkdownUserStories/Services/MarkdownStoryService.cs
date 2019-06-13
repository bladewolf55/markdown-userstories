using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;
using Microsoft.AspNetCore.Hosting;

namespace MarkdownUserStories.Services
{
    //How to use AppData in ASP.NET Core
    //https://stackoverflow.com/a/39217780/1628707

   
    public class MarkdownStoryService : IStoryService
    {
        public MarkdownStoryService(IWebHostEnvironment webHostEnvironment)
        {
            //TODO: Set path to AppData
            string rootPath = Path.Combine(webHostEnvironment.ContentRootPath, "AppData");
            MarkdownPersistEngine.SetRootFolderPath(rootPath);
        }
        public void DeleteStory(UserStory story)
        {
            MarkdownPersistEngine.DeleteUserStory(story);
        }

        public IEnumerable<UserStory> GetStories()
        {
            return MarkdownPersistEngine.GetUserStories();
        }

        public IEnumerable<UserStory> GetStoriesWithStatus(params string[] status)
        {
            return MarkdownPersistEngine.GetUserStories()
                .Where(a => status.Contains(a.Status, StringComparer.InvariantCultureIgnoreCase));
        }

        public UserStory GetStory(string role, string want, string why)
        {
            UserStory story = new UserStory() { Role = role, Want = want, Why = why };
            return MarkdownPersistEngine.ReadUserStory(story);
        }

        public IEnumerable<UserStory> SaveStories(IEnumerable<UserStory> stories)
        {
            foreach(var story in stories)
            {
                SaveStory(story);
            }
            return stories;
        }

        public UserStory SaveStory(UserStory story)
        {
            MarkdownPersistEngine.WriteUserStory(story);
            return story;
        }
    }
    
}
