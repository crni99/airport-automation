import React  from 'react';
import TableActions from '../common/table/TableActions';
import { Entities } from '../../utils/const';
import openMap from "../../utils/openMapHelper"

export default function DestinationsListTable( {destinations }) {
    return (
        <div>
            <hr />
            <table className="table table-responsive table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>City</th>
                        <th>Airport</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {destinations.map(destination => (
                        <tr key={destination.id}>
                            <td>{destination.id}</td>
                            <td className="clickable-row link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover"
                                onClick={() => openMap(destination.airport)}>
                                {destination.city}
                            </td>
                            <td className="clickable-row link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover"
                                onClick={() => openMap(destination.airport)}>
                                {destination.airport}
                            </td>
                            <TableActions entity={Entities.DESTINATIONS} id={destination.id} />
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}