import React  from 'react';
import TableActions from '../common/table/TableActions';
import { Entities } from '../../utils/const';

export default function FlightsListTable( {flights }) {
    return (
        <div>
            <hr />
            <table className="table table-responsive table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Departure Date</th>
                        <th>Departure Time</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {flights.map(flight => (
                        <tr key={flight.id}>
                            <td>{flight.id}</td>
                            <td>{flight.departureDate}</td>
                            <td>{flight.departureTime}</td>
                            <TableActions entity={Entities.FLIGHTS} id={flight.id} />
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}