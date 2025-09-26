import React from 'react';
import { ENTITIES } from '../../utils/const';
import openMap from "../../utils/openMapHelper"
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';
import Link from '@mui/material/Link';
import TableActions from '../common/table/TableActions';

export default function DestinationsListTable({ destinations }) {
    return (
        <TableContainer component={Paper}>
            <Table stickyHeader aria-label="Destinations table">
                <TableHead>
                    <TableRow>
                        <TableCell sx={{ width: '10%' }}>Id</TableCell>
                        <TableCell sx={{ width: '25%' }}>City</TableCell>
                        <TableCell sx={{ width: '45%' }}>Airport</TableCell>
                        <TableCell sx={{ width: '20%' }} align="center">Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {destinations.map((destination) => (
                        <TableRow
                            key={destination.id}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell>{destination.id}</TableCell>
                            <TableCell>
                                <Link
                                    sx={{ color: '#009be5' }}
                                    component="button"
                                    variant="body2"
                                    onClick={() => openMap(destination.city)}
                                >
                                    {destination.city}
                                </Link>
                            </TableCell>
                            <TableCell>
                                <Link
                                    sx={{ color: '#009be5' }}
                                    component="button"
                                    variant="body2"
                                    onClick={() => openMap(destination.airport)}
                                >
                                    {destination.airport}
                                </Link>
                            </TableCell>
                            <TableCell>
                                <TableActions entity={ENTITIES.DESTINATIONS} id={destination.id} />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}