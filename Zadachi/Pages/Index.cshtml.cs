using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zadachi.Lib;
using Zadachi.Lib.Database;
using Zadachi.Models;

namespace Zadachi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IDatabaseService<Activity> _db;
        public string NameSort { get; set; }
        [BindProperty]
        public string OptionCompleted { get; set; }
        [BindProperty]
        public List<SelectListItem> OptionsCompleted { get; set; }
        public PaginatedList<Activity> Activities { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, IConfiguration configuration, UserManager<IdentityUser> user, IDatabaseService<Activity> db)
        {
            _logger = logger;
            _configuration = configuration;
            _userManager = user;
            _db = db;
        }

        public async void OnGetAsync(string sortOrder, int? pageIndex, string optionCompleted)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; 
            OptionCompleted = optionCompleted == null ? "Active" : optionCompleted;
            List<string> optionsCompletedText = new List<string> { "Active", "All", "Completed" };
            OptionsCompleted = optionsCompletedText.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
            IQueryable<Activity> activities = _db.Query();
            IdentityUser? userCurrent = await _userManager.GetUserAsync(User);
            activities = activities.Where(a => a.User == userCurrent);
            switch (OptionCompleted)
            {
                case "Active":
                    activities = activities.Where(a => a.IsCompleted == false);
                    break;
                case "Completed":
                    activities = activities.Where(a => a.IsCompleted == true);
                    break;
            }
            switch (sortOrder)
            {
                case "name_desc":
                    activities = activities.OrderByDescending(x => x.Name);
                    break;
                default:
                    activities = activities.OrderBy(x => x.Name);
                    break;
            }
            var pageSize = _configuration.GetValue("PageSize", 4);
            Activities = await PaginatedList<Activity>.CreateAsync(activities, pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var activity = await _db.GetByIdAsync(id);            
            await _db.DeleteAsync(activity);            
            return RedirectToPage("Index");
        }

        public async Task<IActionResult> OnPostEditAsync(int id)
        {
            return RedirectToPage("EditActivity", new { id = id });
        }
        public async Task<IActionResult> OnPostCompletedAsync()
        {
            return RedirectToPage(new { sortOrder = NameSort, pageIndex = 1, optionCompleted = OptionCompleted });
        }
    }
}
