import { CalendarDays, ChevronLeft, ChevronRight, Clock, Mail, MapPin, StickyNote, UserRound, Video } from "lucide-react";
import { useEffect, useMemo, useState, type JSX } from "react";
import { getCalendarInterviews } from "../services/calendarService";

import type { CalendarDay, CalendarInterview, CalendarInterviewResponse } from "../types/calendar";

const weekdays: string[] = ["Ma", "Di", "Wo", "Do", "Vr", "Za", "Zo"];

const dateFormatter = new Intl.DateTimeFormat("nl-BE", {
    weekday: "long",
    day: "numeric",
    month: "long",
    year: "numeric"
});

const monthFormatter = new Intl.DateTimeFormat("nl-BE", {
    month: "long",
    year: "numeric"
});

const timeFormatter = new Intl.DateTimeFormat("nl-BE", {
    hour: "2-digit",
    minute: "2-digit"
});

export default function Calendar(): JSX.Element {
    const today = useMemo(() => new Date(), []);
    const [visibleMonth, setVisibleMonth] = useState<Date>(() => startOfMonth(today));
    const [selectedDate, setSelectedDate] = useState<string>(() => toDateKey(today));
    const [interviews, setInterviews] = useState<CalendarInterview[]>([]);
    const [isLoading, setIsLoading] = useState<boolean>(false);
    const [error, setError] = useState<string | null>(null);

    const gridRange = useMemo(() => getCalendarGridRange(visibleMonth), [visibleMonth]);
    const gridStartKey = useMemo(() => toDateKey(gridRange.start), [gridRange]);
    const gridEndKey = useMemo(() => toDateKey(gridRange.end), [gridRange]);
    const monthLabel = monthFormatter.format(visibleMonth);

    useEffect(() => {
        let isActive = true;

        const fetchInterviews = async (): Promise<void> => {
            setIsLoading(true);
            setError(null);

            try {
                const data: CalendarInterviewResponse[] = await getCalendarInterviews(gridStartKey, gridEndKey);

                if (isActive) {
                    setInterviews(data.map(mapCalendarInterview));
                }
            } catch (err) {
                if (isActive) {
                    setInterviews([]);
                    setError(err instanceof Error ? err.message : "Kalenderinterviews ophalen mislukt.");
                }
            } finally {
                if (isActive) {
                    setIsLoading(false);
                }
            }
        };

        fetchInterviews();

        return () => {
            isActive = false;
        };
    }, [gridEndKey, gridStartKey]);

    const groupedInterviews = useMemo(() => {
        return interviews.reduce<Record<string, CalendarInterview[]>>((groups, interview) => {
            groups[interview.dateKey] = groups[interview.dateKey] ?? [];
            groups[interview.dateKey].push(interview);
            return groups;
        }, {});
    }, [interviews]);

    const calendarDays = useMemo<CalendarDay[]>(() => {
        return buildCalendarDays(visibleMonth, groupedInterviews, toDateKey(today));
    }, [groupedInterviews, today, visibleMonth]);

    const selectedDayInterviews = groupedInterviews[selectedDate] ?? [];
    const selectedDateLabel = formatDateLabel(selectedDate);
    const hasInterviewsInMonth = interviews.some((interview) => {
        const interviewDate = new Date(interview.scheduledStart);
        return (
            interviewDate.getFullYear() === visibleMonth.getFullYear() &&
            interviewDate.getMonth() === visibleMonth.getMonth()
        );
    });

    const handlePreviousMonth = (): void => {
        changeMonth(-1);
    };

    const handleNextMonth = (): void => {
        changeMonth(1);
    };

    const handleToday = (): void => {
        const currentMonth = startOfMonth(today);
        setVisibleMonth(currentMonth);
        setSelectedDate(toDateKey(today));
    };

    const changeMonth = (offset: number): void => {
        const nextMonth = new Date(visibleMonth.getFullYear(), visibleMonth.getMonth() + offset, 1);
        setVisibleMonth(nextMonth);
        setSelectedDate(toDateKey(nextMonth));
    };

    return (
        <main className="dashboard-container calendar-page">
            <header className="page-header calendar-page-header">
                <div className="page-heading">
                    <h1 className="dashboard-title">Kalender</h1>
                    <p className="dashboard-subtitle">Interviews per maand en geselecteerde dag.</p>
                </div>
            </header>

            <section className="calendar-shell" aria-label="Interviewkalender">
                <section className="calendar-month-panel" aria-label="Maandweergave">
                    <div className="calendar-toolbar">
                        <button className="calendar-icon-button" type="button" onClick={handlePreviousMonth} aria-label="Vorige maand">
                            <ChevronLeft aria-hidden="true" />
                        </button>

                        <h2 className="calendar-month-title">{monthLabel}</h2>

                        <button className="calendar-icon-button" type="button" onClick={handleNextMonth} aria-label="Volgende maand">
                            <ChevronRight aria-hidden="true" />
                        </button>

                        <button className="calendar-today-button" type="button" onClick={handleToday}>
                            <CalendarDays aria-hidden="true" />
                            Vandaag
                        </button>
                    </div>

                    <div className="calendar-weekdays" aria-hidden="true">
                        {weekdays.map((weekday) => (
                            <span key={weekday}>{weekday}</span>
                        ))}
                    </div>

                    <div className="calendar-grid">
                        {calendarDays.map((day) => (
                            <button
                                key={day.dateKey}
                                className={[
                                    "calendar-day-button",
                                    day.isCurrentMonth ? "" : "muted",
                                    day.isToday ? "today" : "",
                                    selectedDate === day.dateKey ? "selected" : "",
                                    day.interviews.length > 0 ? "has-interviews" : ""
                                ].filter(Boolean).join(" ")}
                                type="button"
                                onClick={() => setSelectedDate(day.dateKey)}
                            >
                                <span className="calendar-day-topline">
                                    <span className="calendar-day-number">{day.dayNumber}</span>
                                    {day.interviews.length > 0 && (
                                        <span className="calendar-day-count">{day.interviews.length}</span>
                                    )}
                                </span>

                                {day.interviews.length > 0 && (
                                    <span className="calendar-day-items">
                                        {day.interviews.slice(0, 2).map((interview) => (
                                            <span key={interview.id} className="calendar-day-item">
                                                {interview.companyName}
                                            </span>
                                        ))}
                                    </span>
                                )}
                            </button>
                        ))}
                    </div>

                    {isLoading && <p className="calendar-state">Interviews laden...</p>}
                    {error && <p className="calendar-state calendar-state-error">{error}</p>}
                    {!isLoading && !error && !hasInterviewsInMonth && (
                        <p className="calendar-state">Geen interviews in deze maand.</p>
                    )}
                </section>

                <aside className="calendar-agenda-panel" aria-label="Geselecteerde dag">
                    <div className="calendar-agenda-header">
                        <span>Geselecteerde dag</span>
                        <h2>{selectedDateLabel}</h2>
                    </div>

                    {selectedDayInterviews.length > 0 ? (
                        <ul className="calendar-agenda-list">
                            {selectedDayInterviews.map((interview) => (
                                <li key={interview.id} className="calendar-agenda-item">
                                    <div className="calendar-agenda-item-header">
                                        <div>
                                            <h3>{interview.companyName}</h3>
                                            <p>{interview.jobTitle}</p>
                                        </div>
                                        <span className="calendar-interview-type">{interview.interviewType}</span>
                                    </div>

                                    <dl className="calendar-agenda-details">
                                        <div>
                                            <dt><Clock aria-hidden="true" /> Tijd</dt>
                                            <dd>{interview.timeLabel}</dd>
                                        </div>
                                        <div>
                                            <dt>{interview.meetingLink ? <Video aria-hidden="true" /> : <MapPin aria-hidden="true" />} {interview.meetingLink ? "Meeting" : "Locatie"}</dt>
                                            <dd>{renderLocationValue(interview)}</dd>
                                        </div>
                                        {interview.contactPerson && (
                                            <div>
                                                <dt><UserRound aria-hidden="true" /> Contact</dt>
                                                <dd>{interview.contactPerson}</dd>
                                            </div>
                                        )}
                                        {interview.contactEmail && (
                                            <div>
                                                <dt><Mail aria-hidden="true" /> E-mail</dt>
                                                <dd>{interview.contactEmail}</dd>
                                            </div>
                                        )}
                                        {interview.notes && (
                                            <div>
                                                <dt><StickyNote aria-hidden="true" /> Notities</dt>
                                                <dd>{interview.notes}</dd>
                                            </div>
                                        )}
                                    </dl>
                                </li>
                            ))}
                        </ul>
                    ) : (
                        <p className="calendar-empty-day">Geen interviews op deze dag.</p>
                    )}
                </aside>
            </section>
        </main>
    );
}

