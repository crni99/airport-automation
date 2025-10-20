import React from 'react';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import TableActions from '../common/table/TableActions';
import { currencyFormatter } from '../../utils/formatting';

export default function PlaneTicketsListTable({ planeTickets }) {
    return (
        <TableContainer component={Paper}>
            <Table stickyHeader aria-label="Plane Tickets table">
                <TableHead>
                    <TableRow>
                        <TableCell sx={{ width: '20%' }}>Id</TableCell>
                        <TableCell sx={{ width: '20%' }}>Price (€)</TableCell>
                        <TableCell sx={{ width: '20%' }}>Purchase Date</TableCell>
                        <TableCell sx={{ width: '20%' }}>Seat Number</TableCell>
                        <TableCell sx={{ width: '20%' }} align="center">Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {planeTickets.map((planeTicket) => (
                        <TableRow
                            key={planeTicket.id}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell>{planeTicket.id}</TableCell>
                            <TableCell>{currencyFormatter.format(planeTicket.price)}</TableCell>
                            <TableCell>{planeTicket.purchaseDate}</TableCell>
                            <TableCell>{planeTicket.seatNumber}</TableCell>
                            <TableCell>
                                <TableActions entity='plane-tickets' id={planeTicket.id} />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}