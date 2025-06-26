using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Zadachi.Models
{
    public class Activity
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        public string? Name { get; set; }

        public IdentityUser? User { get; set; }

        public bool IsCompleted { get; set; } = false;

        public string? ActivityFile { get; set; }
    }
}
