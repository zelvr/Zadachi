using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using Zadachi.Lib.Database;
using Zadachi.Lib.FileService;
using Zadachi.Models;

namespace Zadachi.Pages
{
    public class EditActivityModel : PageModel
    {
        private readonly IDatabaseService<Activity> _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFileService _fileService;
        private readonly ILogger<EditActivityModel> _logger;
        private readonly TelegramNotifier _notifier;
        private bool _New;
        [BindProperty]
        public Activity _Activity { get; set; }
        [BindProperty]
        public bool _IsCompleted { get; set; }
        [BindProperty, Display(Name = "Activity File")]
        public IFormFile _ActivityFile { get; set; }

        public EditActivityModel(IDatabaseService<Activity> db, UserManager<IdentityUser> userManager, IFileService fileservice, ILogger<EditActivityModel> logger, TelegramNotifier notifier)
        {
            _db = db;
            _userManager = userManager;
            _fileService = fileservice;
            _logger = logger;
            _notifier = notifier;
        }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            _New = id == 0;
            if (_New)
                _Activity = new();
            else
                _Activity = await _db.GetByIdAsync(id);      
            
            _IsCompleted = _Activity.IsCompleted;
            
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
                var activityFilePath = Path.Combine("files", "activityfiles");
                _Activity.ActivityFile = await _fileService.UploadFileAsync(_ActivityFile, activityFilePath);
            }
            if (_New)
                await _db.AddAsync(_Activity);
                else
                    await _db.UpdateAsync(_Activity);
            _logger.LogInformation($"User {_Activity.User?.UserName} updated activity {_Activity.Name} at {DateTime.Now.ToString()}");     
            return RedirectToPage("Index");
        }
    }
}
