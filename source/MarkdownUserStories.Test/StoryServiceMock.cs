using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;
using MarkdownUserStories.Services;

namespace MarkdownUserStories.Test
{
    // https://stackoverflow.com/a/11296961/1628707
    // This is one of those cases where it's simpler to inherit Collection<T>
    // and add a couple of needed methods.
    public class CalledMethods : Collection<CalledMethod>
    {

        public CalledMethod this[string name]
        {
            get { return this.SingleOrDefault(a => a.Name == name); }
        }

        private CalledMethod AddAndReturn(string name)
        {

            if (this[name] == null) Add(new CalledMethod(name, 0));
            return this[name];
        }

        /// <summary>
        /// Adds a <see cref="CalledMethod"/> if necessary and increments its <see cref="CalledMethod.Count"/>
        /// </summary>
        /// <param name="name"></param>
        public void Increment(string name)
        {
            var entry = this[name] ?? AddAndReturn(name);
            entry.Count++;
        }
    }

    public class CalledMethod
    {
        public string Name { get; set; }
        public int Count { get; set; }

        public CalledMethod() { }
        public CalledMethod(string name, int count = 0)
        {
            Name = name;
            Count = count;
        }
    }

    public class StoryServiceMock : IStoryService
    {
        public IEnumerable<UserStory> UserStories { get; set; }
        public UserStory UserStory { get; set; }

        public Exception Exception = null;

        public CalledMethods CalledMethods = new CalledMethods();

        private void CheckException()
        {
            if (Exception != null) throw Exception;
        }


        public void DeleteStory(UserStory story)
        {
            CalledMethods.Increment(System.Reflection.MethodBase.GetCurrentMethod().Name);
            CheckException();
        }

        public IEnumerable<UserStory> GetStories()
        {
            CalledMethods.Increment(System.Reflection.MethodBase.GetCurrentMethod().Name);
            CheckException();
            return UserStories;
        }

        public IEnumerable<UserStory> GetStoriesWithStatus(params string[] status)
        {
            CalledMethods.Increment(System.Reflection.MethodBase.GetCurrentMethod().Name);
            throw new NotImplementedException();
        }

        public UserStory GetStory(string role, string want, string why)
        {
            CalledMethods.Increment(System.Reflection.MethodBase.GetCurrentMethod().Name);
            return UserStory;
        }

        public IEnumerable<UserStory> SaveStories(IEnumerable<UserStory> stories)
        {
            CalledMethods.Increment(System.Reflection.MethodBase.GetCurrentMethod().Name);
            throw new NotImplementedException();
        }

        public UserStory SaveStory(UserStory story)
        {
            //https://stackoverflow.com/a/10130545/1628707
            CalledMethods.Increment(System.Reflection.MethodBase.GetCurrentMethod().Name);
            CheckException();
            return UserStory;
        }
    }
}
