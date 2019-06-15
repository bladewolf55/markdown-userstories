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
using System.Text.RegularExpressions;

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
        public static string YamlStartToken = "---" + Environment.NewLine;
        public static string YamlEndToken = "---";

        private static string _rootFolderPath = "";

        public static void SetRootFolderPath(string rootFolderPath)
        {
            _rootFolderPath = rootFolderPath;
        }

        public static string RootFolderPath { get { return _rootFolderPath; } }

        /// <summary>
        /// Return path to file
        /// </summary>
        /// <param name="userStory"></param>
        /// <returns></returns>
        public static string GetFilePath(string id)
        {
            string fileName = $"{id}.md";
            string filePath = Path.Combine(RootFolderPath, fileName);
            return filePath;
        }


        public static void RenameUserStory(UserStory oldUserStory, UserStory newUserStory)
        {
            throw new NotImplementedException();
        }

        public static IEnumerable<UserStory> GetUserStories()
        {
            List<UserStory> stories = new List<UserStory>();
            foreach (string filePath in Directory.GetFiles(RootFolderPath))
            {
                stories.Add(ReadUserStoryFromPath(filePath));
            }
            return stories;
        }

        /// <summary>
        /// Sets <see cref="UserStory.Id"/> if empty or null.
        /// </summary>
        /// <param name="userStory"></param>
        public static void WriteUserStory(UserStory userStory)
        {
            if (String.IsNullOrWhiteSpace(userStory.Id))
                userStory.Id = Path.GetFileNameWithoutExtension(Path.GetRandomFileName());
            string filePath = GetFilePath(userStory.Id);
            string text = GetFileContents(userStory);
            File.WriteAllText(filePath, text);
        }

        public static UserStory ReadUserStoryFromId(string id)
        {
            string filePath = GetFilePath(id);
            return ReadUserStoryFromPath(filePath);
        }

        public static UserStory ReadUserStoryFromPath(string filePath)
        {
            UserStory userStory = new UserStory();
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"The User Story file {Path.GetFileName(filePath)} was not found.");
            }
            string userStoryText = File.ReadAllText(filePath);

            //Not valid if first line isn't yaml
            if (!userStoryText.StartsWith(YamlStartToken))
            {
                throw new Exception($"Invalid User Story. Must start with YAML delimiter {YamlStartToken}");
            }

            //string yaml = userStoryText.GetUserStoryTextYaml();
            //string body = userStoryText.GetUserStoryTextBody();

            UserStoryYamlProperties yamlProperties = userStoryText.ToYamlProperties();
            UserStoryBodyProperties bodyProperties = userStoryText.ToBodyProperties();

            userStory.Id = yamlProperties.Id;
            userStory.CreatedOn = yamlProperties.CreatedOn;
            userStory.StartedOn = yamlProperties.StartedOn;
            userStory.CompletedOn = yamlProperties.CompletedOn;
            userStory.Status = yamlProperties.Status;
            userStory.Sequence = yamlProperties.Sequence;
            userStory.Estimate = yamlProperties.Estimate;
            userStory.Role = bodyProperties.Role;
            userStory.Want = bodyProperties.Want;
            userStory.Why = bodyProperties.Why;
            userStory.Discussion = bodyProperties.Discussion;
            userStory.AcceptanceCriteria = bodyProperties.AcceptanceCriteria;

            return userStory;

        }

        public static void DeleteUserStory(string id)
        {
            try
            {
                File.Delete(GetFilePath(id));
            }
            catch { }
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
            var utcIsoConverter = new DateTimeConverter(kind: DateTimeKind.Utc, formats: UtcIsoFormatString);
            var serializer = new SerializerBuilder()
                .WithTypeConverter(utcIsoConverter)
                .Build();

            string yaml = serializer.Serialize(yamlProperties);
            string final = $"{YamlStartToken}{yaml.Trim() + Environment.NewLine}{YamlEndToken}{Environment.NewLine}";
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
                return $@"# As a `{RoleToken}`, `{WantToken}` `{WhyToken}`

## Discussion
{DiscussionToken}

## Acceptance Criteria
{AcceptanceCriteriaToken}
";
            }
        }

        public class UserStoryYamlProperties
        {
            public string Id { get; set; }
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
                Id = userStory.Id,
                CreatedOn = userStory.CreatedOn,
                StartedOn = userStory.StartedOn,
                CompletedOn = userStory.CompletedOn,
                Status = userStory.Status,
                Sequence = userStory.Sequence,
                Estimate = userStory.Estimate
            };
        }

        private static UserStoryYamlProperties ToYamlProperties(this string userStoryText)
        {
            string yaml = userStoryText.GetUserStoryTextYaml();
            var yamlProperties = new Deserializer().Deserialize<UserStoryYamlProperties>(yaml);
            return yamlProperties;
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

        private static UserStoryBodyProperties ToBodyProperties(this string userStoryText)
        {
            UserStoryBodyProperties bodyProperties = new UserStoryBodyProperties();
            string valueStartToken = "# As a ";
            string discussionStartToken = "## Discussion";
            string acceptanceStartToken = "## Acceptance Criteria";
            //string[] lines =  userStoryText.Split(new [] { "\r\n" }, StringSplitOptions.None);
            //string[] lines = Regex.Split(userStoryText, "\r\n").Select(a => a.TrimEnd()).ToArray();
            string[] lines = userStoryText.Split("\r\n");

            bool foundValue = false;
            bool foundDiscussion = false;
            bool foundAcceptance = false;

            string valueString = "";
            List<string> discussionString = new List<string>();
            List<string> acceptanceString = new List<string>();
            foreach (string line in lines)
            {
                if (line.StartsWith(valueStartToken)) { foundValue = true; }
                if (line.StartsWith(discussionStartToken)) { foundValue = false; foundDiscussion = true; continue; }
                if (line.StartsWith(acceptanceStartToken)) { foundValue = false; foundDiscussion = false; foundAcceptance = true; continue; }
                if (foundValue) { valueString += (line.Trim()); }
                if (foundDiscussion) { discussionString.Add(line.Trim()); }
                if (foundAcceptance) { acceptanceString.Add(line.Trim()); }
            }

            //Parse the value line
            var tokenValues = valueString.GetListBetween("`", "`").ToArray();
            bodyProperties.Role = tokenValues[0];
            bodyProperties.Want = tokenValues[1];
            if (tokenValues.Length > 2) bodyProperties.Why = tokenValues[2];

            //Discussion and Acceptance
            //IMPORTANT: The template adds a newline to each of these properties when written to
            //file, so remove the last newline when converting back to the property.
            bodyProperties.Discussion = String.Join(Environment.NewLine, discussionString.Take(discussionString.Count() - 1));
            bodyProperties.AcceptanceCriteria = String.Join(Environment.NewLine, acceptanceString.Take(acceptanceString.Count() - 1));

            return bodyProperties;
        }

        public static string GetUserStoryTextYaml(this string userStoryText, bool keepTokens = false)
        {
            return userStoryText.ExtractYaml(keepTokens);
        }

        public static string GetUserStoryTextBody(this string userStoryText)
        {
            string yaml = GetUserStoryTextYaml(userStoryText, keepTokens: true);
            string body = userStoryText.Replace(yaml, "").TrimStart();
            return body;
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
            s = s.Replace("\r", "").Replace("\n", Environment.NewLine);
            return s;
        }


        // VARIOUS WAYS TO EXTRACT TEXT FROM BETWEEN TWO STRING TOKENS

        /// <summary>
        /// Returns the string from <paramref name="source"/> between
        /// <paramref name="startString"/> and <paramref name="endString"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        /// <remarks>https://stackoverflow.com/a/41242251/1628707</remarks>
        public static string GetStringBetweenUsingNoRegex(this string source, string startString, string endString)
        {
            int Start = 0, End = 0;
            if (source.Contains(startString) && source.Contains(endString))
            {
                Start = source.IndexOf(startString, 0) + startString.Length;
                End = source.IndexOf(endString, Start);
                return source.Substring(Start, End - Start);
            }
            else
                return string.Empty;
        }


        /// <summary>
        /// Returns <see cref="List{String}"/> from <paramref name="source"/> between
        /// <paramref name="startString"/> and <paramref name="endString"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="startString"></param>
        /// <param name="endString"></param>
        /// <returns></returns>
        /// <remarks>https://stackoverflow.com/a/13780976/1628707</remarks>
        public static List<string> GetListBetween(this string source, string startString, string endString)
        {
            var results = new List<string>();

            string pattern = string.Format(
                "{0}({1}){2}",
                Regex.Escape(startString),
                ".+?",
                 Regex.Escape(endString));

            foreach (Match m in Regex.Matches(source, pattern,
                RegexOptions.Singleline))
            {
                results.Add(m.Groups[1].Value);
            }

            return results;
        }

        // Define other methods and classes here
        public static string GetStringBetween(this string source, string startString, string endString)
        {
            return String.Join(Environment.NewLine, GetListBetween(source, startString, endString));
        }

        /// <summary>
        /// Assumes <paramref name="source"/> has YAML delimited by ---\r\n and ---
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string ExtractYaml(this string source, bool keepTokens = false)
        {
            string yaml = GetStringBetween(source, YamlStartToken, YamlEndToken);
            if (keepTokens) yaml = YamlStartToken + yaml + YamlEndToken;
            return yaml;
        }



        #endregion
    }

    //TODO: Pull some of the helpers into here as extension methods

    public static class MarkdownPersistEngineExtenionMethods
    {
        /// <summary>
        /// Returns a string array from <paramref name="source"/> 
        /// split by <paramref name="delimiter"/>
        /// </summary>
        /// <param name="source"></param>
        /// <param name="delimiter"></param>
        /// <param name="removeEmptyEntries"></param>
        /// <returns></returns>
        /// <remarks>
        /// Wouldn't it make sense to include this overload? I bet 
        /// it's one of the most expected method calls, to split on a string.
        /// Until today (2019-06-12) I didn't know it could be done this way.
        /// https://docs.microsoft.com/en-us/dotnet/api/system.string.split?redirectedfrom=MSDN&view=netframework-4.8#System_String_Split_System_String___System_StringSplitOptions_
        /// </remarks>
        public static string[] Split(this string source, string delimiter, bool removeEmptyEntries = false)
        {
            StringSplitOptions options = removeEmptyEntries ? StringSplitOptions.RemoveEmptyEntries : StringSplitOptions.None;
            return source.Split(new string[] { delimiter }, options);
        }
    }


}