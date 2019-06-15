using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;
using MarkdownUserStories.Services;
using System.IO;
using Xunit;
using FluentAssertions;

namespace MarkdownUserStories.Test
{
    public class MarkdownPersistEngine_Should : IDisposable
    {
        private string _rootPath = "";

        public MarkdownPersistEngine_Should()
        {
            //Make this a unique id. The folder will be deleted for each test.
            string tempFolderName = "67e11740-fb7c-47c0-9c4c-e0afee2f10a7";
            _rootPath = Environment.ExpandEnvironmentVariables($@"%TEMP%\{tempFolderName}");
            //Try to delete the folder, then create.
            try { Directory.Delete(_rootPath, recursive: true); } catch { }
            Directory.CreateDirectory(_rootPath);
        }

        public void Dispose()
        {
            try { Directory.Delete(_rootPath, recursive: true); } catch { }
        }

        [Fact]
        public void Return_expected_root_path()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);

            // act            
            // assert
            MarkdownPersistEngine.RootFolderPath.Should().Be(_rootPath);
        }

        [Fact]
        public void Return_expected_filename_property_delimeter()
        {
            // arrange
            // act
            // assert
            MarkdownPersistEngine.FilePropertyDelimeter.Should().Be("`~`");
        }

        [Fact]
        public void Return_expected_file_path_using_allowed_characters()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            string id = MarkdownStoriesMocks.FilenameWithoutExtension;
            string expectedFilePath = Path.Combine(_rootPath, $"{id}.md");

            // act            
            // assert
            MarkdownPersistEngine.GetFilePath(id).Should().Be(expectedFilePath);
        }

 
        [Fact]
        public void Write_UserStory_file_to_filesystem()
        {

            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory userStory = MarkdownStoriesMocks.NewStory;
            userStory.Id = "x";
            string expectedFilePath = Path.Combine(_rootPath, $"{userStory.Id}.md");

            // act
            MarkdownPersistEngine.WriteUserStory(userStory);
            // assert
            File.Exists(expectedFilePath).Should().BeTrue();
        }

        [Fact]
        public void Get_expected_file_contents_from_UserStory_properties()
        {
            // arrange
            UserStory userStory = MarkdownStoriesMocks.FullUserStory;

            string expected = MarkdownStoriesMocks.FullUserStoryText;

            // act
            string actual = MarkdownPersistEngine.GetFileContents(userStory);

            // assert
            // Stripping CR LF asserts the text is correct, but, of course, indendation
            // matters, too.
            //string strippedActual = actual.Replace("\r", "").Replace("\n", "");
            //string strippedExpected = expected.Replace("\r", "").Replace("\n", "");
            //strippedActual.Should().Be(strippedExpected, "the CR LF characters were stripped out");
            actual.Should().Be(expected, "all indentation was retained");
        }

        [Fact]
        public void Read_UserStory_from_file()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory expected = MarkdownStoriesMocks.FullUserStory;
            // manually write the text to avoid the engine causing
            // a problem.
            string filePath = MarkdownPersistEngine.GetFilePath(expected.Id);
            File.WriteAllText(filePath, MarkdownStoriesMocks.FullUserStoryText);

            // act
            UserStory actual = MarkdownPersistEngine.ReadUserStoryFromId(expected.Id);

            // assert
            actual.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public void Successfully_round_trip_a_UserStory()
        {
            // arrange
            UserStory expected = MarkdownStoriesMocks.FullUserStory;

            // act
            MarkdownPersistEngine.WriteUserStory(expected);
            var actual = MarkdownPersistEngine.ReadUserStoryFromId(expected.Id);

            // assert
            expected.Should().BeEquivalentTo(actual);

        }

        [Fact]
        public void Successfully_round_trip_a_file()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            string expected = MarkdownStoriesMocks.FullUserStoryText;
            UserStory story = new UserStory() {Id = "a" };
            string filePath = MarkdownPersistEngine.GetFilePath(story.Id);
            File.WriteAllText(filePath, expected);

            // act
            UserStory userStory = MarkdownPersistEngine.ReadUserStoryFromId(story.Id);
            File.Delete(filePath);
            filePath = MarkdownPersistEngine.GetFilePath(userStory.Id);
            MarkdownPersistEngine.WriteUserStory(userStory);
            string actual = File.ReadAllText(filePath);
            // assert
            actual.Should().Be(expected);
        }


        [Fact]
        public void Delete_a_file()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory story = MarkdownStoriesMocks.FullUserStory;
            MarkdownPersistEngine.WriteUserStory(story);
            string filePath = MarkdownPersistEngine.GetFilePath(story.Id);

            // act
            File.Exists(filePath).Should().BeTrue();
            MarkdownPersistEngine.DeleteUserStory(story.Id);

            // assert
            File.Exists(filePath).Should().BeFalse();

        }

        [Fact]
        public void Return_a_collection_of_user_stories()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            //Add the story files using engine as little as possible to isolate the method use.
            foreach(var story in MarkdownStoriesMocks.CurrentStories)
            {
                string storyText = MarkdownPersistEngine.GetFileContents(story);
                string filePath = MarkdownPersistEngine.GetFilePath(story.Id);
                File.WriteAllText(filePath, storyText);
            }
            int expectedCount = MarkdownStoriesMocks.CurrentStories.Count();

            // act
            var stories = MarkdownPersistEngine.GetUserStories();

            // assert
            stories.Count().Should().Be(expectedCount);
        }

        [Fact]
        public void Return_yaml_text_from_user_story_text()
        {
            // arrange
            string expected = MarkdownStoriesMocks.FullUserStoryTextYaml.Replace("---\r\n", "");
            string userStoryText = MarkdownStoriesMocks.FullUserStoryText;

            // act
            string actual = MarkdownPersistEngine.GetUserStoryTextYaml(userStoryText);

            // assert
            expected.Should().BeEquivalentTo(actual);
        }


        #region "Helpers"


        #endregion

    }
}
