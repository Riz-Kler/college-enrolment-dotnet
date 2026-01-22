using CollegeEnrolment.Data;
using CollegeEnrolment.Domain.Entities;
using CollegeEnrolment.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

using CollegeEnrolment.Domain.Enums;
namespace CollegeEnrolment.Web.Controllers;

public sealed class EnrolmentsController : Controller
{
    private readonly AppDbContext _db;
    public EnrolmentsController(AppDbContext db) => _db = db;

    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var students = await _db.Students.AsNoTracking()
            .OrderBy(s => s.StudentNumber)
            .ToListAsync(ct);

        var offerings = await _db.CourseOfferings.AsNoTracking()
            .Include(o => o.Course)
            .OrderBy(o => o.Course.Code)
            .ToListAsync(ct);

        ViewBag.Students = students.Select(s => new SelectListItem($"{s.StudentNumber} - {s.FirstName} {s.LastName}", s.Id.ToString())).ToList();
        ViewBag.Offerings = offerings.Select(o => new SelectListItem($"{o.Course.Code} - {o.Course.Title} ({o.AcademicYear})", o.Id.ToString())).ToList();

        return View(new EnrolStudentVm());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EnrolStudentVm vm, CancellationToken ct)
    {
        if (!ModelState.IsValid) return await Create(ct);

        var offering = await _db.CourseOfferings
            .Include(o => o.Enrolments)
            .Include(o => o.Course)
            .FirstOrDefaultAsync(o => o.Id == vm.CourseOfferingId, ct);

        if (offering is null) return NotFound();

        var activeCount = offering.Enrolments.Count(e => e.Status == EnrolmentStatus.Active);
        if (activeCount >= offering.Capacity)
        {
            ModelState.AddModelError("", "This course offering is full.");
            return await Create(ct);
        }

        var already = await _db.Enrolments.AnyAsync(e =>
            e.StudentId == vm.StudentId &&
            e.CourseOfferingId == vm.CourseOfferingId, ct);

        if (already)
        {
            ModelState.AddModelError("", "Student is already linked to this offering.");
            return await Create(ct);
        }

        var enrolment = new Enrolment
        {
            StudentId = vm.StudentId,
            CourseOfferingId = vm.CourseOfferingId,
            Status = EnrolmentStatus.Active
        };

        _db.Enrolments.Add(enrolment);

        _db.AuditLogs.Add(new AuditLog
        {
            Actor = User?.Identity?.Name ?? "staff",
            Action = "ENROL",
            EntityType = "Enrolment",
            EntityId = $"{vm.StudentId}:{vm.CourseOfferingId}",
            Details = $"Enrolled into {offering.Course.Code} {offering.AcademicYear}"
        });

        await _db.SaveChangesAsync(ct);
        return RedirectToAction("Index", "Students");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id, CancellationToken ct)
    {
        var enrol = await _db.Enrolments
            .Include(e => e.CourseOffering).ThenInclude(o => o.Course)
            .FirstOrDefaultAsync(e => e.Id == id, ct);

        if (enrol is null) return NotFound();

        enrol.Status = EnrolmentStatus.Withdrawn;

        _db.AuditLogs.Add(new AuditLog
        {
            Actor = User?.Identity?.Name ?? "staff",
            Action = "WITHDRAW",
            EntityType = "Enrolment",
            EntityId = enrol.Id.ToString(),
            Details = $"Withdrawn from {enrol.CourseOffering.Course.Code} {enrol.CourseOffering.AcademicYear}"
        });

        await _db.SaveChangesAsync(ct);
        return RedirectToAction("Details", "Students", new { id = enrol.StudentId });
    }
}
