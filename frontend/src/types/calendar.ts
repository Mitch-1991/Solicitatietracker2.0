export interface CalendarInterviewResponse {
    id: number;
    applicationId: number;
    companyName: string;
    jobTitle: string;
    interviewType: string;
    scheduledStart: string;
    scheduledEnd: string | null;
    location: string | null;
    meetingLink: string | null;
    contactPerson: string | null;
    contactEmail: string | null;
    notes: string | null;
}

export interface CalendarInterview extends CalendarInterviewResponse {
    dateKey: string;
    dateLabel: string;
    timeLabel: string;
}

export interface CalendarDay {
    date: Date;
    dateKey: string;
    dayNumber: number;
    isCurrentMonth: boolean;
    isToday: boolean;
    interviews: CalendarInterview[];
}
