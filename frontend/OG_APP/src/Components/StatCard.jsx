export default function StatCard(props) {

    const Icon = props.icoon
    const styles = {
        backgroundColor: props.kleur
    }
    return (
        <div className="kpi-card" >
            <div>
                <p className="kpi-label">{props.label}</p>
                <p className="kpi-waarde">{props.waarde}</p>
            </div>
            <div className="kpi-icon" style={styles}>
                <Icon />
            </div>

        </div>
    )
}