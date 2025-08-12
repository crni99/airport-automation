export default function TravelClassesListTable( {travelClasses }) {
    return (
        <div>
            <table className="table table-responsive table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Type</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {travelClasses.map(travelClass => (
                        <tr key={travelClass.id}>
                            <td>{travelClass.id}</td>
                            <td>{travelClass.type}</td>
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}