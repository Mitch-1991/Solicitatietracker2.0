namespace SollicitatieTracker.App.DTOs
{
    public class InterviewDto
    {
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
