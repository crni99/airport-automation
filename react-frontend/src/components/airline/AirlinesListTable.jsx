import React from 'react';
import TableActions from '../common/table/TableActions';
import { ENTITIES } from '../../utils/const';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import Paper from '@mui/material/Paper';

export default function AirlinesListTable({ airlines }) {
    return (
        <TableContainer component={Paper}>
            <Table stickyHeader aria-label="Airlines table">
                <TableHead>
                    <TableRow>
                        <TableCell sx={{ width: '20%' }}>Id</TableCell>
                        <TableCell sx={{ width: '60%' }}>Name</TableCell>
                        <TableCell sx={{ width: '20%' }}>Actions</TableCell>
                    </TableRow>
                </TableHead>
                <TableBody>
                    {airlines.map((airline) => (
                        <TableRow
                            key={airline.id}
                            sx={{ '&:last-child td, &:last-child th': { border: 0 } }}
                        >
                            <TableCell>{airline.id}</TableCell>
                            <TableCell>{airline.name}</TableCell>
                            <TableCell>
                                <TableActions entity={ENTITIES.AIRLINES} id={airline.id} />
                            </TableCell>
                        </TableRow>
                    ))}
                </TableBody>
            </Table>
        </TableContainer>
    );
}