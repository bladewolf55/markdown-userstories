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

        public static UserStory FullUserStory
        {
            get
            {
                return new UserStory()
                {
                    CreatedOn = DateTime.Parse("1/1/2001"),
                    StartedOn = DateTime.Parse("2/1/2002"),
                    CompletedOn = DateTime.Parse("3/1/2003"),
                    Status = "In Process",
                    Sequence = 5,
                    Estimate = "XL",
                    Role = "Developer",
                    Want = "I want to see lots of unit tests",
                    Why = "so that I can learn how the program works.",
                    Discussion = @"There's going to be lots of discussion internally, based on
what the developer at [Software Meadows](https://www.softwaremeadows.com) says.

No doubt it'll just be the usual babble.",
                    AcceptanceCriteria = @"1.  Warned if there's a naming collision
2.  Able to export all stories as Markdown files
"
                };

            }
        }

        public static string FullUserStoryText
        {
            get { return FullUserStoryTextYaml + FullUserStoryTextBody; }
        }


        public static string FullUserStoryTextYaml
        {
            get
            {
                var userStory = FullUserStory;
                return $@"---
CreatedOn: 2001-01-01 07:00:00Z
StartedOn: 2002-02-01 07:00:00Z
CompletedOn: 2003-03-01 07:00:00Z
Status: {userStory.Status}
Sequence: {userStory.Sequence}
Estimate: {userStory.Estimate}
---
";
            }
        }

        public static string FullUserStoryTextBody
        {
            get
            {
                var userStory = FullUserStory;
                return $@"# As a `{userStory.Role}`, `{userStory.Want}` `{userStory.Why}`

## Discussion
{userStory.Discussion}

## Acceptance Criteria
{userStory.AcceptanceCriteria}
";               
            }
        }
    }

}
