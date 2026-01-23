using CollegeEnrolment.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using CollegeEnrolment.Domain.Enums;

namespace CollegeEnrolment.Data.Seed;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db, CancellationToken ct = default)
    {
        // Ensure schema exists / is up to date
        await db.Database.MigrateAsync(ct);

        // Always ensure baseline A-Level catalog + demo data exists (idempotent-ish)
        await SeedCatalogAndCurrentYearAsync(db, ct);

        // AI history goes AFTER baseline seed (so the app is usable even if AI seed changes later)
        await SeedAiHistoryAsync(db, ct);
    }

    private static async Task SeedCatalogAndCurrentYearAsync(AppDbContext db, CancellationToken ct)
    {
        // ---------- Courses ----------
        var desiredCourses = new List<Course>
        {
            new() { Code = "MATH", Title = "A Level Mathematics", Level = "A Level" },
            new() { Code = "FMTH", Title = "A Level Further Mathematics", Level = "A Level" },
            new() { Code = "PHYS", Title = "A Level Physics", Level = "A Level" },
            new() { Code = "CHEM", Title = "A Level Chemistry", Level = "A Level" },
            new() { Code = "BIOL", Title = "A Level Biology", Level = "A Level" },
            new() { Code = "CSCI", Title = "A Level Computer Science", Level = "A Level" },
            new() { Code = "ENGL", Title = "A Level English Literature", Level = "A Level" },
            new() { Code = "HIST", Title = "A Level History", Level = "A Level" },
            new() { Code = "ECON", Title = "A Level Economics", Level = "A Level" },
            new() { Code = "PSYC", Title = "A Level Psychology", Level = "A Level" }
        };

        // Upsert courses by Code
        var existingCourses = await db.Courses.ToListAsync(ct);
        foreach (var c in desiredCourses)
        {
            var exists = existingCourses.FirstOrDefault(x => x.Code == c.Code);
            if (exists is null)
            {
                db.Courses.Add(c);
            }
            else
            {
                // keep it safe: update title/level if you renamed them
                exists.Title = c.Title;
                exists.Level = c.Level;
            }
        }

        await db.SaveChangesAsync(ct);

        // Refresh with IDs
        var allCourses = await db.Courses.OrderBy(c => c.Code).ToListAsync(ct);

        // ---------- Offerings (current year) ----------
        const string currentYear = "2025/26";
        var existingOfferings = await db.CourseOfferings
            .Where(o => o.AcademicYear == currentYear)
            .ToListAsync(ct);

        foreach (var course in allCourses)
        {
            var hasOffering = existingOfferings.Any(o => o.CourseId == course.Id);
            if (!hasOffering)
            {
                db.CourseOfferings.Add(new CourseOffering
                {
                    CourseId = course.Id,
                    AcademicYear = currentYear,
                    Capacity = 24
                });
            }
        }

        await db.SaveChangesAsync(ct);

        var offerings = await db.CourseOfferings
            .Include(o => o.Course)
            .Where(o => o.AcademicYear == currentYear)
            .OrderBy(o => o.Course!.Code)
            .ToListAsync(ct);

        // ---------- Students ----------
        // Keep it tight: only seed these if you don't have enough demo students
        var studentCount = await db.Students.CountAsync(ct);
        if (studentCount < 10)
        {
            var demoStudents = new List<Student>
            {
                new() { StudentNumber = "S000001", FirstName = "Aisha",  LastName = "Khan",     Email = "aisha.khan@example.test" },
                new() { StudentNumber = "S000002", FirstName = "Tom",    LastName = "Brown",    Email = "tom.brown@example.test" },
                new() { StudentNumber = "S000003", FirstName = "Bilal",  LastName = "Ahmed",    Email = "bilal.ahmed@example.test" },
                new() { StudentNumber = "S000004", FirstName = "Chloe",  LastName = "Taylor",   Email = "chloe.taylor@example.test" },
                new() { StudentNumber = "S000005", FirstName = "Daniel", LastName = "Evans",    Email = "daniel.evans@example.test" },
                new() { StudentNumber = "S000006", FirstName = "Sophia", LastName = "Patel",    Email = "sophia.patel@example.test" },
                new() { StudentNumber = "S000007", FirstName = "Liam",   LastName = "Wilson",   Email = "liam.wilson@example.test" },
                new() { StudentNumber = "S000008", FirstName = "Maya",   LastName = "Singh",    Email = "maya.singh@example.test" },
                new() { StudentNumber = "S000009", FirstName = "Noah",   LastName = "Johnson",  Email = "noah.johnson@example.test" },
                new() { StudentNumber = "S000010", FirstName = "Zara",   LastName = "Ali",      Email = "zara.ali@example.test" }
            };

            // Avoid duplicates if you've already inserted some of them
            var existingNumbers = await db.Students.Select(s => s.StudentNumber).ToListAsync(ct);
            foreach (var s in demoStudents.Where(s => !existingNumbers.Contains(s.StudentNumber)))
            {
                db.Students.Add(s);
            }

            await db.SaveChangesAsync(ct);
        }

        var students = await db.Students
            .OrderBy(s => s.StudentNumber)
            .Take(10)
            .ToListAsync(ct);

        // ---------- Enrolments ----------
        // Goal: 10 students + 10 courses = coverage across all courses.
        // Create 2 enrolments per student (20 total), rotating courses so every course has at least 2.
        var existingEnrolments = await db.Enrolments
            .Include(e => e.Student)
            .Include(e => e.CourseOffering)
            .ThenInclude(o => o.Course)
            .ToListAsync(ct);

        // Build quick lookup: (StudentId, OfferingId)
        var enrolKey = new HashSet<(int studentId, int offeringId)>(
            existingEnrolments.Select(e => (e.StudentId, e.CourseOfferingId))
        );

        int offeringIndex = 0;
        foreach (var s in students)
        {
            // pick 2 offerings per student
            var o1 = offerings[offeringIndex % offerings.Count];
            var o2 = offerings[(offeringIndex + 1) % offerings.Count];
            offeringIndex += 2;

            if (!enrolKey.Contains((s.Id, o1.Id)))
            {
                db.Enrolments.Add(new Enrolment
                {
                    StudentId = s.Id,
                    CourseOfferingId = o1.Id,
                    Status = EnrolmentStatus.Active
                });
                enrolKey.Add((s.Id, o1.Id));
            }

            if (!enrolKey.Contains((s.Id, o2.Id)))
            {
                db.Enrolments.Add(new Enrolment
                {
                    StudentId = s.Id,
                    CourseOfferingId = o2.Id,
                    Status = EnrolmentStatus.Active
                });
                enrolKey.Add((s.Id, o2.Id));
            }
        }

        // A simple seed audit event
        db.AuditLogs.Add(new AuditLog
        {
            Actor = "seed",
            Action = "SEED_CURRENT",
            EntityType = "Database",
            EntityId = "CollegeEnrolmentDb",
            Details = $"Seeded current year demo data ({currentYear})",
            TimestampUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
    }

    /// <summary>
    /// AI history seed: 4 academic years x 25 outcomes each = 100 records.
    /// Stored as AuditLogs so new tables are not required yet.
    /// AI page can query AuditLogs where Action == "AI_HISTORY_OUTCOME".
    /// </summary>
    private static async Task SeedAiHistoryAsync(AppDbContext db, CancellationToken ct)
    {
        // Avoid reseeding if already present
        var alreadySeeded = await db.AuditLogs.AnyAsync(a => a.Action == "AI_HISTORY_OUTCOME", ct);
        if (alreadySeeded) return;

        var rng = new Random(42);

        var years = new[] { "2021/22", "2022/23", "2023/24", "2024/25" };

        // Keep codes in sync with your seeded course catalog
        var courseCodes = new[] { "MATH", "FMTH", "PHYS", "CHEM", "BIOL", "CSCI", "ENGL", "HIST", "ECON", "PSYC" };

        // Weighted grades (roughly)
        string SampleGrade()
        {
            var roll = rng.Next(100);
            if (roll < 10) return "A*";
            if (roll < 30) return "A";
            if (roll < 55) return "B";
            if (roll < 75) return "C";
            if (roll < 90) return "D";
            return "E";
        }

        // Attendance & mock score give you something to chart
        int SampleAttendance() => rng.Next(70, 100);     // %
        int SampleMock() => rng.Next(40, 100);           // %
        bool SamplePP() => rng.Next(100) < 18;           // disadvantaged flag (demo)

        var logs = new List<AuditLog>();
        var now = DateTime.UtcNow;

        foreach (var year in years)
        {
            for (var i = 1; i <= 25; i++)
            {
                // "historic student" id is just a synthetic label inside Details
                var histStudent = $"{year.Replace("/", "")}-H{i:000}";

                // Give each historic student 3 subjects
                var subjects = courseCodes
                    .OrderBy(_ => rng.Next())
                    .Take(3)
                    .ToArray();

                foreach (var subject in subjects)
                {
                    var attendance = SampleAttendance();
                    var mock = SampleMock();
                    var grade = SampleGrade();
                    var pp = SamplePP();

                    // Store as simple JSON-ish string (easy to parse without adding libs)
                    var details =
                        $"year={year};student={histStudent};course={subject};grade={grade};attendance={attendance};mock={mock};pp={(pp ? "Y" : "N")}";

                    logs.Add(new AuditLog
                    {
                        Actor = "seed",
                        Action = "AI_HISTORY_OUTCOME",
                        EntityType = "AI",
                        EntityId = $"{histStudent}:{subject}",
                        Details = details,
                        TimestampUtc = now.AddDays(-rng.Next(30, 900))
                    });
                }
            }
        }

        db.AuditLogs.AddRange(logs);

        db.AuditLogs.Add(new AuditLog
        {
            Actor = "seed",
            Action = "SEED_AI_HISTORY",
            EntityType = "AI",
            EntityId = "CollegeEnrolmentDb",
            Details = "Seeded AI history: 4 years x 25 students x 3 subjects (stored in AuditLogs)",
            TimestampUtc = DateTime.UtcNow
        });

        await db.SaveChangesAsync(ct);
    }
}
