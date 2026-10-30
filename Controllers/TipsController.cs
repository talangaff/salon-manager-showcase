using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using salon_manager.Data;
using salon_manager.Models;

namespace salon_manager.Controllers;

[Authorize]
public class TipsController : Controller
{
    private readonly TipContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public TipsController(TipContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userManager.GetUserId(User);
        var tips = await _context.Tips
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
        return View(tips);
    }

    public IActionResult Create()
    {
        return View(new Tip { Date = DateTime.SpecifyKind(DateTime.Today, DateTimeKind.Utc) });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Tip tip)
    {
        if (tip.PaymentMethod == PaymentMethod.Cash)
        {
            if (tip.TotalCash == null || tip.ServiceCost == null)
            {
                ModelState.AddModelError("", "Total Cash and Service Cost are required for cash payments.");
                return View(tip);
            }
            tip.TipAmount = tip.TotalCash.Value - tip.ServiceCost.Value;
        }

        if (!ModelState.IsValid)
            return View(tip);

        tip.Date = DateTime.SpecifyKind(tip.Date, DateTimeKind.Utc);
        tip.UserId = _userManager.GetUserId(User);
        _context.Tips.Add(tip);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var tip = await _context.Tips.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (tip == null) return NotFound();
        return View(tip);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Tip tip)
    {
        if (id != tip.Id) return NotFound();

        var userId = _userManager.GetUserId(User);
        var existing = await _context.Tips.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (existing == null) return NotFound();

        if (tip.PaymentMethod == PaymentMethod.Cash)
        {
            if (tip.TotalCash == null || tip.ServiceCost == null)
            {
                ModelState.AddModelError("", "Total Cash and Service Cost are required for cash payments.");
                return View(tip);
            }
            tip.TipAmount = tip.TotalCash.Value - tip.ServiceCost.Value;
        }

        if (!ModelState.IsValid)
            return View(tip);

        existing.Date = DateTime.SpecifyKind(tip.Date, DateTimeKind.Utc);
        existing.PaymentMethod = tip.PaymentMethod;
        existing.TipAmount = tip.TipAmount;
        existing.TotalCash = tip.TotalCash;
        existing.ServiceCost = tip.ServiceCost;
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Totals(int? year)
    {
        var userId = _userManager.GetUserId(User);

        var availableYears = await _context.Tips
            .Where(t => t.UserId == userId)
            .Select(t => t.Date.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();

        decimal? total = null;
        string? chartLabelsJson = null;
        string? chartValuesJson = null;

        if (year.HasValue)
        {
            var query = _context.Tips.Where(t => t.UserId == userId && t.Date.Year == year.Value);
            total = await query.SumAsync(t => t.TipAmount);

            var monthlyTips = await query
                .GroupBy(t => t.Date.Month)
                .Select(g => new { Month = g.Key, Total = g.Sum(t => t.TipAmount) })
                .ToListAsync();

            var labels = Enumerable.Range(1, 12)
                .Select(m => CultureInfo.CurrentCulture.DateTimeFormat.GetAbbreviatedMonthName(m))
                .ToList();
            var values = Enumerable.Range(1, 12)
                .Select(m => monthlyTips.FirstOrDefault(x => x.Month == m)?.Total ?? 0m)
                .ToList();

            chartLabelsJson = System.Text.Json.JsonSerializer.Serialize(labels);
            chartValuesJson = System.Text.Json.JsonSerializer.Serialize(values);
        }

        ViewBag.Year = year;
        ViewBag.AvailableYears = availableYears;
        ViewBag.Total = total;
        ViewBag.ChartLabels = chartLabelsJson;
        ViewBag.ChartValues = chartValuesJson;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        var tip = await _context.Tips.FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);
        if (tip != null)
        {
            _context.Tips.Remove(tip);
            await _context.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
