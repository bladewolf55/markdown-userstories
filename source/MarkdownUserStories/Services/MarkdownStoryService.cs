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
        public void DeleteStory(string id)
        {
            MarkdownPersistEngine.DeleteUserStory(id);
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

        public UserStory GetStory(string id)
        {            
            return MarkdownPersistEngine.ReadUserStoryFromId(id);
        }

        public IEnumerable<UserStory> SaveStories(IEnumerable<UserStory> stories)
        {
            // BusinessRule: Each Status is resequenced in descending order
            // with particular statuses always setting sequence to 1

            var statuses = stories.Select(a => a.Status);
            foreach(string status in statuses)
            {
                var filteredStories = stories.Where(a => a.Status == status);
                ResequenceByStatus(status, ref filteredStories );
            }

            foreach(var story in stories)
            {
                SaveStory(story);
            }
            return stories;
        }

        private void ResequenceByStatus(string status, ref IEnumerable<UserStory> stories)
        {
            if (status.Equals("Done", StringComparison.InvariantCultureIgnoreCase))
            {
                foreach(var story in stories) { story.Sequence = 1; }
            }
            //Default is to sort descending.
            int count = stories.Count();
            foreach (var story in stories)
            {
                story.Sequence = count;
                count--;
            }
        }

        public UserStory SaveStory(UserStory story)
        {
            DateTime now = DateTime.Now;
            UserStory oldStory = null;
            try
            {
                oldStory = GetStory(story.Id);
            }
            catch { }
            //BusinessRule: Set Sequence to 1 when Done
            //TODO: Don't use explicit string.
            string oldStatus = oldStory?.Status.ToLower();
            string newStatus = story.Status.ToLower();

            //BusinessRule: Set dates depending on status
            if (newStatus != oldStatus)
            {
                if (newStatus == "backlog") { story.StartedOn = null; story.CompletedOn = null; }
                if (newStatus == "in process" | newStatus == "waiting") {
                    if (!story.StartedOn.HasValue) story.StartedOn = now;
                    story.CompletedOn = null;
                }
                if (newStatus == "done")
                {
                    if (!story.StartedOn.HasValue) story.StartedOn = now;
                    if (!story.CompletedOn.HasValue) story.CompletedOn = now;
                    story.Sequence = 1;
                }
            }

            MarkdownPersistEngine.WriteUserStory(story);
            return story;
        }
    }
    
}
