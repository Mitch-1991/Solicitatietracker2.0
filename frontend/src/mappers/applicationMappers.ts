import type { 
    ApplicationFormData, 
    createApplicationDto, 
    OverviewApplication, 
    updateApplicationDto 
} from "../types/application";
import type {ApplicationStatus, PriorityStatus} from "../types/common";

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
    return {
        userId: 1,
        companyName: data.companyName.trim(),
        jobTitle: data.jobTitle.trim(),
        jobUrl: data.jobUrl?.trim() || null,
        location: data.location?.trim() || null,
        status: (data.status.trim()) as ApplicationStatus,
        priority: (data.priority.trim()) as PriorityStatus || null,
        appliedDate: data.date ||  null,
        nextStep: data.nextStep.trim() || null,
        salaryMin: data.salaryMin ? Number(data.salaryMin) : null,
        salaryMax: data.salaryMax ? Number(data.salaryMax) : null,
        notes: data.notes.trim() || null,
    };
}
export function mapFormDataToUpdateDto(data: ApplicationFormData): updateApplicationDto {
    return {
        companyName: data.companyName.trim(),
        jobTitle: data.jobTitle.trim(),
        jobUrl: data.jobUrl?.trim() || null,
        location: data.location?.trim() || null,
        status: (data.status.trim()) as ApplicationStatus,
        priority: (data.priority.trim()) as PriorityStatus || null,
        appliedDate: data.date || null,
        nextStep: data.nextStep.trim() || null,
        salaryMin: data.salaryMin ? Number(data.salaryMin) : null,
        salaryMax: data.salaryMax ? Number(data.salaryMax) : null,
        notes: data.notes.trim() || null,
    };
}

export function mapCreatedApplicationToOverviewItem(application: createApplicationDto, bedrijf: string): OverviewApplication {
    return {
        id: application.userId,
        company: bedrijf.trim(),
        function: application.jobTitle,
        status: application.status,
        date: application.appliedDate,
        nextStep: application.nextStep,
    };
}

//any moet nog aangepast worden
export function mapApplicationToFormData(application: any): ApplicationFormData {
    if (!application) {
        return emptyFormData;
    }

    return {
        companyName: application.bedrijf ?? "",
        jobTitle: application.functie ?? application.jobTitle ?? "",
        jobUrl: application.jobUrl ?? "",
        status: application.status ?? "",
        date: application.datum ?? application.appliedDate ?? "",
        priority: application.priority ?? "",
        location: application.locatie ?? application.location ?? "",
        salaryMin: application.salarisMin?.toString() ?? application.salaryMin?.toString() ?? "",
        salaryMax: application.salarisMax?.toString() ?? application.salaryMax?.toString() ?? "",
        contactPerson: application.contactpersoon ?? application.contactPerson ?? "",
        contactEmail: application.contactEmail ?? "",
        nextStep: application.volgendeStap ?? application.nextStep ?? "",
        notes: application.beschrijving ?? application.omschrijving ?? "",
    };
}

