using System.Collections.Generic;

namespace CollegeEnrolment.Domain.Entities;

public sealed class Student
{
    public int Id { get; set; }

    public string StudentNumber { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;

    // FE assumption: all students are full-time
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
