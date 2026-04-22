import type {
    ApplicationFormData,
    createApplicationDto,
    updateApplicationDto,
    ApplicationDetailResponse,
    createdApplicationResponse
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
    };
}

