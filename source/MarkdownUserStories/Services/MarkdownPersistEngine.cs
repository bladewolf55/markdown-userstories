using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using MarkdownUserStories.Models;

using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization.Converters;
using System.Text;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.RepresentationModel;
using System.Globalization;


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
        public const string RoleToken = "{ROLE}";
        public const string WantToken = "{WANT}";
        public const string WhyToken = "{WHY}";
        public const string DiscussionToken = "{DISCUSSION}";
        public const string AcceptanceCriteriaToken = "{ACCEPTANCE CRITERIA}";
        public static string UtcIsoFormatString = DateTimeFormatInfo.InvariantInfo.UniversalSortableDateTimePattern;

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
            string text = GetFileContents(userStory);
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

        public static string GetFileContents(UserStory userStory)
        {
            return GetUserStoryYaml(userStory) + GetUserStoryBody(userStory);
        }

        public static string GetUserStoryYaml(UserStory userStory)
        {
            var yamlProperties = userStory.ToYamlProperties();
            var utcIsoConverter = new DateTimeConverter(kind: DateTimeKind.Local, formats: UtcIsoFormatString);
            var serializer = new SerializerBuilder()
                .WithTypeConverter(utcIsoConverter)
         
                .Build();

            string yaml = serializer.Serialize(yamlProperties);
            string final = $"---{Environment.NewLine}{yaml.Trim() + Environment.NewLine}---{Environment.NewLine}";
            return final.NormalizeYamlLineEndings();
        }

        public static string GetUserStoryBody(UserStory userStory)
        {
            var bodyProperties = userStory.ToBodyProperties();
            string markdown = UserStoryMarkdownTemplate
                .Replace(RoleToken, bodyProperties.Role)
                .Replace(WantToken, bodyProperties.Want)
                .Replace(WhyToken, bodyProperties.Why)
                .Replace(DiscussionToken, bodyProperties.Discussion)
                .Replace(AcceptanceCriteriaToken, bodyProperties.AcceptanceCriteria);
            return markdown.NormalizeYamlLineEndings();
        }

        private static string UserStoryMarkdownTemplate
        {
            get
            {
                return $@"# As a {RoleToken}, {WantToken} {WhyToken}

## Discussion
{DiscussionToken}

## Acceptance Criteria
{AcceptanceCriteriaToken}
";
            }
        }

        public class UserStoryYamlProperties
        {
            public DateTime CreatedOn { get; set; }
            public DateTime? StartedOn { get; set; }
            public DateTime? CompletedOn { get; set; }
            public string Status { get; set; } = "";
            public int Sequence { get; set; }
            public string Estimate { get; set; } = "";

        }

        public class UserStoryBodyProperties
        {
            public string Role { get; set; } = "";
            public string Want { get; set; } = "";
            public string Why { get; set; } = "";
            public string Discussion { get; set; } = "";
            public string AcceptanceCriteria { get; set; } = "";

        }

        private static UserStoryYamlProperties ToYamlProperties(this UserStory userStory)
        {
            return new UserStoryYamlProperties()
            {
                CreatedOn = userStory.CreatedOn,
                StartedOn = userStory.StartedOn,
                CompletedOn = userStory.CompletedOn,
                Status = userStory.Status,
                Sequence = userStory.Sequence,
                Estimate = userStory.Estimate
            };
        }

        private static UserStoryBodyProperties ToBodyProperties(this UserStory userStory)
        {
            return new UserStoryBodyProperties()
            {
                Role = userStory.Role,
                Want = userStory.Want,
                Why = userStory.Why,
                Discussion = userStory.Discussion,
                AcceptanceCriteria = userStory.AcceptanceCriteria
            };
        }



        //https://en.wikipedia.org/wiki/ISO_8601
        public static string ToUtcIso(this DateTime dateTime)
        {
            return dateTime.ToString(UtcIsoFormatString);
        }

        /// <summary>
        /// The YAML spec says line endings must be LF. For consistent display, normalize to CRLF.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string NormalizeYamlLineEndings(this string s)
        {
            s = s.Replace("\r","").Replace("\n",Environment.NewLine);
            return s;
        }

        #endregion
    }

}