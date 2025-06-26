using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Zadachi.Lib;
using Zadachi.Models;

namespace Zadachi.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly ZadachiDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<IdentityUser> _userManager;
        public string NameSort { get; set; }
        [BindProperty]
        public string OptionCompleted { get; set; }
        [BindProperty]
        public List<SelectListItem> OptionsCompleted { get; set; }
        public PaginatedList<Activity> Activities { get; private set; }

        public IndexModel(ILogger<IndexModel> logger, ZadachiDbContext dbContext, IConfiguration configuration, UserManager<IdentityUser> user)
        {
            _logger = logger;
            _context = dbContext;
            _configuration = configuration;
            _userManager = user;
        }

        public async void OnGetAsync(string sortOrder, int? pageIndex, string optionCompleted)
        {
            NameSort = String.IsNullOrEmpty(sortOrder) ? "name_desc" : ""; 
            OptionCompleted = optionCompleted == null ? "Active" : optionCompleted;
            List<string> optionsCompletedText = new List<string> { "Active", "All", "Completed" };
            OptionsCompleted = optionsCompletedText.Select(x => new SelectListItem { Text = x, Value = x }).ToList();
            IQueryable<Activity> activities = from a in _context.Activities select a;
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
            Activities = await PaginatedList<Activity>.CreateAsync(activities.AsNoTracking(), pageIndex ?? 1, pageSize);
        }

        public async Task<IActionResult> OnPostRemoveAsync(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
                await _context.SaveChangesAsync();
            }
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
