namespace SollicitatieTracker.App.DTOs
{
    public class CalendarInterviewDto
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public string JobTitle { get; set; } = string.Empty;
        public string InterviewType { get; set; } = string.Empty;
        public DateTime ScheduledStart { get; set; }
        public DateTime? ScheduledEnd { get; set; }
        public string? Location { get; set; }
        public string? MeetingLink { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactEmail { get; set; }
        public string? Notes { get; set; }
    }
}
