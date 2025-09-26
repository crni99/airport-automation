import React from 'react';
import { ENTITIES } from '../../utils/const';
import openMap from '../../utils/openMapHelper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Link from '@mui/material/Link';
import TableActions from '../common/table/TableActions';

export default function PassengersListTable({ passengers }) {
    return (
        <TableContainer component={Paper}>
            <Table stickyHeader aria-label="Passengers table">
                <TableHead>
                    <TableRow>
                        <TableCell sx={{ width: '10%' }}>Id</TableCell>
                        <TableCell sx={{ width: '10%' }}>First Name</TableCell>
                        <TableCell sx={{ width: '10%' }}>Last Name</TableCell>
                        <TableCell sx={{ width: '10%' }}>UPRN</TableCell>
                        <TableCell sx={{ width: '10%' }}>Passport</TableCell>
                        <TableCell sx={{ width: '25%' }}>Address</TableCell>
                        <TableCell sx={{ width: '15%' }}>Phone</TableCell>
                        <TableCell sx={{ width: '10%' }} align="center">Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {passengers.map((passenger) => (
                        <TableRow
                            key={passenger.id}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell>{passenger.id}</TableCell>
                            <TableCell>{passenger.firstName}</TableCell>
                            <TableCell>{passenger.lastName}</TableCell>
                            <TableCell>{passenger.uprn}</TableCell>
                            <TableCell>{passenger.passport}</TableCell>
                            <TableCell>
                                <Link
                                    sx={{ color: '#009be5' }}
                                    component="button"
                                    variant="body2"
                                    onClick={() => openMap(passenger.address)}
                                >
                                    {passenger.address}
                                </Link>
                            </TableCell>
                            <TableCell>{passenger.phone}</TableCell>
                            <TableCell>
                                <TableActions entity={ENTITIES.PASSENGERS} id={passenger.id} />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}