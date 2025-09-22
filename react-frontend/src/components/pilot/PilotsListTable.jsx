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

export default function PilotsListTable({ pilots }) {
    return (
        <TableContainer component={Paper}>
            <Table stickyHeader aria-label="Pilots table">
                <TableHead>
                    <TableRow>
                        <TableCell sx={{ width: '10%'}}>Id</TableCell>
                        <TableCell sx={{ width: '20%'}}>First Name</TableCell>
                        <TableCell sx={{ width: '30%'}}>Last Name</TableCell>
                        <TableCell sx={{ width: '15%'}}>UPRN</TableCell>
                        <TableCell sx={{ width: '10%'}}>Flying Hours</TableCell>
                        <TableCell sx={{ width: '15%'}}>Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {pilots.map((pilot) => (
                        <TableRow
                            key={pilot.id}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell component="th" scope="row">
                                {pilot.id}
                            </TableCell>
                            <TableCell>{pilot.firstName}</TableCell>
                            <TableCell>{pilot.lastName}</TableCell>
                            <TableCell>{pilot.uprn}</TableCell>
                            <TableCell>{pilot.flyingHours}</TableCell>
                            <TableCell>
                                <TableActions entity={ENTITIES.PILOTS} id={pilot.id} />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}