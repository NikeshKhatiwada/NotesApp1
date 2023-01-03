using AspNetCoreHero.ToastNotification.Abstractions;
using Markdig;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp1.Models;
using Pioneer.Pagination;
using System.Diagnostics;
using NotesApp1.Services;

namespace NotesApp1.Controllers
{
    public class HomeController : Controller
    {
        private readonly NoteItemDataService itemDataService;
        private readonly NoteUserDataService userDataService;
        private readonly ILogger<HomeController> _logger;
        private readonly IPaginatedMetaService _paginatedMetaService;
        private readonly INotyfService _notyf;

        public HomeController(ILogger<HomeController> logger, IPaginatedMetaService paginatedMetaService, INotyfService notyf)
        {
            itemDataService = new NoteItemDataService();
            userDataService = new NoteUserDataService();
            _logger = logger;
            _paginatedMetaService = paginatedMetaService;
            _notyf = notyf;
        }

        [HttpGet]
        [Authorize]
        public IActionResult Index(int page = 1)
        {
            NoteUser noteUser = userDataService.GetNoteUsers().Where(NoteUser => NoteUser.UserName == User.Identity.Name).First();
            List<NoteItem> NoteItems = new List<NoteItem>();
            int pageSize = 3;
            int totalItems = 3;
            if (itemDataService.GetNoteItemsForUser(noteUser) != null)
            {
                NoteItems = itemDataService.GetNoteItemsForUser(noteUser).ToList();
                NoteItems = NoteItems.OrderByDescending(NoteItem => NoteItem.CreatedAt).ToList();
                foreach (var noteItem in NoteItems)
                {
                    if (noteItem.Title.Length > 16)
                        noteItem.Title = noteItem.Title.Substring(0, 15);
                    noteItem.Description = Markdown.ToPlainText(noteItem.Description);
                    if (noteItem.Description.Length > 56)
                        noteItem.Description = noteItem.Description.Substring(0, 55);
                }
                totalItems = NoteItems.Count;
            }
            ViewBag.PaginatedMeta = _paginatedMetaService.GetMetaData(totalItems, page, pageSize);
            return View(NoteItems);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Search(string query)
        {
            NoteUser noteUser = userDataService.GetNoteUsers().Where(NoteUser => NoteUser.UserName == User.Identity.Name).First();
            List<NoteItem> NoteItems = itemDataService.GetNoteItemsForUser(noteUser).ToList(); //Context.NoteItem.Where(NoteItem => Convert.ToString(NoteItem.NoteUser.Id) == userId).ToList();
            NoteItems = NoteItems.Where(NoteItem => NoteItem.Title.ToLower().Contains(query.ToLower()) || NoteItem.Description.ToLower().Contains(query.ToLower())).ToList();
            foreach (var noteItem in NoteItems)
            {
                if (noteItem.Title.Length > 16)
                    noteItem.Title = noteItem.Title.Substring(0, 15);
                noteItem.Description = Markdown.ToPlainText(noteItem.Description);
                if (noteItem.Description.Length > 56)
                    noteItem.Description = noteItem.Description.Substring(0, 55);
            }
            return View("Index", NoteItems);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}