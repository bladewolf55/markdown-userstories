using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MarkdownUserStories.Models
{
    public class UserStory
    {
        public DateTime CreatedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Status { get; set; } = "";
        public int Sequence { get; set; }
        public string Estimate { get; set; } = "";
        public string Role { get; set; } = "";
        public string Want { get; set; } = "";
        public string Why { get; set; } = "";
        public string Discussion { get; set; } = "";
        public string AcceptanceCriteria { get; set; } = "";
    }
}
