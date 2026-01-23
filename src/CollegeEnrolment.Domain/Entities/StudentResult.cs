using System;

namespace CollegeEnrolment.Domain.Entities
{
    public class StudentResult
    {
        public int Id { get; set; }

        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        // e.g. "2021/22", "2022/23" etc
        public string AcademicYear { get; set; } = string.Empty;

        // A*, A, B, C, D, E, U
        public string FinalGrade { get; set; } = "U";

        // optional but makes “AI” feel real
        public decimal AttendancePercent { get; set; } = 90m;

        public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;
    }
}
