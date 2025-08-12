import React  from 'react';
import TableActions from '../common/table/TableActions';
import { Entities } from '../../utils/const';
import openMap from '../../utils/openMapHelper';

export default function PassengersListTable( {passengers }) {
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
                        <th>Passport</th>
                        <th>Address</th>
                        <th>Phone</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {passengers.map(passenger => (
                        <tr key={passenger.id}>
                            <td>{passenger.id}</td>
                            <td>{passenger.firstName}</td>
                            <td>{passenger.lastName}</td>
                            <td>{passenger.uprn}</td>
                            <td>{passenger.passport}</td>
                            <td className="clickable-row link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover"
                                onClick={() => openMap(passenger.address)}>
                                {passenger.address}
                            </td>
                            <td>{passenger.phone}</td>
                            <TableActions entity={Entities.PASSENGERS} id={passenger.id} />
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}