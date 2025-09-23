import React from 'react';
import { ENTITIES } from '../../utils/const';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import TableActions from '../common/table/TableActions';

export default function FlightsListTable({ flights }) {
    return (
        <TableContainer component={Paper}>
            <Table stickyHeader aria-label="Flights table">
                <TableHead>
                    <TableRow>
                        <TableCell sx={{ width: '20%' }}>Id</TableCell>
                        <TableCell sx={{ width: '30%' }}>Departure Date</TableCell>
                        <TableCell sx={{ width: '30%' }}>Departure Time</TableCell>
                        <TableCell sx={{ width: '20%' }}>Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {flights.map((flight) => (
                        <TableRow
                            key={flight.id}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell component="th" scope="row">
                                {flight.id}
                            </TableCell>
                            <TableCell>{flight.departureDate}</TableCell>
                            <TableCell>{flight.departureTime}</TableCell>
                            <TableCell>
                                <TableActions entity={ENTITIES.FLIGHTS} id={flight.id} />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}