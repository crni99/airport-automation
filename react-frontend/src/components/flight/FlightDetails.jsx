import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { deleteData } from '../../utils/delete.js';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import PageNavigationActions from '../common/pagination/PageNavigationActions.jsx';
import Alert from '../common/Alert.jsx';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import openDetails from "../../utils/openDetailsHelper";
import { ENTITIES } from '../../utils/const.js';
import openMap from '../../utils/openMapHelper.js';
import { DD } from '..//common/table/DD.jsx';
import { DT } from '..//common/table/DT.jsx';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import AlertTitle from '@mui/material/AlertTitle';
import Stack from '@mui/material/Stack';
import Typography from '@mui/material/Typography';
import Divider from '@mui/material/Divider';
import Grid from '@mui/material/Grid';
import Link from '@mui/material/Link';
import Button from '@mui/material/Button';

export default function FlightDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: flight, dataExist, error, isLoading } = useFetch(ENTITIES.FLIGHTS, id);
    const navigate = useNavigate();

    const [operationState, setOperationState] = useState({
        operationError: null,
        isPending: false
    });

    const handleOperation = async (operation) => {
        try {
            setOperationState(prevState => ({ ...prevState, isPending: true }));
            let operationResult;

            if (operation === 'edit') {
                operationResult = await editData(ENTITIES.FLIGHTS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(ENTITIES.FLIGHTS, id, dataCtx.apiUrl, navigate);
            }
            if (operationResult) {
                setOperationState(prevState => ({ ...prevState, operationError: operationResult.message }));
            }
        } catch (error) {
            setOperationState(prevState => ({ ...prevState, operationError: error.message }));
        } finally {
            setOperationState(prevState => ({ ...prevState, isPending: false }));
        }
    };

    return (
        <Box sx={{ p: 3 }}>
            <PageTitle title='Flight Details' />

            {(isLoading || operationState.isPending) && (
                <Stack direction="row" justifyContent="center" sx={{ mt: 4 }}>
                    <CircularProgress />
                </Stack>
            )}

            {error && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    <AlertTitle>Error</AlertTitle>
                    {error.message}
                </Alert>
            )}

            {operationState.operationError && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    <AlertTitle>Error</AlertTitle>
                    {operationState.operationError}
                </Alert>
            )}

            {dataExist && (
                <>
                    <Box sx={{ mt: 3 }}>
                        <Grid container spacing={8}>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Id:</Typography>
                                <Typography variant="body1" component="div">{flight.id}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Departure Date:</Typography>
                                <Typography variant="body1" component="div">{flight.departureDate}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Departure Time:</Typography>
                                <Typography variant="body1" component="div">{flight.departureTime}</Typography>
                            </Grid>
                        </Grid>
                    </Box>

                    <Divider sx={{ my: 3 }} />

                    <Box>
                        <Typography variant="h6" gutterBottom>Airline Details</Typography>
                        <Grid container spacing={8}>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Id:</Typography>
                                <Link
                                    component="button"
                                    variant="body1"
                                    onClick={() => navigate(`/airlines/${flight.airline.id}`)}
                                >
                                    {flight.airline.id}
                                </Link>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Name:</Typography>
                                <Typography variant="body1" component="div">{flight.airline.name}</Typography>
                            </Grid>
                        </Grid>
                    </Box>

                    <Divider sx={{ my: 3 }} />

                    <Box>
                        <Typography variant="h6" gutterBottom>Destination Details</Typography>
                        <Grid container spacing={8}>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Id:</Typography>
                                <Link
                                    component="button"
                                    variant="body1"
                                    onClick={() => navigate(`/destinations/${flight.destination.id}`)}
                                >
                                    {flight.destination.id}
                                </Link>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>City:</Typography>
                                <Link
                                    component="button"
                                    variant="body1"
                                    onClick={() => openMap(flight.destination.city)}
                                >
                                    {flight.destination.city}
                                </Link>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Airport:</Typography>
                                <Link
                                    component="button"
                                    variant="body1"
                                    onClick={() => openMap(flight.destination.airport)}
                                >
                                    {flight.destination.airport}
                                </Link>
                            </Grid>
                        </Grid>
                    </Box>

                    <Divider sx={{ my: 3 }} />

                    <Box sx={{ mb: 3 }}>
                        <Typography variant="h6" gutterBottom>Pilot Details</Typography>
                        <Grid container spacing={8}>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Id:</Typography>
                                <Link
                                    component="button"
                                    variant="body1"
                                    onClick={() => navigate(`/pilots/${flight.pilot.id}`)}
                                >
                                    {flight.pilot.id}
                                </Link>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>First Name:</Typography>
                                <Typography variant="body1" component="div">{flight.pilot.firstName}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Last Name:</Typography>
                                <Typography variant="body1" component="div">{flight.pilot.lastName}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>UPRN:</Typography>
                                <Typography variant="body1" component="div">{flight.pilot.uprn}</Typography>
                            </Grid>
                            <Grid item xs={12} sm={6}>
                                <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Flying Hours:</Typography>
                                <Typography variant="body1" component="div">{flight.pilot.flyingHours}</Typography>
                            </Grid>
                        </Grid>
                    </Box>

                    <PageNavigationActions dataType={ENTITIES.FLIGHTS} dataId={id} onEdit={() => handleOperation('edit')} onDelete={() => handleOperation('delete')} />
                </>
            )}
        </Box>
    );
}