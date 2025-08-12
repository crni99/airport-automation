import React  from 'react';
import TableActions from '../common/table/TableActions';
import { Entities } from '../../utils/const';

export default function PlaneTicketsListTable({ planeTickets }) {
    return (
        <div>
            <hr />
            <table className="table table-responsive table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Price</th>
                        <th>Purchase Date</th>
                        <th>Seat Number</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {planeTickets.map(planeTicket => (
                        <tr key={planeTicket.id}>
                            <td>{planeTicket.id}</td>
                            <td>{planeTicket.price}</td>
                            <td>{planeTicket.purchaseDate}</td>
                            <td>{planeTicket.seatNumber}</td>
                            <TableActions entity={Entities.PLANE_TICKETS} id={planeTicket.id} />
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    );
}
