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
        public void Write_expected_Markdown_from_UserStory_properties()
        {
            // arrange


            // act


            // assert

        }

        [Fact]
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
