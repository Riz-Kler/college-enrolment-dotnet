using System.Collections.Generic;

namespace CollegeEnrolment.Domain.Entities;

public sealed class CourseOffering
{
    public int Id { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    // FE academic year format
    public string AcademicYear { get; set; } = "2025/26";

    // Typical FE cohort size
    public int Capacity { get; set; } = 20;

    public ICollection<TimetableSlot> Timetable { get; set; } = new List<TimetableSlot>();
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
