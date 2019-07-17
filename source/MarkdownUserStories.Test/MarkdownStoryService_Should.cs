using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;
using MarkdownUserStories.Services;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using System.Collections;
using FluentAssertions;

namespace MarkdownUserStories.Test
{
    // While I currently only have one kind of service (i.e Markdown), using
    // an enumerable class lets me test that any service implementing
    // IStoryService behaves as expected.
    public class StoryServices : IEnumerable<object[]>
    {
        List<object[]> list = new List<object[]>()
        {
            new object[] {
                new MarkdownStoryService(new MockWebHostEnvironment())
            }
        };
        public IEnumerator<object[]> GetEnumerator() => list.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }

    public class StoryService_Should
    {
        public StoryService_Should()
        {
            MarkdownTestFileHelpers.ClearTestFiles();
        }

        [Theory]
        [ClassData(typeof(StoryServices))]
        public void Set_Sequence_to_1_on_Save_when_Status_is_Done(IStoryService storyService)
        {
            // arrange
            UserStory story = new UserStory()
            {
                Role = "X",
                Want = "Y",
                Sequence = 2,
                Status = "Done"
            };
            story = storyService.SaveStory(story);
            story.Sequence.Should().Be(1);
        }

        [Theory]
        [ClassData(typeof(StoryServices))]
        public void Set_and_unset_dates_depending_on_status(IStoryService storyService)
        {
            // arrange
            var createdOn = DateTime.Now;
            UserStory story = new UserStory()
            {
                Role = "X",
                Want = "Y",
                Status = "Backlog",
                Sequence = 1,
                CreatedOn = createdOn
            };


            story.CreatedOn.Should().Be(createdOn);
            story.StartedOn.Should().BeNull($"Status is {story.Status}");
            story.CompletedOn.Should().BeNull($"Status is {story.Status}");

            story.Status = "In Process";
            story = storyService.SaveStory(story);
            story.CreatedOn.Should().Be(createdOn);
            story.StartedOn.Should().NotBeNull($"Status is {story.Status}");
            story.CompletedOn.Should().BeNull($"Status is {story.Status}");

            story.Status = "Waiting";
            story = storyService.SaveStory(story);
            story.CreatedOn.Should().Be(createdOn);
            story.StartedOn.Should().NotBeNull($"Status is {story.Status}");
            story.CompletedOn.Should().BeNull($"Status is {story.Status}");

            story.Status = "Done";
            story = storyService.SaveStory(story);
            story.CreatedOn.Should().Be(createdOn);
            story.StartedOn.Should().NotBeNull($"Status is {story.Status}");
            story.CompletedOn.Should().NotBeNull($"Status is {story.Status}");

            story.Status = "Waiting";
            story = storyService.SaveStory(story);
            story.CreatedOn.Should().Be(createdOn);
            story.StartedOn.Should().NotBeNull($"Status is {story.Status}");
            story.CompletedOn.Should().BeNull($"Status is {story.Status}");

            story.Status = "In Process";
            story = storyService.SaveStory(story);
            story.CreatedOn.Should().Be(createdOn);
            story.StartedOn.Should().NotBeNull($"Status is {story.Status}");
            story.CompletedOn.Should().BeNull($"Status is {story.Status}");

            story.Status = "Backlog";
            story = storyService.SaveStory(story);
            story.CreatedOn.Should().Be(createdOn);
            story.StartedOn.Should().BeNull($"Status is {story.Status}");
            story.CompletedOn.Should().BeNull($"Status is {story.Status}");

        }
    }
}
