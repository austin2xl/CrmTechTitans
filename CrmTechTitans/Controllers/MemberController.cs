using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using CrmTechTitans.Models.ViewModels;
using CrmTechTitans.Utilities;
using CrmTechTitans.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using SkiaSharp;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class MemberController : Controller
    {
        private readonly CrmContext _context;

        public MemberController(CrmContext context)
        {
            _context = context;
        }

        // GET: Member
        public async Task<IActionResult> Index(string searchString, string statusFilter, string sortField, string sortDirection)
        {
            // Start with the base query
            var members = _context.Members
                .Include(m => m.MemberThumbnail)
                .Include(m => m.IndustryMembers)
                .ThenInclude(im => im.Industry)
                .Include(m => m.MemberMembershipTypes) // Include the join table
                .ThenInclude(mmt => mmt.MembershipType)
                .AsQueryable();



            return View(await members.ToListAsync());
        }

        // GET: Member/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                 .Include(m => m.MemberPhoto)
                 .Include(m => m.MemberContacts)
                     .ThenInclude(mc => mc.Contact)
                 .Include(m => m.MemberAddresses)
                     .ThenInclude(ma => ma.Address)
                 .Include(m => m.IndustryMembers)
                     .ThenInclude(im => im.Industry)
                 .Include(m => m.MemberMembershipTypes)
                     .ThenInclude(mmt => mmt.MembershipType)
                 .FirstOrDefaultAsync(m => m.ID == id);


            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        [HttpPost]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult ToggleArchive([FromBody] MembershipStatusUpdateVM model)
        {
            // Check if the model or new status is invalid
            if (model == null || string.IsNullOrWhiteSpace(model.NewStatus))
            {
                return BadRequest(new { message = "Invalid request data." });
            }

            // Find the member in the database
            var member = _context.Members.Find(model.MemberId);
            if (member == null)
            {
                return NotFound(new { message = "Member not found." });
            }

            // Attempt to parse the new status to the MembershipStatus enum
            if (!Enum.TryParse(model.NewStatus, true, out MembershipStatus newStatus))
            {
                return BadRequest(new { message = "Invalid membership status." });
            }

            if (newStatus == MembershipStatus.Cancelled)
            {
                member.MemberSince = DateTime.Today; // Store the current date
            }

            // Update member's status and reason
            member.MembershipStatus = newStatus;

            // Set the reason if provided (check if Reason is provided in the model)
            if (!string.IsNullOrWhiteSpace(model.reason))
            {
                member.Reason = model.reason;  // Update the reason field with the reason provided
            }

            // Save changes to the database
            _context.SaveChanges();

            // Return success message
            return Ok(new { message = $"Member status updated to {newStatus}" });
        }




        // GET: Member/Create
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult Create()
        {
            var model = new MemberCreateViewModel
            {
                Contacts = new List<ContactViewModel> { new ContactViewModel() }, // Add one empty contact
                Addresses = new List<AddressViewModel> { new AddressViewModel() }, // Add one empty address

                // Load Available Membership Types
                AvailableMembershipTypes = _context.MembershipTypes
            .Select(m => new MembershipTypeViewModel
            {
                ID = m.ID,
                Name = m.Name
            }).ToList(),

                AvailableIndustries = _context.Industries
            .Select(industry => new IndustryViewModel
            {
                ID = industry.ID,
                Name = industry.Name,
                NAICS = industry.NAICS
            }).ToList() // Convert Industry entities to IndustryViewModels
            };

            return View(model);
        }

        // POST: Member/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create(MemberCreateViewModel model, IFormFile? memberPicture, IFormFile? contactPicture)
        {
            // Validate required collections
            if (model.SelectedMembershipTypeIDs == null || !model.SelectedMembershipTypeIDs.Any())
            {
                ModelState.AddModelError("SelectedMembershipTypeIDs", "At least one membership type is required");
            }

            if (model.SelectedIndustryIds == null || !model.SelectedIndustryIds.Any())
            {
                ModelState.AddModelError("SelectedIndustryIds", "At least one industry is required");
            }

            // Validate addresses
            bool hasValidAddress = false;
            if (model.Addresses != null && model.Addresses.Any())
            {
                foreach (var address in model.Addresses)
                {
                    if (!string.IsNullOrWhiteSpace(address.Street) && 
                        !string.IsNullOrWhiteSpace(address.City) && 
                        address.Province != 0)
                    {
                        hasValidAddress = true;
                        break;
                    }
                }
            }

            if (!hasValidAddress)
            {
                ModelState.AddModelError("Addresses", "At least one complete address is required");
            }

            // Validate contacts
            bool hasValidContact = false;
            if (model.Contacts != null && model.Contacts.Any())
            {
                foreach (var contact in model.Contacts)
                {
                    if (!string.IsNullOrWhiteSpace(contact.FirstName) && 
                        !string.IsNullOrWhiteSpace(contact.Phone))
                    {
                        hasValidContact = true;
                        break;
                    }
                }
            }

            if (!hasValidContact)
            {
                ModelState.AddModelError("Contacts", "At least one contact with name and phone is required");
            }

            if (ModelState.IsValid)
            {
                // Check for duplicate member name
                if (!string.IsNullOrEmpty(model.MemberName))
                {
                    var existingMember = await _context.Members
                        .FirstOrDefaultAsync(m => m.MemberName.ToLower() == model.MemberName.ToLower());
                    
                    if (existingMember != null)
                    {
                        ModelState.AddModelError("MemberName", "A member with this name already exists. Please use a different name.");
                        TempData["error"] = "A member with this name already exists.";
                        
                        // Repopulate available industries and membership types before returning
                        model.AvailableIndustries = _context.Industries
                            .Select(industry => new IndustryViewModel
                            {
                                ID = industry.ID,
                                Name = industry.Name,
                                NAICS = industry.NAICS
                            }).ToList();

                        model.AvailableMembershipTypes = _context.MembershipTypes
                            .Select(m => new MembershipTypeViewModel
                            {
                                ID = m.ID,
                                Name = m.Name
                            }).ToList();
                            
                            // Add a flag to indicate validation errors
                            ViewBag.HasValidationErrors = true;
                            return View(model);
                    }
                }

                try
                {
                    await AddMemberPicture(model, memberPicture);
                    // Create Member
                    var member = new Member
                    {
                        MemberName = model.MemberName,
                        ContactedBy = model.ContactedBy,
                        CompanySize = model.CompanySize,
                        CompanyWebsite = model.CompanyWebsite,
                        MemberSince = model.MemberSince,
                        LastContactDate = model.LastContactDate,
                        Notes = model.Notes,
                        MemberPhoto = model.MemberPhoto,
                        MemberThumbnail = model.MemberThumbnail
                    };

                    // Add Addresses
                    foreach (var addressModel in model.Addresses)
                    {
                        // Skip empty addresses
                        if (string.IsNullOrWhiteSpace(addressModel.Street) && 
                            string.IsNullOrWhiteSpace(addressModel.City))
                        {
                            continue;
                        }

                        var address = new Address
                        {
                            Street = addressModel.Street,
                            City = addressModel.City,
                            Province = addressModel.Province,
                            PostalCode = addressModel.PostalCode,
                            AddressType = addressModel.AddressType
                        };
                        member.MemberAddresses.Add(new MemberAddress { Address = address });
                    }

                    // Assign multiple membership types
                    foreach (var membershipTypeID in model.SelectedMembershipTypeIDs)
                    {
                        member.MemberMembershipTypes.Add(new MemberMembershipType
                        {
                            MembershipTypeID = membershipTypeID
                        });
                    }

                    // Add Contacts
                    foreach (var contactModel in model.Contacts)
                    {
                        // Skip empty contacts
                        if (string.IsNullOrWhiteSpace(contactModel.FirstName) && 
                            string.IsNullOrWhiteSpace(contactModel.Phone))
                        {
                            continue;
                        }

                        // Check for duplicate contacts
                        if (!string.IsNullOrEmpty(contactModel.Email) || !string.IsNullOrEmpty(contactModel.Phone))
                        {
                            var duplicateExists = _context.Contacts.Any(c =>
                                c.FirstName.ToLower() == contactModel.FirstName.ToLower() &&
                                c.LastName.ToLower() == contactModel.LastName.ToLower() &&
                                (c.Email.ToLower() == contactModel.Email.ToLower() || c.Phone == contactModel.Phone));

                            if (duplicateExists)
                            {
                                ModelState.AddModelError("Contacts", $"The contact {contactModel.FirstName} {contactModel.LastName} already exists. Please enter a new one.");
                                TempData["error"] = $"The contact {contactModel.FirstName} {contactModel.LastName} already exists. Please enter a new one.";
                                model.AvailableIndustries = _context.Industries.Select(i => new IndustryViewModel { ID = i.ID, Name = i.Name, NAICS = i.NAICS }).ToList();
                                model.AvailableMembershipTypes = _context.MembershipTypes.Select(mt => new MembershipTypeViewModel { ID = mt.ID, Name = mt.Name }).ToList();
                                return View(model);
                            }
                        }

                        await AddContactPicture(contactModel, contactPicture);
                        var contact = new Contact
                        {
                            FirstName = contactModel.FirstName,
                            LastName = contactModel.LastName,
                            Email = contactModel.Email,
                            Phone = contactModel.Phone,
                            ContactType = contactModel.ContactType,
                            ContactPhoto = contactModel.ContactPhoto,
                            ContactThumbnail = contactModel.ContactThumbnail
                        };
                        member.MemberContacts.Add(new MemberContact { Contact = contact });
                    }

                    // Add Selected Industries
                    foreach (var industryId in model.SelectedIndustryIds)
                    {
                        var industry = await _context.Industries.FindAsync(industryId);
                        if (industry != null)
                        {
                            member.IndustryMembers.Add(new MemberIndustry { Industry = industry });
                        }
                    }

                    // Save to Database
                    _context.Members.Add(member);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Member Created successfully";
                    TempData["success"] = "Member Created successfully";

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Error creating member: {ex.Message}");
                    // Log the exception
                    Console.WriteLine($"Error creating member: {ex.Message}");
                }
            }
            else
            {
                // Log validation errors for debugging
                foreach (var modelState in ModelState.Values)
                {
                    foreach (var error in modelState.Errors)
                    {
                        Console.WriteLine($"Validation error: {error.ErrorMessage}");
                    }
                }
            }

            // If we get here, something failed, redisplay form
            // Repopulate available industries
            model.AvailableIndustries = _context.Industries
                .Select(industry => new IndustryViewModel
                {
                    ID = industry.ID,
                    Name = industry.Name,
                    NAICS = industry.NAICS
                }).ToList();

            // Reload membership types
            model.AvailableMembershipTypes = _context.MembershipTypes
                .Select(m => new MembershipTypeViewModel
                {
                    ID = m.ID,
                    Name = m.Name
                }).ToList();
                
            // Add a flag to indicate validation errors
            ViewBag.HasValidationErrors = true;
            
            // Store the current step in TempData to help the client-side script restore the correct step
            int currentStep = 0;
            
            // Determine which step has validation errors
            if (ModelState["MemberName"]?.Errors.Count > 0 || 
                ModelState["CompanySize"]?.Errors.Count > 0 || 
                ModelState["CompanyWebsite"]?.Errors.Count > 0 || 
                ModelState["ContactedBy"]?.Errors.Count > 0 ||
                ModelState["SelectedMembershipTypeIDs"]?.Errors.Count > 0)
            {
                currentStep = 0; // Step 1 has errors
            }
            else if (ModelState["MemberSince"]?.Errors.Count > 0 || 
                     ModelState["LastContactDate"]?.Errors.Count > 0 || 
                     ModelState["Notes"]?.Errors.Count > 0)
            {
                currentStep = 1; // Step 2 has errors
            }
            else if (ModelState["Addresses"]?.Errors.Count > 0)
            {
                currentStep = 2; // Step 3 has errors
            }
            else if (ModelState["Contacts"]?.Errors.Count > 0 || 
                     ModelState["SelectedIndustryIds"]?.Errors.Count > 0)
            {
                currentStep = 3; // Step 4 has errors
            }
            
            // Store the current step in session
            HttpContext.Session.SetInt32("currentFormStep", currentStep);
            
            return View(model);
        }

        //GET Edit
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberPhoto)
                .Include(m => m.MemberThumbnail)
                .Include(m => m.MemberContacts)
                .ThenInclude(mc => mc.Contact)
                .Include(m => m.MemberMembershipTypes) // Include the join table
                .ThenInclude(mmt => mmt.MembershipType)
                .Include(m => m.MemberAddresses) // Ensure we include addresses
                .ThenInclude(ma => ma.Address) // Include the Address entity
                .Include(m => m.IndustryMembers)
                .ThenInclude(im => im.Industry)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (member == null)
            {
                return NotFound();
            }

            // Map Member to ViewModel
            var model = new MemberCreateViewModel
            {
                ID = member.ID,
                MemberName = member.MemberName,
                ContactedBy = member.ContactedBy,
                CompanySize = member.CompanySize,
                CompanyWebsite = member.CompanyWebsite,
                MemberSince = member.MemberSince,
                LastContactDate = member.LastContactDate,
                Notes = member.Notes,
                MembershipStatus = member.MembershipStatus,
                MemberPhoto = member.MemberPhoto,
                MemberThumbnail = member.MemberThumbnail,
                Addresses = member.MemberAddresses.Select(ma => new AddressViewModel
                {
                    ID = ma.Address.ID, // Ensure ID is mapped
                    Street = ma.Address.Street,
                    City = ma.Address.City,
                    Province = ma.Address.Province,
                    PostalCode = ma.Address.PostalCode,
                    AddressType = ma.AddressType // Map address type
                }).ToList(),
                Contacts = member.MemberContacts.Select(mc => new ContactViewModel
                {
                    ID = mc.Contact.ID, // Ensure ID is included
                    FirstName = mc.Contact.FirstName,
                    LastName = mc.Contact.LastName,
                    Email = mc.Contact.Email,
                    Phone = mc.Contact.Phone,
                    ContactType = mc.ContactType,
                    ContactPhoto = mc.Contact.ContactPhoto,
                    ContactThumbnail = mc.Contact.ContactThumbnail
                }).ToList(),
                SelectedIndustryIds = member.IndustryMembers.Select(im => im.IndustryID).ToList(),
                SelectedMembershipTypeIDs = member.MemberMembershipTypes
                    .Select(mmt => mmt.MembershipTypeID)
                    .ToList(),
                AvailableIndustries = _context.Industries
                    .Select(industry => new IndustryViewModel
                    {
                        ID = industry.ID,
                        Name = industry.Name,
                        NAICS = industry.NAICS
                    }).ToList(),
                AvailableMembershipTypes = _context.MembershipTypes
                    .Select(mt => new MembershipTypeViewModel
                    {
                        ID = mt.ID,
                        Name = mt.Name
                    }).ToList()
            };

            return View(model);
        }

        // POST: Member/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, MemberCreateViewModel model, string? chkRemoveMemberImage, IFormFile? memberPicture)
        {
            if (id != model.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Check for duplicate member name (excluding the current member)
                if (!string.IsNullOrEmpty(model.MemberName))
                {
                    var existingMember = await _context.Members
                        .FirstOrDefaultAsync(m => m.ID != id && 
                                                m.MemberName.ToLower() == model.MemberName.ToLower());
                    
                    if (existingMember != null)
                    {
                        ModelState.AddModelError("MemberName", "A member with this name already exists. Please use a different name.");
                        TempData["error"] = "A member with this name already exists.";
                        
                        // Repopulate available industries and membership types
                        model.AvailableIndustries = _context.Industries
                            .Select(industry => new IndustryViewModel
                            {
                                ID = industry.ID,
                                Name = industry.Name,
                                NAICS = industry.NAICS
                            }).ToList();
                        
                        model.AvailableMembershipTypes = _context.MembershipTypes
                            .Select(mt => new MembershipTypeViewModel
                            {
                                ID = mt.ID,
                                Name = mt.Name
                            }).ToList();
                        
                        return View(model);
                    }
                }

                try
                {
                    // Fetch the existing member from the database with all related entities
                    var member = await _context.Members
                        .Include(m => m.MemberPhoto)
                        .Include(m => m.MemberThumbnail)
                        .Include(m => m.MemberMembershipTypes)
                        .Include(m => m.IndustryMembers)
                        .Include(m => m.MemberContacts)
                            .ThenInclude(mc => mc.Contact)
                                .ThenInclude(c => c.ContactPhoto)
                        .Include(m => m.MemberContacts)
                            .ThenInclude(mc => mc.Contact)
                                .ThenInclude(c => c.ContactThumbnail)
                        .Include(m => m.MemberAddresses)
                            .ThenInclude(ma => ma.Address)
                        .FirstOrDefaultAsync(m => m.ID == id);

                    if (member == null)
                    {
                        return NotFound();
                    }

                    // Process member picture
                    if (chkRemoveMemberImage == "on")
                    {
                        // Remove existing photo and thumbnail
                        if (member.MemberPhoto != null)
                        {
                            _context.MemberPhotos.Remove(member.MemberPhoto);
                            member.MemberPhoto = null;
                        }
                        if (member.MemberThumbnail != null) 
                        {
                            _context.MemberThumbnails.Remove(member.MemberThumbnail);
                            member.MemberThumbnail = null;
                        }
                    }
                    else if (memberPicture != null)
                    {
                        // Update photo with new upload
                        await AddMemberPicture(model, memberPicture);
                        // Update member with new photo references
                        if (model.MemberPhoto != null)
                        {
                            if (member.MemberPhoto == null)
                            {
                                member.MemberPhoto = new MemberPhoto
                                {
                                    Content = model.MemberPhoto.Content,
                                    MimeType = model.MemberPhoto.MimeType
                                };
                            }
                            else
                            {
                                member.MemberPhoto.Content = model.MemberPhoto.Content;
                                member.MemberPhoto.MimeType = model.MemberPhoto.MimeType;
                            }
                        }
                        if (model.MemberThumbnail != null)
                        {
                            if (member.MemberThumbnail == null)
                            {
                                member.MemberThumbnail = new MemberThumbnail
                                {
                                    Content = model.MemberThumbnail.Content,
                                    MimeType = model.MemberThumbnail.MimeType
                                };
                            }
                            else
                            {
                                member.MemberThumbnail.Content = model.MemberThumbnail.Content;
                                member.MemberThumbnail.MimeType = model.MemberThumbnail.MimeType;
                            }
                        }
                    }

                    // Update Member properties
                    member.MemberName = model.MemberName;
                    member.ContactedBy = model.ContactedBy;
                    member.CompanySize = model.CompanySize;
                    member.CompanyWebsite = model.CompanyWebsite;
                    member.MemberSince = model.MemberSince;
                    member.LastContactDate = model.LastContactDate;
                    member.Notes = model.Notes;
                    member.MembershipStatus = model.MembershipStatus;

                    // Update Industry memberships - clear existing and add selected ones
                    member.IndustryMembers.Clear();
                    foreach (var industryId in model.SelectedIndustryIds)
                    {
                        member.IndustryMembers.Add(new MemberIndustry { 
                            MemberID = member.ID,
                            IndustryID = industryId
                        });
                    }

                    // Update Membership Types - clear existing and add selected ones
                    member.MemberMembershipTypes.Clear();
                    foreach (var membershipTypeId in model.SelectedMembershipTypeIDs)
                    {
                        member.MemberMembershipTypes.Add(new MemberMembershipType { 
                            MemberID = member.ID,
                            MembershipTypeID = membershipTypeId
                        });
                    }

                    // Update only if we have existing contacts to manage
                    // We're NOT clearing contacts here since they're managed separately through the Contacts controller
                    // The Edit view only shows existing contacts but doesn't allow creating new ones directly
                    
                    // Save changes
                    _context.Update(member);
                    await _context.SaveChangesAsync();

                    TempData["success"] = "Member edited successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!MemberExists(model.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        TempData["error"] = "An error occurred. Failed to edit the member.";
                        Console.WriteLine($"Error updating member: {ex.Message}");
                    }
                }
                catch (Exception ex)
                {
                    TempData["error"] = "An error occurred. Failed to edit the member.";
                    Console.WriteLine($"Error updating member: {ex.Message}");
                }
            }

            // If validation fails, return the form with validation messages
            model.AvailableIndustries = _context.Industries
                .Select(industry => new IndustryViewModel
                {
                    ID = industry.ID,
                    Name = industry.Name,
                    NAICS = industry.NAICS
                }).ToList();

            model.AvailableMembershipTypes = _context.MembershipTypes
                .Select(m => new MembershipTypeViewModel
                {
                    ID = m.ID,
                    Name = m.Name
                }).ToList();

            return View(model);
        }



        // GET: Member/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.ID == id);
            if (member == null)
            {
                return NotFound();
            }

            return View(member);
        }

        // POST: Member/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members
                .FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(string email, string phone, int? excludeContactId = null)
        {
            // If both email and phone are empty, there's nothing to check
            if (string.IsNullOrEmpty(email) && string.IsNullOrEmpty(phone))
                return false;

            var query = _context.Contacts.AsQueryable();

            if (excludeContactId.HasValue)
            {
                query = query.Where(c => c.ID != excludeContactId.Value);
            }

            // Check for duplicates if either email OR phone match (when they're not empty)
            return query.Any(c =>
                (!string.IsNullOrEmpty(email) && c.Email == email) ||
                (!string.IsNullOrEmpty(phone) && c.Phone == phone));
        }


        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.ID == id);
        }

        private async Task AddMemberPicture(MemberCreateViewModel member, IFormFile thePicture)
        {
            if (thePicture != null)
            {
                string mimeType = thePicture.ContentType;
                long fileLength = thePicture.Length;
                if (!(mimeType == "" || fileLength == 0)) // Check if file exists
                {
                    if (mimeType.Contains("image"))
                    {
                        using var memoryStream = new MemoryStream();
                        await thePicture.CopyToAsync(memoryStream);
                        var pictureArray = memoryStream.ToArray(); // Convert to byte array

                        // Retrieve existing MemberPhoto and MemberThumbnail from the database
                        var existingPhoto = await _context.MemberPhotos
                                                          .Where(p => p.MemberID == member.ID)
                                                          .FirstOrDefaultAsync();
                        var existingThumbnail = await _context.MemberThumbnails
                                                              .Where(t => t.MemberID == member.ID)
                                                              .FirstOrDefaultAsync();

                        if (existingPhoto != null)
                        {
                            // Update the existing photo instead of creating a new one
                            existingPhoto.Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600);
                        }
                        else
                        {
                            member.MemberPhoto = new MemberPhoto
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 500, 600),
                                MimeType = "image/webp"
                            };
                        }

                        if (existingThumbnail != null)
                        {
                            // Update existing thumbnail
                            existingThumbnail.Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70);
                        }
                        else
                        {
                            member.MemberThumbnail = new MemberThumbnail
                            {
                                Content = ResizeImage.ShrinkImageWebp(pictureArray, 50, 70),
                                MimeType = "image/webp"
                            };
                        }
                    }
                }
            }
        }

        private async Task AddContactPicture(ContactViewModel contact, IFormFile thePicture)
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
        public IActionResult ExportMembers()
        {
            var model = new MemberExportViewModel
            {
                Members = _context.Members.ToList() // Fetch members from the database
            };
            return View(model); // Returns the ExportMembers.cshtml view
        }

        [HttpPost]
        public IActionResult DownloadMembers([FromForm] MemberExportOptions options)
        {
            if (options.SelectedFields == null || !options.SelectedFields.Any())
            {
                return BadRequest("Please select at least one field to export.");
            }

            //IQueryable<Member> membersQuery = _context.Members.AsQueryable();
            IQueryable<Member> membersQuery = _context.Members
             .Include(m => m.MemberContacts)
             .Include(m => m.IndustryMembers).ThenInclude(im => im.Industry)
             .Include(m => m.MemberAddresses).ThenInclude(a => a.Address)
             .AsQueryable();

            // If "Download All" is selected, fetch all members
            if (!options.DownloadAll)
            {
                // Otherwise, filter by selected members
                if (options.SelectedMemberIds != null && options.SelectedMemberIds.Any())
                {
                    membersQuery = membersQuery.Where(m => options.SelectedMemberIds.Contains(m.ID));
                }
                else
                {
                    return BadRequest("No members selected.");
                }
            }

            var members = membersQuery.ToList();

            // Set the license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excel = new ExcelPackage())
            {
                var workSheet = excel.Workbook.Worksheets.Add("Members");

                int column = 1;
                Dictionary<string, int> fieldMapping = new Dictionary<string, int>();

                // Add headers properly
                foreach (var field in options.SelectedFields)
                {
                    workSheet.Cells[3, column].Value = field;  // <-- Row 3 for column headings
                    workSheet.Cells[3, column].Style.Font.Bold = true;
                    fieldMapping[field] = column;
                    column++;
                }

                int row = 4; // Data starts from row 4
                foreach (var member in members)
                {
                    column = 1;
                    foreach (var field in options.SelectedFields)
                    {
                        switch (field)
                        {
                            case "MemberName":
                                workSheet.Cells[row, column].Value = member.MemberName;
                                break;
                            case "CompanySize":
                                workSheet.Cells[row, column].Value = member.CompanySize.ToString();
                                break;
                            case "Website":
                                workSheet.Cells[row, column].Value = member.CompanyWebsite;
                                break;
                            case "MembershipStatus":
                                workSheet.Cells[row, column].Value = member.MembershipStatus.ToString();
                                break;
                            case "ContactedBy":
                                workSheet.Cells[row, column].Value = member.ContactedBy.ToString();
                                break;
                            case "MemberSince":
                                workSheet.Cells[row, column].Value = member.MemberSince.ToShortDateString();
                                workSheet.Column(column).Style.Numberformat.Format = "yyyy-mm-dd";
                                break;
                            case "Notes":
                                workSheet.Cells[row, column].Value = member.Notes;
                                break;
                            case "Industries":
                                workSheet.Cells[row, column].Value = string.Join(", ",
                                    member.IndustryMembers.Select(im => im.Industry.NAICS)); // List industries
                                break;
                            case "Address":
                                var address = member.MemberAddresses.FirstOrDefault()?.Address;
                                if (address != null)
                                {
                                    workSheet.Cells[row, column].Value = address.Street;
                                }
                                else
                                {
                                    workSheet.Cells[row, column].Value = "N/A";
                                }
                                break;
                            case "City":
                                var addressCity = member.MemberAddresses.FirstOrDefault()?.Address;
                                workSheet.Cells[row, column].Value = addressCity != null ? addressCity.City : "N/A";
                                break;
                            case "Province":
                                var addressProvince = member.MemberAddresses.FirstOrDefault()?.Address;
                                workSheet.Cells[row, column].Value = addressProvince != null ? addressProvince.Province.ToString() : "N/A";
                                break;
                            case "PostalCode":
                                var addressPostal = member.MemberAddresses.FirstOrDefault()?.Address;
                                workSheet.Cells[row, column].Value = addressPostal != null && !string.IsNullOrEmpty(addressPostal.PostalCode) 
                                    ? addressPostal.PostalCode : "N/A";
                                break;
                        }
                        column++;
                    }
                    row++;
                }

                workSheet.Cells.AutoFitColumns();

                // Fix title and timestamp formatting
                workSheet.Cells[1, 1].Value = "Member Report";
                using (ExcelRange title = workSheet.Cells[1, 1, 1, options.SelectedFields.Count])
                {
                    title.Merge = true;
                    title.Style.Font.Bold = true;
                    title.Style.Font.Size = 18;
                    title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    title.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    title.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(48, 84, 150));
                    title.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }

                // Fix "Created Date" placement (move it below title)
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                workSheet.Cells[2, 1].Value = "Created: " + localDate.ToString("MMMM d, yyyy h:mm tt");
                using (ExcelRange dateRange = workSheet.Cells[2, 1, 2, options.SelectedFields.Count])
                {
                    dateRange.Merge = true;
                    dateRange.Style.Font.Bold = true;
                    dateRange.Style.Font.Size = 11;
                    dateRange.Style.Font.Italic = true;
                    dateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    dateRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    dateRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }

                // Style the headers
                using (ExcelRange headers = workSheet.Cells[3, 1, 3, options.SelectedFields.Count])
                {
                    headers.Style.Font.Bold = true;
                    headers.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    headers.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(220, 230, 242));
                    headers.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                    headers.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                    headers.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                    headers.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                    headers.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Format the data range
                int lastRow = 3 + members.Count;
                int lastCol = options.SelectedFields.Count;
                
                if (members.Count > 0)
                {
                    // Apply alternating row colors in the data area
                    for (int dataRow = 4; dataRow <= lastRow; dataRow++)
                    {
                        using (ExcelRange rowRange = workSheet.Cells[dataRow, 1, dataRow, lastCol])
                        {
                            // Apply light gray to even rows
                            if (dataRow % 2 == 0)
                            {
                                rowRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                                rowRange.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(242, 242, 242));
                            }
                            
                            // Apply borders to all data cells
                            rowRange.Style.Border.Top.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Left.Style = ExcelBorderStyle.Thin;
                            rowRange.Style.Border.Right.Style = ExcelBorderStyle.Thin;
                            
                            // Vertical alignment
                            rowRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        }
                    }
                    
                    // Format specific data types and cell alignment
                    for (column = 1; column <= lastCol; column++)
                    {
                        string fieldName = options.SelectedFields[column - 1];
                        
                        // Set column-specific formatting
                        if (fieldName == "MemberSince" || fieldName.Contains("Date"))
                        {
                            // Format dates consistently
                            workSheet.Column(column).Style.Numberformat.Format = "yyyy-mm-dd";
                            workSheet.Column(column).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                        else if (fieldName == "Website")
                        {
                            // Format URLs
                            using (ExcelRange urlRange = workSheet.Cells[4, column, lastRow, column])
                            {
                                urlRange.Style.Font.Color.SetColor(System.Drawing.Color.Blue);
                                urlRange.Style.Font.UnderLine = true;
                            }
                        }
                        else if (fieldName == "Notes")
                        {
                            // Format multi-line text fields
                            workSheet.Column(column).Style.WrapText = true;
                            workSheet.Column(column).Width = 40; // Fixed width for notes
                        }
                        else if (fieldName == "MembershipStatus")
                        {
                            // Center status values
                            workSheet.Column(column).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        }
                    }
                }
                
                // Add freeze panes to keep headers visible when scrolling
                workSheet.View.FreezePanes(4, 1);
                
                // Adjust column widths but ensure minimum and maximum widths
                workSheet.Cells.AutoFitColumns(15, 80);

                try
                {
                    Byte[] fileBytes = excel.GetAsByteArray();
                    string filename = "Members.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(fileBytes, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not generate the file.");
                }
            }
        }

        // GET: Member/ExportMemberDetails/5
        [HttpGet]
        public async Task<IActionResult> ExportMemberDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var member = await _context.Members
                .Include(m => m.MemberContacts)
                    .ThenInclude(mc => mc.Contact)
                .Include(m => m.IndustryMembers)
                    .ThenInclude(im => im.Industry)
                .Include(m => m.MemberAddresses)
                    .ThenInclude(ma => ma.Address)
                .Include(m => m.MemberMembershipTypes)
                    .ThenInclude(mmt => mmt.MembershipType)
                .FirstOrDefaultAsync(m => m.ID == id);

            if (member == null)
            {
                return NotFound();
            }

            // Set the license context
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (ExcelPackage excel = new ExcelPackage())
            {
                // Create the main worksheet for general information
                var mainSheet = excel.Workbook.Worksheets.Add($"{member.MemberName} - Details");
                
                // Set title
                mainSheet.Cells[1, 1].Value = $"{member.MemberName} - Member Details";
                using (ExcelRange title = mainSheet.Cells[1, 1, 1, 5])
                {
                    title.Merge = true;
                    title.Style.Font.Bold = true;
                    title.Style.Font.Size = 18;
                    title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Add timestamp
                DateTime utcDate = DateTime.UtcNow;
                TimeZoneInfo esTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                DateTime localDate = TimeZoneInfo.ConvertTimeFromUtc(utcDate, esTimeZone);
                mainSheet.Cells[2, 1].Value = "Exported: " + localDate.ToString("MMMM d, yyyy h:mm tt");
                using (ExcelRange dateRange = mainSheet.Cells[2, 1, 2, 5])
                {
                    dateRange.Merge = true;
                    dateRange.Style.Font.Italic = true;
                    dateRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                // Add member information
                int row = 4; // Start from row 4

                // Section header - Company Information
                mainSheet.Cells[row, 1].Value = "Company Information";
                using (ExcelRange sectionHeader = mainSheet.Cells[row, 1, row, 2])
                {
                    sectionHeader.Merge = true;
                    sectionHeader.Style.Font.Bold = true;
                    sectionHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sectionHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }
                row++;

                // Member Name
                mainSheet.Cells[row, 1].Value = "Member Name:";
                mainSheet.Cells[row, 2].Value = member.MemberName;
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Company Size
                mainSheet.Cells[row, 1].Value = "Company Size:";
                mainSheet.Cells[row, 2].Value = member.CompanySize.ToString();
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Website
                mainSheet.Cells[row, 1].Value = "Website:";
                mainSheet.Cells[row, 2].Value = member.CompanyWebsite;
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Industries
                mainSheet.Cells[row, 1].Value = "Industries:";
                mainSheet.Cells[row, 2].Value = string.Join(", ", member.IndustryMembers.Select(im => im.Industry.Name));
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row += 2; // Add an extra row for spacing

                // Section header - Membership Information
                mainSheet.Cells[row, 1].Value = "Membership Information";
                using (ExcelRange sectionHeader = mainSheet.Cells[row, 1, row, 2])
                {
                    sectionHeader.Merge = true;
                    sectionHeader.Style.Font.Bold = true;
                    sectionHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sectionHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }
                row++;

                // Membership Types
                mainSheet.Cells[row, 1].Value = "Membership Types:";
                mainSheet.Cells[row, 2].Value = string.Join(", ", member.MemberMembershipTypes.Select(mmt => mmt.MembershipType.Name));
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Membership Status
                mainSheet.Cells[row, 1].Value = "Status:";
                mainSheet.Cells[row, 2].Value = member.MembershipStatus.ToString();
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Member Since
                mainSheet.Cells[row, 1].Value = "Member Since:";
                mainSheet.Cells[row, 2].Value = member.MemberSince.ToString("MMMM d, yyyy");
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Last Contact Date
                mainSheet.Cells[row, 1].Value = "Last Contact Date:";
                mainSheet.Cells[row, 2].Value = member.LastContactDate?.ToString("MMMM d, yyyy") ?? "Not contacted yet";
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row++;

                // Contacted By
                mainSheet.Cells[row, 1].Value = "Contacted By:";
                mainSheet.Cells[row, 2].Value = member.ContactedBy ?? "N/A";
                mainSheet.Cells[row, 1].Style.Font.Bold = true;
                row += 2; // Add an extra row for spacing

                // Section header - Notes
                mainSheet.Cells[row, 1].Value = "Notes";
                using (ExcelRange sectionHeader = mainSheet.Cells[row, 1, row, 2])
                {
                    sectionHeader.Merge = true;
                    sectionHeader.Style.Font.Bold = true;
                    sectionHeader.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    sectionHeader.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                }
                row++;

                // Notes content
                mainSheet.Cells[row, 1].Value = member.Notes ?? "No notes available";
                using (ExcelRange notesRange = mainSheet.Cells[row, 1, row, 5])
                {
                    notesRange.Merge = true;
                    notesRange.Style.WrapText = true;
                }
                row += 2; // Add an extra row for spacing

                // Create a separate worksheet for contacts
                if (member.MemberContacts.Any())
                {
                    var contactsSheet = excel.Workbook.Worksheets.Add("Contacts");
                    
                    // Add title
                    contactsSheet.Cells[1, 1].Value = "Member Contacts";
                    using (ExcelRange title = contactsSheet.Cells[1, 1, 1, 5])
                    {
                        title.Merge = true;
                        title.Style.Font.Bold = true;
                        title.Style.Font.Size = 16;
                        title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    
                    // Add headers
                    int contactRow = 3;
                    contactsSheet.Cells[contactRow, 1].Value = "Type";
                    contactsSheet.Cells[contactRow, 2].Value = "Name";
                    contactsSheet.Cells[contactRow, 3].Value = "Email";
                    contactsSheet.Cells[contactRow, 4].Value = "Phone";
                    
                    // Style headers
                    for (int i = 1; i <= 4; i++)
                    {
                        contactsSheet.Cells[contactRow, i].Style.Font.Bold = true;
                        contactsSheet.Cells[contactRow, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        contactsSheet.Cells[contactRow, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    
                    contactRow++;
                    
                    // Add contacts
                    foreach (var memberContact in member.MemberContacts)
                    {
                        var contact = memberContact.Contact;
                        contactsSheet.Cells[contactRow, 1].Value = memberContact.ContactType.ToString();
                        contactsSheet.Cells[contactRow, 2].Value = $"{contact.FirstName} {contact.LastName}";
                        contactsSheet.Cells[contactRow, 3].Value = contact.Email;
                        contactsSheet.Cells[contactRow, 4].Value = contact.Phone;
                        contactRow++;
                    }
                    
                    contactsSheet.Cells.AutoFitColumns();
                }

                // Create a separate worksheet for addresses
                if (member.MemberAddresses.Any())
                {
                    var addressesSheet = excel.Workbook.Worksheets.Add("Addresses");
                    
                    // Add title
                    addressesSheet.Cells[1, 1].Value = "Member Addresses";
                    using (ExcelRange title = addressesSheet.Cells[1, 1, 1, 5])
                    {
                        title.Merge = true;
                        title.Style.Font.Bold = true;
                        title.Style.Font.Size = 16;
                        title.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                    
                    // Add headers
                    int addressRow = 3;
                    addressesSheet.Cells[addressRow, 1].Value = "Type";
                    addressesSheet.Cells[addressRow, 2].Value = "Street";
                    addressesSheet.Cells[addressRow, 3].Value = "City";
                    addressesSheet.Cells[addressRow, 4].Value = "Province";
                    addressesSheet.Cells[addressRow, 5].Value = "Postal Code";
                    
                    // Style headers
                    for (int i = 1; i <= 5; i++)
                    {
                        addressesSheet.Cells[addressRow, i].Style.Font.Bold = true;
                        addressesSheet.Cells[addressRow, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        addressesSheet.Cells[addressRow, i].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGray);
                    }
                    
                    addressRow++;
                    
                    // Add addresses
                    foreach (var memberAddress in member.MemberAddresses)
                    {
                        var address = memberAddress.Address;
                        addressesSheet.Cells[addressRow, 1].Value = memberAddress.AddressType.ToString();
                        addressesSheet.Cells[addressRow, 2].Value = address.Street;
                        addressesSheet.Cells[addressRow, 3].Value = address.City;
                        addressesSheet.Cells[addressRow, 4].Value = address.Province.ToString();
                        addressesSheet.Cells[addressRow, 5].Value = address.PostalCode;
                        addressRow++;
                    }
                    
                    addressesSheet.Cells.AutoFitColumns();
                }

                // Auto-fit columns in main sheet
                mainSheet.Cells.AutoFitColumns();

                try
                {
                    Byte[] fileBytes = excel.GetAsByteArray();
                    string filename = $"{member.MemberName}_Details.xlsx";
                    string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    return File(fileBytes, mimeType, filename);
                }
                catch (Exception)
                {
                    return BadRequest("Could not generate the file.");
                }
            }
        }
    }
}