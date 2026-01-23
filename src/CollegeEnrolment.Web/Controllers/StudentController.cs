using CollegeEnrolment.Data;
using CollegeEnrolment.Domain.Entities;
using CollegeEnrolment.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CollegeEnrolment.Web.Controllers;

public sealed class StudentsController : Controller
{
    private readonly AppDbContext _db;

    public StudentsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Index(string? q, CancellationToken ct)
    {
        var query = _db.Students.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            q = q.Trim();
            query = query.Where(s =>
                s.StudentNumber.Contains(q) ||
                s.FirstName.Contains(q) ||
                s.LastName.Contains(q) ||
                s.Email.Contains(q));
        }

        var items = await query
            .OrderBy(s => s.StudentNumber)
            .ToListAsync(ct);

        ViewBag.Q = q ?? "";
        return View(items);
    }

    public IActionResult Create() => View(new StudentCreateVm());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(StudentCreateVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(vm);

        var exists = await _db.Students.AnyAsync(s =>
            s.StudentNumber == vm.StudentNumber || s.Email == vm.Email, ct);

        if (exists)
        {
            ModelState.AddModelError("", "Student number or email already exists.");
            return View(vm);
        }

        var student = new Student
        {
            StudentNumber = vm.StudentNumber.Trim(),
            FirstName = vm.FirstName.Trim(),
            LastName = vm.LastName.Trim(),
            Email = vm.Email.Trim()
        };

        _db.Students.Add(student);

        _db.AuditLogs.Add(new AuditLog
        {
            Actor = User?.Identity?.Name ?? "staff",
            Action = "CREATE",
            EntityType = "Student",
            EntityId = student.StudentNumber,
            Details = $"{student.FirstName} {student.LastName} registered"
        });

        await _db.SaveChangesAsync(ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var student = await _db.Students
            .AsNoTracking()
            .Include(s => s.Enrolments)
            .ThenInclude(e => e.CourseOffering)
            .ThenInclude(o => o.Course)
            .FirstOrDefaultAsync(s => s.Id == id, ct);

        if (student is null) return NotFound();
        return View(student);
    }
}
