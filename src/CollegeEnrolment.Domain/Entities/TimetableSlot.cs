using System;

namespace CollegeEnrolment.Domain.Entities;

public sealed class TimetableSlot
{
    public int Id { get; set; }

    public int CourseOfferingId { get; set; }
    public CourseOffering CourseOffering { get; set; } = null!;

    public DayOfWeek DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }

    public string Room { get; set; } = string.Empty;
}
