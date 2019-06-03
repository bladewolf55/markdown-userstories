using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;
using MarkdownUserStories.Services;

namespace MarkdownUserStories.Test
{
    public static class MarkdownStoriesMocks
    {
        public static IEnumerable<UserStory> CurrentStories
        {
            get
            {
                return new List<UserStory>()
                {
                    new UserStory()
                    {
                         CreatedOn = DateTime.Parse("1/1/2019"),
                         StartedOn = null,
                         CompletedOn = null,
                         Status = "",
                         Sequence = 1,
                         Estimate = "",
                         Role = "",
                         Want = "",
                         Why = "",
                         Discussion = "",
                         AcceptanceCriteria = ""
                    },
                    new UserStory()
                    {
                         CreatedOn = DateTime.Parse("2/1/2019"),
                         StartedOn = DateTime.Parse("2/2/2019"),
                         CompletedOn = null,
                         Status = "",
                         Sequence = 2,
                         Estimate = "",
                         Role = "",
                         Want = "",
                         Why = "",
                         Discussion = "",
                         AcceptanceCriteria = ""
                    },
                    new UserStory()
                    {
                         CreatedOn = DateTime.Parse("3/1/2019 13:00"),
                         StartedOn = DateTime.Parse("3/2/2019 14:00"),
                         CompletedOn = DateTime.Parse("3/3/2019 15:00"),
                         Status = "",
                         Sequence = 3,
                         Estimate = "",
                         Role = "",
                         Want = "",
                         Why = "",
                         Discussion = "",
                         AcceptanceCriteria = ""
                    },
                };
            }
        }

        /// <summary>
        /// All strings are empty, all dates null except CreateOnd = Now, Sequence = 0
        /// </summary>
        public static UserStory NewStory
        {
            get
            {
                return new UserStory()
                {
                    CreatedOn = DateTime.Now,
                    StartedOn = null,
                    CompletedOn = null,
                    Status = "",
                    Sequence = 0,
                    Estimate = "",
                    Role = "",
                    Want = "",
                    Why = "",
                    Discussion = "",
                    AcceptanceCriteria = ""

                };
            }
        }

    }
 
}
