import React  from 'react';
import TableActions from '../common/table/TableActions';
import { Entities } from '../../utils/const';

export default function PilotsListTable( {pilots }) {
    return (
        <div>
            <hr />
            <table className="table table-responsive table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>First Name</th>
                        <th>Last Name</th>
                        <th>UPRN</th>
                        <th>Flying Hours</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {pilots.map(pilot => (
                        <tr key={pilot.id}>
                            <td>{pilot.id}</td>
                            <td>{pilot.firstName}</td>
                            <td>{pilot.lastName}</td>
                            <td>{pilot.uprn}</td>
                            <td>{pilot.flyingHours}</td>
                            <TableActions entity={Entities.PILOTS} id={pilot.id} />
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}