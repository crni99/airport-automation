import React from 'react';
import useFetch from "../../hooks/useFetch";
import { ENTITIES } from '../../utils/const.js';
import { Container } from '@mui/material';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import Chip from '@mui/material/Chip';
import Stack from '@mui/material/Stack';
import Divider from '@mui/material/Divider';
import Paper from '@mui/material/Paper';
import Table from '@mui/material/Table';
import TableBody from '@mui/material/TableBody';
import TableCell from '@mui/material/TableCell';
import TableContainer from '@mui/material/TableContainer';
import TableHead from '@mui/material/TableHead';
import TableRow from '@mui/material/TableRow';
import CustomAlert from './Alert.jsx';

export default function HealthCheck() {
    const { data, dataExist, error, isLoading, isError } = useFetch(ENTITIES.HEALTH_CHECKS, null, null, null);

    const extractErrorMessage = (error) => {
        if (error && error.message) {
            return error.message;
        }
        return "An unknown error occurred";
    };

    return (
        <Container sx={{ mt: 4 }}>
            <Box sx={{ p: 3 }}>
                {isLoading && (
                    <Stack sx={{ mb: 3}}>
                        <CircularProgress />
                    </Stack>
                )}

                {isError && error && (
                    <CustomAlert alertType='error' type='Error' message={extractErrorMessage(error)} sx={{ mb:3 }} />
                )}

                {data && dataExist && !isError ? (
                    <Box sx={{ mt: 3 }}>
                        <Stack direction={{ xs: 'column', sm: 'row' }} spacing={2} alignItems={{ xs: 'flex-start', sm: 'center' }}>
                            <Typography variant="subtitle1" sx={{ fontWeight: 'bold' }}>
                                Status:
                            </Typography>
                            {data.status === "Healthy" ? (
                                <Chip label={data.status} color="success" />
                            ) : (
                                <Chip label={data.status} color="error" />
                            )}
                            <Typography variant="subtitle1" sx={{ fontWeight: 'bold', pl: 4 }}>
                                Total Duration:
                            </Typography>
                            <Typography variant="body1">
                                {data.totalDuration}
                            </Typography>
                        </Stack>

                        <Divider sx={{ my: 3 }} />

                        <TableContainer component={Paper} elevation={0}>
                            <Table aria-label="health check table">
                                <TableHead>
                                    <TableRow>
                                        <TableCell sx={{ width: '20%' }}>Name</TableCell>
                                        <TableCell sx={{ width: '40%' }}>Description</TableCell>
                                        <TableCell sx={{ width: '20%' }}>Duration</TableCell>
                                        <TableCell sx={{ width: '20%' }} align="center">Status</TableCell>
                                    </TableRow>
                                </TableHead>
                                <TableBody>
                                    {Object.keys(data.entries).map((key) => (
                                        <TableRow key={key}>
                                            <TableCell>{key}</TableCell>
                                            <TableCell>{data.entries[key].description}</TableCell>
                                            <TableCell>{data.entries[key].duration}</TableCell>
                                            <TableCell align="center">
                                                {data.entries[key].status === "Healthy" ? (
                                                    <Chip label={data.entries[key].status} color="success" />
                                                ) : (
                                                    <Chip label={data.entries[key].status} color="error" />
                                                )}
                                            </TableCell>
                                        </TableRow>
                                    ))}
                                </TableBody>
                            </Table>
                        </TableContainer>
                    </Box>
                ) : (
                    <CustomAlert alertType='info' type='Info' message='No data available' />
                )}
            </Box>
        </Container>
    );
}