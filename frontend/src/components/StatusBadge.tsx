import type {ApplicationStatus} from "../types/common";
import type {JSX} from "react";
export default function StatusBadge(props: {status: ApplicationStatus}): JSX.Element {
    const statusClassName: string = props.status
        ? props.status.toLowerCase().replace(/\s+/g, "-")
        : ""

    return (
        <span className={`status-badge ${statusClassName}`}>{props.status}</span>
    )
}
