using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using salon_manager.Data;
using salon_manager.Models;

namespace salon_manager.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly TipContext _context;

    public AdminController(UserManager<ApplicationUser> userManager, TipContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public IActionResult Index()
    {
        var users = _userManager.Users
            .Where(u => u.UserName != "Admin")
            .OrderBy(u => u.UserName)
            .ToList();
        return View(users);
    }

    [HttpGet]
    public IActionResult CreateUser()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateUser(CreateUserViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var user = new ApplicationUser
        {
            UserName = model.Username,
            MustChangePassword = true
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");
            TempData["Success"] = $"User '{model.Username}' created successfully.";
            return RedirectToAction(nameof(Index));
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null && user.UserName != "Admin")
            await _userManager.DeleteAsync(user);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> UserTips(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var tips = await _context.Tips
            .Where(t => t.UserId == id)
            .OrderByDescending(t => t.Date)
            .ToListAsync();

        ViewBag.Username = user.UserName;
        ViewBag.UserId = id;
        return View(tips);
    }

    public async Task<IActionResult> UserTotals(string id, int? year)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return NotFound();

        var availableYears = await _context.Tips
            .Where(t => t.UserId == id)
            .Select(t => t.Date.Year)
            .Distinct()
            .OrderByDescending(y => y)
            .ToListAsync();

        decimal? total = null;
        string? chartLabelsJson = null;
        string? chartValuesJson = null;

        if (year.HasValue)
        {
            var query = _context.Tips.Where(t => t.UserId == id && t.Date.Year == year.Value);
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

        ViewBag.Username = user.UserName;
        ViewBag.UserId = id;
        ViewBag.Year = year;
        ViewBag.AvailableYears = availableYears;
        ViewBag.Total = total;
        ViewBag.ChartLabels = chartLabelsJson;
        ViewBag.ChartValues = chartValuesJson;
        return View();
    }
}
