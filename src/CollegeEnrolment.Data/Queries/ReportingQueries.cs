using CollegeEnrolment.Data.Reports;
using Microsoft.EntityFrameworkCore;

namespace CollegeEnrolment.Data.Queries;

public static class ReportingQueries
{
    public static async Task<List<CourseCapacityReportRow>> GetCourseCapacityReportAsync(
        this AppDbContext db,
        string academicYear,
        CancellationToken ct = default)
    {
        // EnrolmentStatus.Active = 1
        const int ActiveStatus = 1;

        var sql = @"
SELECT
    c.Code        AS CourseCode,
    c.Title       AS CourseTitle,
    o.AcademicYear,
    o.Capacity,
    COUNT(e.Id)   AS ActiveEnrolments,
    CAST(
        CASE WHEN o.Capacity = 0 THEN 0
             ELSE (COUNT(e.Id) * 100.0 / o.Capacity)
        END AS decimal(5,2)
    ) AS UtilisationPercent
FROM CourseOfferings o
JOIN Courses c ON c.Id = o.CourseId
LEFT JOIN Enrolments e
    ON e.CourseOfferingId = o.Id
   AND e.Status = {0}
WHERE o.AcademicYear = {1}
GROUP BY c.Code, c.Title, o.AcademicYear, o.Capacity
ORDER BY UtilisationPercent DESC, ActiveEnrolments DESC;
";

        return await db.CourseCapacityReport
            .FromSqlRaw(sql, ActiveStatus, academicYear)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
