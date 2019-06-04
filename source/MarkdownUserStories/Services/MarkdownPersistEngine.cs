using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using MarkdownUserStories.Models;
using Microsoft.AspNetCore.Hosting;

namespace MarkdownUserStories.Services
{
    //How to use AppData in ASP.NET Core
    //https://stackoverflow.com/a/39217780/1628707

    public static class MarkdownPersistEngine
    {
        /// <summary>
        /// Delimits the Role, Want, Why properties in a filename.
        /// </summary>
        public const string FilePropertyDelimeter = "`~`";
        private static string _rootFolderPath = "";

        public static void SetRootFolderPath(string rootFolderPath)
        {
            _rootFolderPath = rootFolderPath;
        }

        public static string RootFolderPath { get { return _rootFolderPath; } }
        /// <summary>
        /// Return path, and filename in form Role``Want``Why.md. If Why is null, no character is used.
        /// Filename invalid characters are replaced with an underscore.
        /// </summary>
        /// <param name="userStory"></param>
        /// <returns></returns>
        public static string GetFilePath(UserStory userStory)
        {
            string fileName = $"{userStory.Role}{FilePropertyDelimeter}{userStory.Want}{FilePropertyDelimeter}{userStory.Why ?? ""}.md";
            fileName = ReplaceInvalidCharacters(fileName);
            string filePath = Path.Combine(RootFolderPath, fileName);
            return filePath;
        }


        public static void RenameUserStory(UserStory oldUserStory, UserStory newUserStory)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<UserStory> GetUserStories()
        {
            throw new NotImplementedException();
        }
        public static void WriteUserStory(UserStory userStory)
        {
            string filePath = GetFilePath(userStory);
            string yaml = GetUserStoryYaml(userStory);
            string body = GetUserStoryBody(userStory);
            string text = yaml + body;
            File.WriteAllText(filePath, text);
        }

        public static UserStory ReadUserStory(UserStory userStory)
        {
            throw new NotImplementedException();
        }


        #region "Helpers"

        private static string ReplaceInvalidCharacters(string fileName)
        {
            char[] invalidFileChars = Path.GetInvalidFileNameChars();
            fileName = String.Join("", fileName.Select(a => invalidFileChars.Contains(a) ? '_' : a));
            return fileName;
        }

        public static string GetUserStoryYaml(UserStory userStory)
        {
            throw new NotImplementedException();
        }

        public static string GetUserStoryBody(UserStory userStory)
        {
            throw new NotImplementedException();

        }

        #endregion
    }
}