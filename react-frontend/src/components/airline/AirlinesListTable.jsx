import React  from 'react';
import TableActions from '../common/table/TableActions';
import { Entities } from '../../utils/const';

export default function AirlinesListTable( {airlines }) {
    return (
        <div>
            <hr />
            <table className="table table-responsive table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Name</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {airlines.map(airline => (
                        <tr key={airline.id}>
                            <td>{airline.id}</td>
                            <td>{airline.name}</td>
                            <TableActions entity={Entities.AIRLINES} id={airline.id} />
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}