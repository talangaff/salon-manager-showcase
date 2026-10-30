using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using salon_manager.Data;
using salon_manager.Models;

namespace salon_manager.Controllers;

[Authorize]
public class ColorsController : Controller
{
    private readonly TipContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ColorsController(TipContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var userId = _userManager.GetUserId(User);
        var query = _context.Clients
            .Include(c => c.Colors)
            .Include(c => c.Toners)
            .Where(c => c.UserId == userId);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(c =>
                c.FirstName.ToLower().Contains(term) ||
                c.LastName.ToLower().Contains(term) ||
                (c.FirstName + " " + c.LastName).ToLower().Contains(term));
        }

        var clients = await query
            .OrderBy(c => c.LastName)
            .ThenBy(c => c.FirstName)
            .ToListAsync();

        ViewBag.Search = search;
        return View(clients);
    }

    public IActionResult Create()
    {
        return View(new ClientFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClientFormViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var userId = _userManager.GetUserId(User);
        var client = new Client
        {
            FirstName = vm.FirstName,
            LastName = vm.LastName,
            UserId = userId
        };

        foreach (var color in vm.Colors.Where(c => !string.IsNullOrWhiteSpace(c)))
            client.Colors.Add(new ClientColor { Color = color.Trim() });

        foreach (var toner in vm.Toners.Where(t => !string.IsNullOrWhiteSpace(t)))
            client.Toners.Add(new ClientToner { Toner = toner.Trim() });

        _context.Clients.Add(client);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var userId = _userManager.GetUserId(User);
        var client = await _context.Clients
            .Include(c => c.Colors)
            .Include(c => c.Toners)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (client == null) return NotFound();

        var vm = new ClientFormViewModel
        {
            Id = client.Id,
            FirstName = client.FirstName,
            LastName = client.LastName,
            Colors = client.Colors.Select(c => c.Color).ToList(),
            Toners = client.Toners.Select(t => t.Toner).ToList()
        };

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, ClientFormViewModel vm)
    {
        if (id != vm.Id) return NotFound();

        if (!ModelState.IsValid)
            return View(vm);

        var userId = _userManager.GetUserId(User);
        var client = await _context.Clients
            .Include(c => c.Colors)
            .Include(c => c.Toners)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (client == null) return NotFound();

        client.FirstName = vm.FirstName;
        client.LastName = vm.LastName;

        _context.ClientColors.RemoveRange(client.Colors);
        client.Colors.Clear();

        foreach (var color in vm.Colors.Where(c => !string.IsNullOrWhiteSpace(c)))
            client.Colors.Add(new ClientColor { Color = color.Trim() });

        _context.ClientToners.RemoveRange(client.Toners);
        client.Toners.Clear();

        foreach (var toner in vm.Toners.Where(t => !string.IsNullOrWhiteSpace(t)))
            client.Toners.Add(new ClientToner { Toner = toner.Trim() });

        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = _userManager.GetUserId(User);
        var client = await _context.Clients
            .Include(c => c.Colors)
            .Include(c => c.Toners)
            .FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);

        if (client != null)
        {
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }
}
