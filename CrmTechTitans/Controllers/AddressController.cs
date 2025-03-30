using CrmTechTitans.Data;
using CrmTechTitans.Models;
using CrmTechTitans.Models.Enumerations;
using CrmTechTitans.Models.JoinTables;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

//Braydon Pew added 01.22.25
namespace CrmTechTitans.Controllers
{
    [Authorize]
    public class AddressController : Controller
    {
        private readonly CrmContext _context;

        public AddressController(CrmContext context)
        {
            _context = context;
        }

        // GET: Address
        public async Task<IActionResult> Index()
        {
            return View(await _context.Addresses.Include(m => m.MemberAddresses).ThenInclude(im => im.Member).ToListAsync());
        }

        // GET: Address/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                  .Include(a => a.MemberAddresses)
            .ThenInclude(ma => ma.Member) // Include the Member entity
        .FirstOrDefaultAsync(a => a.ID == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // GET: Address/Create
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public IActionResult Create(int? memberId, string? returnUrl)
        {
            ViewBag.MemberId = memberId;
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Address/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Create([Bind("ID,Street,City,Province,PostalCode,AddressType")] Address address, int? memberId, string? returnUrl)
        {
            if (ModelState.IsValid)
            {
                _context.Add(address);
                await _context.SaveChangesAsync();

                // If memberId is provided, create the relationship with the member
                if (memberId.HasValue)
                {
                    var memberAddress = new MemberAddress
                    {
                        MemberID = memberId.Value,
                        AddressID = address.ID,
                        AddressType = address.AddressType ?? AddressType.Billing
                    };
                    _context.MemberAddresses.Add(memberAddress);
                    await _context.SaveChangesAsync();
                }

                TempData["success"] = "Address created successfully!";
                TempData["message"] = "Address created successfully!";

                // Return to the specified URL or default to the Address Index
                if (!string.IsNullOrEmpty(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.MemberId = memberId;
            ViewBag.ReturnUrl = returnUrl;
            return View(address);
        }

        // GET: Address/Edit/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int? id, string? returnUrl)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses.FindAsync(id);
            if (address == null)
            {
                return NotFound();
            }
            
            ViewBag.ReturnUrl = returnUrl;
            return View(address);
        }

        // POST: Address/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Street,City,Province,PostalCode,AddressType")] Address address, string? returnUrl)
        {
            if (id != address.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(address);
                    await _context.SaveChangesAsync();
                    
                    TempData["success"] = "Address updated successfully!";
                    TempData["message"] = "Address updated successfully!";

                    // Return to the specified URL or default to the Address Index
                    if (!string.IsNullOrEmpty(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AddressExists(address.ID))
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
            return View(address);
        }

        // GET: Address/Delete/5
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var address = await _context.Addresses
                .FirstOrDefaultAsync(m => m.ID == id);
            if (address == null)
            {
                return NotFound();
            }

            return View(address);
        }

        // POST: Address/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = UserRoles.Administrator + "," + UserRoles.Editor)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var address = await _context.Addresses.FindAsync(id);
            if (address != null)
            {
                _context.Addresses.Remove(address);
            }

            await _context.SaveChangesAsync();
            TempData["success"] = "Address deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool AddressExists(int id)
        {
            return _context.Addresses.Any(e => e.ID == id);
        }
    }
}