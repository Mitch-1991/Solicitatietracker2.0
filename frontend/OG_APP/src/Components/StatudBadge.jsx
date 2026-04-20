export default function StatusBadge(props){
    const statusClassName = props.status
        ? props.status.toLowerCase().replace(/\s+/g, "-")
        : ""

    return (
        <span className={`status-badge ${statusClassName}`}>{props.status}</span>
    )
}
