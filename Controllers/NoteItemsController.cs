using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Markdig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NotesApp1.Models;
using NotesApp1.Services;

namespace NotesApp1.Controllers
{
    public class NoteItemsController : Controller
    {
        private readonly NoteItemDataService itemDataService;
        private readonly INotyfService _notyf;
        public NoteItemsController(INotyfService notyf)
        {
            itemDataService = new NoteItemDataService();
            _notyf = notyf;
        }

        public System.Security.Claims.ClaimsPrincipal GetUser()
        {
            return User;
        }

        [Authorize]
        public IActionResult Details(int? id, System.Security.Claims.ClaimsPrincipal user)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noteItem = itemDataService.GetNoteItemDetails(id.Value);
            if (noteItem == null)
            {
                return NotFound();
            }

            if(!(noteItem.NoteUser.Id.Equals(HttpContext.Session.Id)))
            {
                return BadRequest();
            }

            //noteItem.Description = Markdown.ToHtml(noteItem.Description);
            return View(noteItem);
        }

        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: NoteItems/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Create(NoteItem noteItem, IFormFile Image)
        {
            /*if (ModelState.IsValid)
            {*/
                noteItem.NoteUser.Id = _context.NoteUser.Where(NoteUser => NoteUser.UserName == User.Identity.Name).First().Id;
                noteItem.CreatedAt = DateTime.Now;
                noteItem.UpdatedAt = DateTime.Now;
                if (Image != null)
                {
                    string uploadPath = Path.Combine(".\\wwwroot", "Images");
                    string fileName = Path.GetRandomFileName();
                    string filePath = Path.Combine(uploadPath, Image.FileName);
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        Image.CopyTo(fileStream);
                    }
                    FileInfo fileInfo = new FileInfo(filePath);
                    var fileExtension = fileInfo.Extension;
                    fileName = fileName.Substring(0, fileName.IndexOf("."));
                    fileName = fileName + fileExtension;
                    if (fileInfo.Exists)
                    {
                        fileInfo.MoveTo(Path.Combine(uploadPath, fileName));
                    }
                    noteItem.Image = fileName;
                }
                itemDataService.InsertNoteItem(noteItem);
                _notyf.Success("Note added successfully.");
                return Redirect("/");
            /*}
            return View(noteItem);*/
        }

        // GET: NoteItems/Edit/5
        [Authorize]
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noteItem = itemDataService.GetNoteItemDetails(id.Value);
            if (noteItem == null)
            {
                return NotFound();
            }

            if (!(noteItem.NoteUser.Id.Equals(HttpContext.Session.Id)))
            {
                return BadRequest();
            }
            return View(noteItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Edit(NoteItem item, IFormFile Image)
        {
            /*if (id != noteItem.Id)
            {
                return NotFound();
            }*/

            NoteItem noteItem = itemDataService.GetNoteItemDetails(item.Id);
            if (noteItem == null)
            {
                return NotFound();
            }

            /*if (ModelState.IsValid)
            {*/
            try
                {
                    noteItem.Title = item.Title;
                    noteItem.Description = item.Description;
                    noteItem.UpdatedAt = DateTime.Now;
                    if(Image != null)
                    {
                        string uploadPath = Path.Combine(".\\wwwroot", "Images");
                        string fileName = Path.GetRandomFileName();
                        string filePath = Path.Combine(uploadPath, Image.FileName);
                        using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                        {
                            Image.CopyTo(fileStream);
                        }
                        FileInfo fileInfo = new FileInfo(filePath);
                        var fileExtension = fileInfo.Extension;
                        fileName = fileName.Substring(0, fileName.IndexOf("."));
                        fileName = fileName + fileExtension;
                        if (fileInfo.Exists)
                        {
                            fileInfo.MoveTo(Path.Combine(uploadPath, fileName));
                        }
                        if (noteItem.Image != null)
                        {
                            string deletePath = Path.Combine(".\\wwwroot", "Images");
                            string fileDeletePath = Path.Combine(deletePath, noteItem.Image);
                            FileInfo deleteFile = new FileInfo(fileDeletePath);
                            if (deleteFile.Exists)
                            {
                                deleteFile.Delete();
                            }
                        }
                        noteItem.Image = fileName;
                    }
                    itemDataService.UpdateNoteItem(noteItem);
                    _notyf.Success("Note edited successfully.");
                }
                    catch
                {
                    throw;
                }
                return Redirect("/");
            /*}
            return View(noteItem);*/
        }

        // GET: NoteItems/Delete/5
        [Authorize]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var noteItem = itemDataService.GetNoteItemDetails(id.Value);
            if (noteItem == null)
            {
                return NotFound();
            }

            if (!(noteItem.NoteUser.Id.Equals(HttpContext.Session.Id)))
            {
                return BadRequest();
            }
            return View(noteItem);
        }

        // POST: NoteItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Delete(int id)
        {
            var noteItem = itemDataService.GetNoteItemDetails(id);
            if (noteItem == null)
            {
                return NotFound();
            }
            if (noteItem != null)
            {
                if (noteItem.Image != null)
                {
                    string deletePath = Path.Combine(".\\wwwroot", "Images");
                    string fileDeletePath = Path.Combine(deletePath, noteItem.Image);
                    FileInfo deleteFile = new FileInfo(fileDeletePath);
                    if (deleteFile.Exists)
                    {
                        deleteFile.Delete();
                    }
                }
                itemDataService.DeleteNoteItem(noteItem);
                _notyf.Success("Note deleted successfully.");
            }
            return Redirect("/");
        }
    }
}
