import React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import TableActions from '../common/table/TableActions';
import { ENTITIES } from '../../utils/const';

export default function ApiUsersListTable({ apiUsers }) {
    return (
        <TableContainer component={Paper}>
            <Table stickyHeader aria-label="API Users table">
                <TableHead>
                    <TableRow>
                        <TableCell sx={{ width: '10%'}}>Id</TableCell>
                        <TableCell sx={{ width: '20%'}}>Username</TableCell>
                        <TableCell sx={{ width: '30%'}}>Password</TableCell>
                        <TableCell sx={{ width: '20%'}}>Roles</TableCell>
                        <TableCell sx={{ width: '10%'}}>Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {apiUsers.map((apiUser) => (
                        <TableRow
                            key={apiUser.apiUserId}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell>{apiUser.apiUserId}</TableCell>
                            <TableCell>{apiUser.userName}</TableCell>
                            <TableCell>{apiUser.password}</TableCell>
                            <TableCell>{apiUser.roles}</TableCell>
                            <TableCell>
                                <TableActions entity={ENTITIES.API_USERS} id={apiUser.id} />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}