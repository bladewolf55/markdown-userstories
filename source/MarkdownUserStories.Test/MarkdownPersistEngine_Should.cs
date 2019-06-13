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
            UserStory userStory = MarkdownStoriesMocks.NewStory;
            userStory.Role = "My Role!";
            userStory.Want = "My want,";
            userStory.Why = "Just .Because";
            string expectedFilePath = Path.Combine(_rootPath, $"My Role!`~`My want,`~`Just .Because.md");

            // act            
            // assert
            MarkdownPersistEngine.GetFilePath(userStory).Should().Be(expectedFilePath);
        }

        [Fact]
        public void Return_expected_file_path_when_why_is_null()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory userStory = MarkdownStoriesMocks.NewStory;
            userStory.Role = "My Role!";
            userStory.Want = "My want,";
            userStory.Why = null;
            string expectedFilePath = Path.Combine(_rootPath, $"My Role!`~`My want,`~`.md");

            // act            
            // assert
            MarkdownPersistEngine.GetFilePath(userStory).Should().Be(expectedFilePath);

        }

        [Fact]
        public void Return_expected_file_path_using_disallowed_characters()
        {
            // These are not ALL the disallowed characters, only the printed ones. 
            // " < > | : * ? \ /

            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory userStory = MarkdownStoriesMocks.NewStory;
            userStory.Role = "My Role!\"<>";
            userStory.Want = "My |:*want,";
            userStory.Why = "\\Just /.Because";
            string expectedFilePath = Path.Combine(_rootPath, $"My Role!___`~`My ___want,`~`_Just _.Because.md");

            // act            
            // assert
            MarkdownPersistEngine.GetFilePath(userStory).Should().Be(expectedFilePath);
        }

        [Fact]
        public void Write_UserStory_file_to_filesystem_using_disallowed_characters()
        {
            // These are not ALL the disallowed characters, only the printed ones. 
            // " < > | : * ? \ /

            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory userStory = MarkdownStoriesMocks.NewStory;
            userStory.Role = "My Role!\"<>";
            userStory.Want = "My |:*want,";
            userStory.Why = "\\Just /.Because";
            string expectedFilePath = Path.Combine(_rootPath, $"My Role!___`~`My ___want,`~`_Just _.Because.md");

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
            string strippedActual = actual.Replace("\r", "").Replace("\n", "");
            string strippedExpected = expected.Replace("\r", "").Replace("\n", "");
            strippedActual.Should().Be(strippedExpected, "the CR LF characters were stripped out");
            actual.Should().Be(expected, "all indentation was retained");
        }

        [Fact]
        public void Read_UserStory_from_file()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory expected = MarkdownStoriesMocks.FullUserStory;
            // Simulate how the UI might contruct a UserStory with only the 
            // required keys to retrieve a story.
            UserStory passedForSearch = MarkdownStoriesMocks.NewStory;
            passedForSearch.Role = expected.Role;
            passedForSearch.Want = expected.Want;
            passedForSearch.Why = expected.Why;
            // manually write the text to avoid the engine causing
            // a problem.
            string filePath = MarkdownPersistEngine.GetFilePath(expected);
            File.WriteAllText(filePath, MarkdownStoriesMocks.FullUserStoryText);

            // act
            UserStory actual = MarkdownPersistEngine.ReadUserStory(passedForSearch);

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
            var actual = MarkdownPersistEngine.ReadUserStory(expected);

            // assert
            expected.Should().BeEquivalentTo(actual);

        }

        [Fact]
        public void Successfully_round_trip_a_file()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            string expected = MarkdownStoriesMocks.FullUserStoryText;
            UserStory story = new UserStory() { Role = "a", Want = "b" };
            string filePath = MarkdownPersistEngine.GetFilePath(story);
            File.WriteAllText(filePath, expected);

            // act
            UserStory userStory = MarkdownPersistEngine.ReadUserStory(story);
            File.Delete(filePath);
            filePath = MarkdownPersistEngine.GetFilePath(userStory);
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
            string filePath = MarkdownPersistEngine.GetFilePath(story);

            // act
            File.Exists(filePath).Should().BeTrue();
            MarkdownPersistEngine.DeleteUserStory(story);

            // assert
            File.Exists(filePath).Should().BeFalse();

        }

        [Fact(Skip = "x")]
        public void Rename_file_on_changed_ids()
        {
            // arrange


            // act


            // assert
            throw new NotImplementedException();
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