function startOfMonth(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth(), 1);
}

function endOfMonth(date: Date): Date {
    return new Date(date.getFullYear(), date.getMonth() + 1, 0);
}

function getCalendarGridRange(visibleMonth: Date): { start: Date; end: Date } {
    const monthStart = startOfMonth(visibleMonth);
    const monthEnd = endOfMonth(visibleMonth);
    const leadingDays = (monthStart.getDay() + 6) % 7;
    const daysInGrid = Math.ceil((leadingDays + monthEnd.getDate()) / 7) * 7;
    const start = new Date(monthStart);
    start.setDate(monthStart.getDate() - leadingDays);

    const end = new Date(start);
    end.setDate(start.getDate() + daysInGrid - 1);

    return { start, end };
}

function toDateKey(date: Date): string {
    const year = date.getFullYear();
    const month = String(date.getMonth() + 1).padStart(2, "0");
    const day = String(date.getDate()).padStart(2, "0");

    return `${year}-${month}-${day}`;
}

function mapCalendarInterview(interview: CalendarInterviewResponse): CalendarInterview {
    const start = new Date(interview.scheduledStart);
    const end = interview.scheduledEnd ? new Date(interview.scheduledEnd) : null;

    return {
        ...interview,
        dateKey: toDateKey(start),
        dateLabel: dateFormatter.format(start),
        timeLabel: end ? `${timeFormatter.format(start)} - ${timeFormatter.format(end)}` : timeFormatter.format(start)
    };
}

