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
            try { Directory.Delete(_rootPath, recursive: true); } catch{}
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
        public void Write_expected_file_contents_from_UserStory_properties()
        {
            // arrange
            MarkdownPersistEngine.SetRootFolderPath(_rootPath);
            UserStory userStory = new UserStory()
            {
                CreatedOn = DateTimeHelpers.ToLocalDateTime("2001-01-01T00:00Z"),
                StartedOn = DateTimeHelpers.ToLocalDateTime("2/1/2002 00:00"),
                CompletedOn = DateTimeHelpers.ToLocalDateTime("3/1/2003 12:00 AM -7:00"),
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

            string expected = $@"---
CreatedOn: {userStory.CreatedOn.ToUtcIso()}
StartedOn: {userStory.StartedOn?.ToUtcIso()}
CompletedOn: {userStory.CompletedOn?.ToUtcIso()}
Status: {userStory.Status}
Sequence: {userStory.Sequence}
Estimate: {userStory.Estimate}
---
# As a {userStory.Role}, {userStory.Want} {userStory.Why}

## Discussion
{userStory.Discussion}

## Acceptance Criteria
{userStory.AcceptanceCriteria}
";

            // act
            string actual = MarkdownPersistEngine.GetFileContents(userStory);

            // assert
            string strippedActual = actual.Replace("\r", "").Replace("\n", "");
            string strippedExpected = expected.Replace("\r", "").Replace("\n", "");
            strippedActual.Should().Be(strippedExpected);
            actual.Should().Be(expected);
        }

        [Fact(Skip = "Not created")]
        public void Rename_file_on_changed_ids()
        {
            // arrange


            // act


            // assert
            throw new NotImplementedException();
        }


        #region "Helpers"



        #endregion

    }
}
