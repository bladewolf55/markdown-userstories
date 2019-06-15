using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
//
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace MarkdownUserStories.Models
{
    public class UserStory
    {
        public string Id { get; set; } = "";
        public DateTime CreatedOn { get; set; }
        public DateTime? StartedOn { get; set; }
        public DateTime? CompletedOn { get; set; }
        public string Status { get; set; } = "";
        [Range(1,99) ]
        public int Sequence { get; set; }
        public string Estimate { get; set; } = "";
        [Required]
        public string Role { get; set; } = "";
        public string Want { get; set; } = "";
        public string Why { get; set; } = "";
        public string Discussion { get; set; } = "";
        public string AcceptanceCriteria { get; set; } = "";
    }
}
