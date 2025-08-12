import React  from 'react';
import TableActions from '../common/table/TableActions';
import { Entities } from '../../utils/const';

export default function ApiUsersListTable( {apiUsers }) {
    return (
        <div>
            <hr />
            <table className="table table-responsive table-striped">
                <thead>
                    <tr>
                        <th>Id</th>
                        <th>Username</th>
                        <th>Password</th>
                        <th>Roles</th>
                        <th>Actions</th>
                    </tr>
                </thead>
                <tbody id="tableBody">
                    {apiUsers.map(apiUser => (
                        <tr key={apiUser.apiUserId}>
                            <td>{apiUser.apiUserId}</td>
                            <td>{apiUser.userName}</td>
                            <td>{apiUser.password}</td>
                            <td>{apiUser.roles}</td>
                            <TableActions entity={Entities.API_USERS} id={apiUser.id} />
                        </tr>
                    ))}
                </tbody>
            </table>
        </div>
    )
}