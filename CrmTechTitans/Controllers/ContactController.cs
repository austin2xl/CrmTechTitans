using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.ViewModels;
using CrmTechTitans.Utilities;
using Microsoft.AspNetCore.Authorization;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;

namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class ContactController : Controller
    {
        private readonly CrmContext _context;

        public ContactController(CrmContext context)
        {
            _context = context;
        }

        // GET: Contact
        public async Task<IActionResult> Index(string? SearchString,
     string? actionButton, string sortDirection = "asc", string sortField = "FirstName")
        {


            var contacts = _context.Contacts
              .Include(c => c.ContactThumbnail)
              .Include(c => c.MemberContacts)
              .ThenInclude(mc => mc.Member)
               .AsNoTracking();



            // Execute the query and get the result
            var contactsList = await contacts.ToListAsync();

            return View(contactsList);
        }

        // GET: Contact/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ContactPhoto)
                .Include(c => c.MemberContacts) // Include MemberContacts
            .ThenInclude(mc => mc.Member) // Include Member entity
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contact/Create
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult Create(int? memberId, string? returnUrl)
        {
            ViewBag.MemberId = memberId;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Contact/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create([Bind("ID,FirstName,LastName,Email,Phone,ContactType")] Contact contact, int? memberId, string? returnUrl, IFormFile? contactPicture)
        {
            if (ModelState.IsValid)
            {
                // Check for duplicate contact first
                if (!string.IsNullOrEmpty(contact.FirstName) && !string.IsNullOrEmpty(contact.LastName))
                {
                    var existingContact = await _context.Contacts
                        .FirstOrDefaultAsync(c => 
                            c.FirstName.ToLower() == contact.FirstName.ToLower() && 
                            c.LastName.ToLower() == contact.LastName.ToLower() &&
                            (string.IsNullOrEmpty(c.Email) ? string.IsNullOrEmpty(contact.Email) : 
                             c.Email.ToLower() == (contact.Email == null ? "" : contact.Email.ToLower())));
                    
                    if (existingContact != null)
                    {
                        ModelState.AddModelError("", "A contact with this name and email already exists.");
                        TempData["error"] = "A contact with this name and email already exists.";
                        ViewBag.MemberId = memberId;
                        ViewBag.ReturnUrl = returnUrl;
                        return View(contact);
                    }
                }

                // Process contact picture if any
                await AddContactPicture(contact, contactPicture);
                
                _context.Add(contact);
                await _context.SaveChangesAsync();

                // If memberId is provided, create the relationship with the member
                if (memberId.HasValue)
                {
                    var memberContact = new MemberContact
                    {
                        MemberID = memberId.Value,
                        ContactID = contact.ID,
                        ContactType = contact.ContactType ?? ContactType.General
                    };
                    _context.MemberContacts.Add(memberContact);
                    await _context.SaveChangesAsync();
                }

                TempData["success"] = "Contact created successfully!";
                TempData["message"] = "Contact created successfully!";

                // Return to the specified URL or default to the Contact Index
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.MemberId = memberId;
            ViewBag.ReturnUrl = returnUrl;
            return View(contact);
        }

        // GET: Contact/Edit/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ContactPhoto)
                .Include(c => c.ContactThumbnail)
                .FirstOrDefaultAsync(m => m.ID == id);
                
            if (contact == null)
            {
                return NotFound();
            }
            
            ViewBag.ReturnUrl = returnUrl;
            return View(contact);
        }

        // POST: Contact/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,FirstName,LastName,Email,Phone,ContactType")] Contact contact, string? returnUrl, string? chkRemoveContactImage, IFormFile? contactPicture)
        {
            if (id != contact.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check for duplicate contact (excluding the current contact)
                if (!string.IsNullOrEmpty(contact.FirstName) && !string.IsNullOrEmpty(contact.LastName))
                {
                    var existingContact = await _context.Contacts
                        .FirstOrDefaultAsync(c => 
                            c.ID != id &&
                            c.FirstName.ToLower() == contact.FirstName.ToLower() && 
                            c.LastName.ToLower() == contact.LastName.ToLower() &&
                            (string.IsNullOrEmpty(c.Email) ? string.IsNullOrEmpty(contact.Email) : 
                             c.Email.ToLower() == (contact.Email == null ? "" : contact.Email.ToLower())));
                    
                    if (existingContact != null)
                    {
                        ModelState.AddModelError("", "A contact with this name and email already exists.");
                        TempData["error"] = "A contact with this name and email already exists.";
                        ViewBag.ReturnUrl = returnUrl;
                        return View(contact);
                    }
                }

                try
                {
                    // Get existing contact with photo
                    var existingContact = await _context.Contacts
                        .Include(c => c.ContactPhoto)
                        .Include(c => c.ContactThumbnail)
                        .FirstOrDefaultAsync(c => c.ID == id);
                        
                    if (existingContact == null)
                    {
                        return NotFound();
                    }

                    // Update contact properties
                    existingContact.FirstName = contact.FirstName;
                    existingContact.LastName = contact.LastName;
                    existingContact.Email = contact.Email;
                    existingContact.Phone = contact.Phone;
                    existingContact.ContactType = contact.ContactType;

                    // Handle image updates
                    if (chkRemoveContactImage != null)
                    {
                        if (existingContact.ContactPhoto != null)
                            _context.ContactPhotos.Remove(existingContact.ContactPhoto);
                        if (existingContact.ContactThumbnail != null)
                            _context.ContactThumbnails.Remove(existingContact.ContactThumbnail);
                        existingContact.ContactPhoto = null;
                        existingContact.ContactThumbnail = null;
                    }
                    else if (contactPicture != null)
                    {
                        await AddContactPicture(existingContact, contactPicture);
                    }

                    _context.Update(existingContact);
                    await _context.SaveChangesAsync();
                    
                    TempData["success"] = "Contact updated successfully!";
                    TempData["message"] = "Contact updated successfully!";

                    // Return to the specified URL or default to the Contact Index
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            
            ViewBag.ReturnUrl = returnUrl;
            return View(contact);
        }

        // GET: Contact/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.ContactPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contact/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.ContactPhoto)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Contact deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return _context.Contacts.Any(e => e.ID == id);
        }

        private async Task AddContactPicture(Contact contact, IFormFile thePicture)
        {
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0))
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray();

                        var existingPhoto = await _context.ContactPhotos
                                                          .Where(p => p.ContactID == contact.ID)
                                                          .FirstOrDefaultAsync();
                        var existingThumbnail = await _context.ContactThumbnails
                                                              .Where(t => t.ContactID == contact.ID)
                                                              .FirstOrDefaultAsync();

                        if (existingPhoto != null)
                        {
                            // Update existing photo instead of inserting a new one
                            existingPhoto.Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600);
                        }
                        else
                        {
                            contact.ContactPhoto = new ContactPhoto
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                        }

                        if (existingThumbnail != null)
                        {
                            existingThumbnail.Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70);
                        }
                        else
                        {
                            contact.ContactThumbnail = new ContactThumbnail
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }
    }
}
