using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Zadachi.Models;

namespace Zadachi.Pages
{
    public class EditActivityModel : PageModel
    {
        private readonly ZadachiDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EditActivityModel> _logger;
        private readonly TelegramNotifier _notifier;
        private bool _New;
        [BindProperty]
        public Activity _Activity { get; set; }
        [BindProperty]
        public bool _IsCompleted { get; set; }
        [BindProperty, Display(Name = "Activity File")]
        public IFormFile _ActivityFile { get; set; }

        public EditActivityModel(ZadachiDbContext db, UserManager<IdentityUser> userManager, IWebHostEnvironment environment, ILogger<EditActivityModel> logger, TelegramNotifier notifier)
        {
            _context = db;
            _userManager = userManager;
            _environment = environment;
            _logger = logger;
            _notifier = notifier;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            _New = id == 0;
            if (_New)
                _Activity = new();
            else            
                _Activity = await _context.Activities.FindAsync(id);      
            
            _IsCompleted = _Activity.IsCompleted;

            if (_Activity.ActivityFile != null)
            {

                var activityFile = Path.Combine(_environment.WebRootPath, "files", "activityfiles", _Activity.ActivityFile);
                using (var stream = System.IO.File.OpenRead(activityFile))
                {
                    _ActivityFile = new FormFile(stream, 0, stream.Length, null, Path.GetFileName(stream.Name));
                }
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync()
        {     
            _Activity.User = await _userManager.GetUserAsync(User);
            if (_IsCompleted && !_Activity.IsCompleted)
            {
                TempData["Message"] = $"Task {_Activity.Name} completed!";
                TempData["MessageType"] = "success";
                await _notifier.SendNotificationAsync($"Task {_Activity.Name} completed by user {_Activity.User?.UserName}");
            }
            _Activity.IsCompleted = _IsCompleted;
            if (_ActivityFile!=null)
            {
                string activityFileName = $"{_ActivityFile.FileName.Replace(".txt","")}_{DateTime.Now.ToString().Replace("/","")}.txt";
                _Activity.ActivityFile = activityFileName;            
                var activityFile = Path.Combine(_environment.WebRootPath, "files", "activityfiles", activityFileName);
                using var fileStream = new FileStream(activityFile, FileMode.Create);
                await _ActivityFile.CopyToAsync(fileStream);
            }
            if (_New)
                    _context.Activities.AddAsync(_Activity);
                else
                    _context.Activities.Update(_Activity);
            await _context.SaveChangesAsync();
            _logger.LogInformation($"User {_Activity.User?.UserName} updated activity {_Activity.Name} at {DateTime.Now.ToString()}");     
            return RedirectToPage("Index");
        }
    }
}
