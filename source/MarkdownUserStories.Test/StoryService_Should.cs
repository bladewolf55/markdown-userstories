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
using FluentAssertions.Execution;

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

            using (new AssertionScope())
            {

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

        [Theory]
        [ClassData(typeof(StoryServices))]
        public void Resequence_stories_according_to_business_rules_when_saved_in_batch(IStoryService storyService)
        {
            // arrange
            List<UserStory> stories = new List<UserStory>();
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "Done", Sequence = 99 });
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "Done", Sequence = 99 });
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "Waiting", Sequence = 99 });
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "Waiting", Sequence = 99 });
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "In Process", Sequence = 99 });
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "In Process", Sequence = 99 });
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "Backlog", Sequence = 99 });
            stories.Add(new UserStory() { Role = "X", Want = "Y", Status = "Backlog", Sequence = 99 });

            // act
            var expected = storyService.SaveStories(stories).ToArray();

            // assert
            using (new AssertionScope())
            {
                expected[0].Sequence.Should().Be(1);
                expected[1].Sequence.Should().Be(1);
                expected[2].Sequence.Should().Be(2);
                expected[3].Sequence.Should().Be(1);
                expected[4].Sequence.Should().Be(2);
                expected[5].Sequence.Should().Be(1);
                expected[6].Sequence.Should().Be(2);
                expected[7].Sequence.Should().Be(1);
            }
        }
    }
}
