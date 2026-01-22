using CollegeEnrolment.Domain.Entities;
using Microsoft.EntityFrameworkCore;

using CollegeEnrolment.Domain.Enums;


namespace CollegeEnrolment.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        await db.Database.MigrateAsync(ct);

        if (await db.Courses.AnyAsync(ct)) return;

        var courses = new List<Course>
        {
            new() { Code = "ITFND", Title = "IT Fundamentals", Level = "Level 2" },
            new() { Code = "NET01", Title = ".NET Web Development", Level = "Level 3" },
            new() { Code = "CS101", Title = "Intro to Programming", Level = "Level 2" }
        };

        db.Courses.AddRange(courses);
        await db.SaveChangesAsync(ct);

        var offerings = courses.Select(c => new CourseOffering
        {
            CourseId = c.Id,
            AcademicYear = "2025/26",
            Capacity = 20
        }).ToList();

        db.CourseOfferings.AddRange(offerings);
        await db.SaveChangesAsync(ct);

        var slots = new List<TimetableSlot>
        {
            new() { CourseOfferingId = offerings[0].Id, DayOfWeek = DayOfWeek.Monday, StartTime = new TimeOnly(9,0), EndTime = new TimeOnly(11,0), Room = "Lab 1" },
            new() { CourseOfferingId = offerings[1].Id, DayOfWeek = DayOfWeek.Wednesday, StartTime = new TimeOnly(13,0), EndTime = new TimeOnly(15,0), Room = "Room 12" },
            new() { CourseOfferingId = offerings[2].Id, DayOfWeek = DayOfWeek.Friday, StartTime = new TimeOnly(10,0), EndTime = new TimeOnly(12,0), Room = "Lab 2" }
        };

        db.TimetableSlots.AddRange(slots);

        var students = new List<Student>
        {
            new() { StudentNumber = "S000001", FirstName = "Aisha", LastName = "Khan", Email = "aisha.khan@example.test" },
            new() { StudentNumber = "S000002", FirstName = "Tom", LastName = "Brown", Email = "tom.brown@example.test" }
        };

        db.Students.AddRange(students);

        await db.SaveChangesAsync(ct);

        db.Enrolments.Add(new Enrolment { StudentId = students[0].Id, CourseOfferingId = offerings[0].Id, Status = EnrolmentStatus.Active });
        db.Enrolments.Add(new Enrolment { StudentId = students[0].Id, CourseOfferingId = offerings[1].Id, Status = EnrolmentStatus.Active });

        db.AuditLogs.Add(new AuditLog
        {
            Actor = "seed",
            Action = "SEED",
            EntityType = "Database",
            EntityId = "CollegeEnrolmentDb",
            Details = "Seeded initial demo data"
        });

        await db.SaveChangesAsync(ct);
    }
}
