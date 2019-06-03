﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;

namespace MarkdownUserStories.Services
{
    public interface IStoryService
    {
        IEnumerable<UserStory> GetStories();
        IEnumerable<UserStory> GetStoriesWithStatus(params string[] status);
        IEnumerable<UserStory> SaveStories(IEnumerable<UserStory> stories);
        UserStory GetStory(string role, string want, string why);
        UserStory SaveStory(UserStory story);
        void DeleteStory(UserStory story);

    }
}
