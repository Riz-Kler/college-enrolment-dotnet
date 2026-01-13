using System.Collections.Generic;

namespace CollegeEnrolment.Domain.Entities;

public sealed class Course
{
    public int Id { get; set; }

    public string Code { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    // Example: Level 2, Level 3 (very FE-appropriate)
    public string Level { get; set; } = string.Empty;

    public ICollection<CourseOffering> Offerings { get; set; } = new List<CourseOffering>();
}
