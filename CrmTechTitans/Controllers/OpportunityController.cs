using CrmTechTitans.Data;
using CrmTechTitans.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrmTechTitans.Controllers
{
    public class OpportunityController : Controller
    {
        private readonly CrmContext _context;

        public OpportunityController(CrmContext context)
        {
            _context = context;
        }

        // GET: Opportunity
        public async Task<IActionResult> Index()
        {
            return View(await _context.Opportunities.Include(c => c.MemberOpportunities)
            .ThenInclude(mc => mc.Member).ToListAsync());
        }

        // GET: Opportunity/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // GET: Opportunity/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Opportunity/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Title,Status,Description,Priority")] Opportunity opportunity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(opportunity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(opportunity);
        }

        // GET: Opportunity/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity == null)
            {
                return NotFound();
            }
            return View(opportunity);
        }

        // POST: Opportunity/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Status,Description,Priority")] Opportunity opportunity)
        {
            if (id != opportunity.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(opportunity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OpportunityExists(opportunity.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(opportunity);
        }

        // GET: Opportunity/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var opportunity = await _context.Opportunities
                .FirstOrDefaultAsync(m => m.ID == id);
            if (opportunity == null)
            {
                return NotFound();
            }

            return View(opportunity);
        }

        // POST: Opportunity/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opportunity = await _context.Opportunities.FindAsync(id);
            if (opportunity != null)
            {
                _context.Opportunities.Remove(opportunity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OpportunityExists(int id)
        {
            return _context.Opportunities.Any(e => e.ID == id);
        }
    }
}
