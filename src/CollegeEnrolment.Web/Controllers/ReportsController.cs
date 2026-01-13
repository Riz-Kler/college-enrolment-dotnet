using CollegeEnrolment.Data;
using CollegeEnrolment.Data.Queries;
using Microsoft.AspNetCore.Mvc;

namespace CollegeEnrolment.Web.Controllers;

public sealed class ReportsController : Controller
{
    private readonly AppDbContext _db;

    public ReportsController(AppDbContext db) => _db = db;

    [HttpGet("/reports/course-capacity")]
    public async Task<IActionResult> CourseCapacity(string year = "2025/26", CancellationToken ct = default)
    {
        var rows = await _db.GetCourseCapacityReportAsync(year, ct);
        ViewBag.Year = year;
        return View(rows);
    }
}
