import type {
    ApplicationFormData,
    createApplicationDto,
    updateApplicationDto,
    ApplicationDetailResponse,
    createdApplicationResponse,
    InterviewDto
} from "../types/application";
import { applicationStatusToApiStatusMap } from "../types/common";
import type { ApplicationStatus, PriorityStatus } from "../types/common";
import type { DashboardOverviewItem } from "../types/dashboard";

export const emptyFormData: ApplicationFormData = {
    companyName: "",
    jobTitle: "",
    jobUrl: "",
    status: "",
    date: "",
    priority: "",
    location: "",
    salaryMin: "",
    salaryMax: "",
    contactPerson: "",
    contactEmail: "",
    nextStep: "",
    notes: "",
    websiteURL: "",
    interviewType: "",
    interviewDate: "",
    interviewStartTime: "",
    interviewEndTime: "",
    interviewLocation: "",
    meetingLink: "",
    interviewContactPerson: "",
    interviewContactEmail: "",
    interviewNotes: "",
};

export function mapFormDataToCreateDto(data: ApplicationFormData): createApplicationDto {
    const trimmedStatus = data.status.trim() as ApplicationStatus

    return {
        userId: 1,
        companyName: data.companyName.trim(),
        jobTitle: data.jobTitle.trim(),
        jobUrl: data.jobUrl?.trim() || null,
        location: data.location?.trim() || null,
        status: applicationStatusToApiStatusMap[trimmedStatus],
        priority: (data.priority?.trim()) as PriorityStatus || null,
        appliedDate: data.date || null,
        nextStep: data.nextStep.trim() || null,
        salaryMin: data.salaryMin ? Number(data.salaryMin) : null,
        salaryMax: data.salaryMax ? Number(data.salaryMax) : null,
        notes: data.notes.trim() || null,
        interview: mapInterviewFormDataToDto(data),
    };
}
export function mapFormDataToUpdateDto(data: ApplicationFormData): updateApplicationDto {
    const trimmedStatus = data.status.trim() as ApplicationStatus

    return {
        companyName: data.companyName.trim(),
        jobTitle: data.jobTitle.trim(),
        jobUrl: data.jobUrl?.trim() || null,
        location: data.location?.trim() || null,
        status: applicationStatusToApiStatusMap[trimmedStatus],
        priority: (data.priority?.trim()) as PriorityStatus || null,
        appliedDate: data.date || null,
        nextStep: data.nextStep.trim() || null,
        salaryMin: data.salaryMin ? Number(data.salaryMin) : null,
        salaryMax: data.salaryMax ? Number(data.salaryMax) : null,
        notes: data.notes.trim() || null,
        interview: mapInterviewFormDataToDto(data),
    };
}

export function mapCreatedApplicationToOverviewItem(application: createdApplicationResponse, companyName: string): DashboardOverviewItem {
    return {
        id: application.id,
        companyName: companyName.trim(),
        jobTitle: application.jobTitle,
        status: application.status,
        appliedDate: application.appliedDate,
        nextStep: application.nextStep,
    };
}


export function mapApplicationToFormData(
    application: ApplicationDetailResponse | createdApplicationResponse | null): ApplicationFormData {
    if (!application) {
        return emptyFormData;
    }

    return {
        companyName: application.companyName ?? "",
        jobTitle: application.jobTitle ?? "",
        jobUrl: application.jobUrl ?? "",
        status: application.status ?? "",
        date: application.appliedDate ?? "",
        priority: application.priority ?? "",
        location: application.location ?? "",
        salaryMin: application.salaryMin ?? "",
        salaryMax: application.salaryMax ?? "",
        contactPerson: application.contactPerson ?? "",
        contactEmail: application.contactEmail ?? "",
        nextStep: application.nextStep ?? "",
        notes: application.notes ?? "",
        websiteURL: "",
        interviewType: normalizeInterviewType(application.interview?.interviewType),
        interviewDate: application.interview ? toDateInputValue(application.interview.scheduledStart) : "",
        interviewStartTime: application.interview ? toTimeInputValue(application.interview.scheduledStart) : "",
        interviewEndTime: application.interview?.scheduledEnd ? toTimeInputValue(application.interview.scheduledEnd) : "",
        interviewLocation: application.interview?.location ?? "",
        meetingLink: application.interview?.meetingLink ?? "",
        interviewContactPerson: application.interview?.contactPerson ?? "",
        interviewContactEmail: application.interview?.contactEmail ?? "",
        interviewNotes: application.interview?.notes ?? "",
    };
}

function mapInterviewFormDataToDto(data: ApplicationFormData): InterviewDto | null {
    if (data.status !== "Gesprek") {
        return null;
    }

    if (data.interviewType !== "Online" && data.interviewType !== "Op locatie") {
        return null;
    }

    return {
        interviewType: data.interviewType,
        scheduledStart: combineDateAndTime(data.interviewDate, data.interviewStartTime),
        scheduledEnd: data.interviewEndTime ? combineDateAndTime(data.interviewDate, data.interviewEndTime) : null,
        location: data.interviewType === "Op locatie" ? data.interviewLocation.trim() || null : null,
        meetingLink: data.interviewType === "Online" ? data.meetingLink.trim() || null : null,
        contactPerson: data.interviewContactPerson.trim() || null,
        contactEmail: data.interviewContactEmail.trim() || null,
        notes: data.interviewNotes.trim() || null,
    };
}

function combineDateAndTime(date: string, time: string): string {
    return `${date}T${time}:00`;
}

function toDateInputValue(value: string): string {
    return value.slice(0, 10);
}

function toTimeInputValue(value: string): string {
    const time = value.includes("T") ? value.split("T")[1] : value.split(" ")[1] ?? "";
    return time.slice(0, 5);
}

function normalizeInterviewType(value: string | undefined): "Online" | "Op locatie" | "" {
    return value === "Online" || value === "Op locatie" ? value : "";
}

