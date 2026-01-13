using System;
using CollegeEnrolment.Domain.Enums;

namespace CollegeEnrolment.Domain.Entities;

public sealed class Enrolment
{
    public int Id { get; set; }

    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;

    public int CourseOfferingId { get; set; }
    public CourseOffering CourseOffering { get; set; } = null!;

    public EnrolmentStatus Status { get; set; } = EnrolmentStatus.Active;

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
