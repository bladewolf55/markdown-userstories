using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarkdownUserStories.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MarkdownUserStories.ViewModels
{
    public class UserStoryEdit : UserStory
    {

        public List<SelectListItem> StatusSelect { get; } = new List<SelectListItem>()
        {
            new SelectListItem("Backlog","Backlog", true),
            new SelectListItem("In Process","In Process"),
            new SelectListItem("Waiting","Waiting"),
            new SelectListItem("Done","Done")
        };

        public UserStoryEdit() { }

        public UserStoryEdit(UserStory userStory)
        {
            base.AcceptanceCriteria = userStory.AcceptanceCriteria;
            base.CompletedOn = userStory.CompletedOn;
            base.CreatedOn = userStory.CreatedOn;
            base.Discussion = userStory.Discussion;
            base.Estimate = userStory.Estimate;
            base.Id = userStory.Id;
            base.Role = userStory.Role;
            base.Sequence = userStory.Sequence;
            base.StartedOn = userStory.StartedOn;
            base.Status = userStory.Status;
            base.Want = userStory.Want;
            base.Why = userStory.Why;
        }

    }
}