function buildCalendarDays(
    visibleMonth: Date,
    groupedInterviews: Record<string, CalendarInterview[]>,
    todayKey: string
): CalendarDay[] {
    const gridRange = getCalendarGridRange(visibleMonth);
    const daysInGrid = differenceInDays(gridRange.start, gridRange.end) + 1;

    return Array.from({ length: daysInGrid }, (_, index) => {
        const date = new Date(gridRange.start);
        date.setDate(gridRange.start.getDate() + index);
        const dateKey = toDateKey(date);

        return {
            date,
            dateKey,
            dayNumber: date.getDate(),
            isCurrentMonth: date.getMonth() === visibleMonth.getMonth(),
            isToday: dateKey === todayKey,
            interviews: groupedInterviews[dateKey] ?? []
        };
    });
}

function differenceInDays(start: Date, end: Date): number {
    const millisecondsPerDay = 24 * 60 * 60 * 1000;
    return Math.round((end.getTime() - start.getTime()) / millisecondsPerDay);
}

function formatDateLabel(dateKey: string): string {
    return dateFormatter.format(new Date(`${dateKey}T00:00:00`));
}

function renderLocationValue(interview: CalendarInterview): JSX.Element | string {
    if (interview.meetingLink) {
        return (
            <a href={interview.meetingLink} target="_blank" rel="noreferrer">
                {interview.meetingLink}
            </a>
        );
    }

    return interview.location ?? "-";
}
