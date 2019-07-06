using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;
using MarkdownUserStories.ViewModels;
using MarkdownUserStories.Services;

namespace MarkdownUserStories.Test
{
    public static class MarkdownStoriesMocks
    {
        public const string FilenameWithoutExtension = "pqymwz2x";
        public static IEnumerable<UserStory> CurrentStories
        {
            get
            {
                return new List<UserStory>()
                {
                    new UserStory()
                    {
                        Id = "UserStory1",
                         CreatedOn = DateTime.Parse("1/1/2019"),
                         StartedOn = null,
                         CompletedOn = null,
                         Status = "Backlog",
                         Sequence = 1,
                         Estimate = "Estimate1",
                         Role = "Role1",
                         Want = "Want1",
                         Why = "Why1",
                         Discussion = "Discussion1",
                         AcceptanceCriteria = "AcceptanceCriteria1"
                    },
                    new UserStory()
                    {
                        Id = "UserStory2",
                         CreatedOn = DateTime.Parse("2/1/2019"),
                         StartedOn = DateTime.Parse("2/2/2019"),
                         CompletedOn = null,
                         Status = "In Process",
                         Sequence = 2,
                         Estimate = "Estimate2",
                         Role = "Role2",
                         Want = "Want2",
                         Why = "Why2",
                         Discussion = "Discussion2",
                         AcceptanceCriteria = "AcceptanceCriteria2"
                    },
                    new UserStory()
                    {
                        Id = "UserStory3",
                         CreatedOn = DateTime.Parse("3/1/2019 13:00"),
                         StartedOn = DateTime.Parse("3/2/2019 14:00"),
                         CompletedOn = DateTime.Parse("3/3/2019 15:00"),
                         Status = "Done",
                         Sequence = 3,
                         Estimate = "Estimate3",
                         Role = "Role3",
                         Want = "Want3",
                         Why = "Why3",
                         Discussion = "Discussion3",
                         AcceptanceCriteria = "AcceptanceCriteria3"
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
                    Id = "",
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
                    // This is the result of Path.GetFileNameWithoutExtension(Path.GetRandomFileName())
                    // pqymwz2x
                    // Note it still works with 8.3 file names!
                    Id = FilenameWithoutExtension,
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
                var UserStory = FullUserStory;
                return $@"---
Id: {FilenameWithoutExtension}
CreatedOn: 2001-01-01 07:00:00Z
StartedOn: 2002-02-01 07:00:00Z
CompletedOn: 2003-03-01 07:00:00Z
Status: {UserStory.Status}
Sequence: {UserStory.Sequence}
Estimate: {UserStory.Estimate}
---
";
            }
        }

        public static string FullUserStoryTextBody
        {
            get
            {
                var UserStory = FullUserStory;
                return $@"# As a `{UserStory.Role}`, `{UserStory.Want}` `{UserStory.Why}`

## Discussion
{UserStory.Discussion}

## Acceptance Criteria
{UserStory.AcceptanceCriteria}
";
            }
        }


        //------------UserStoryEdit, return the editing version of the UserStory
        public static UserStoryEdit NewStoryEdit => new UserStoryEdit(NewStory);
        public static UserStoryEdit FullUserStoryEdit => new UserStoryEdit(FullUserStory);
        public static IEnumerable<UserStoryEdit> CurrentStoriesEdit => CurrentStories.Select(a => new UserStoryEdit(a));

    }

}
