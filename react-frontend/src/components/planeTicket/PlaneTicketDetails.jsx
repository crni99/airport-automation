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
import openDetails from "../../utils/openDetailsHelper.js";
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Grid from '@mui/material/Grid';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import AlertTitle from '@mui/material/AlertTitle';
import Divider from '@mui/material/Divider';

export default function PlaneTicketDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: planeTicket, dataExist, error, isLoading } = useFetch(ENTITIES.PLANE_TICKETS, id);
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
                operationResult = await editData(ENTITIES.PLANE_TICKETS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(ENTITIES.PLANE_TICKETS, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Plane Ticket Details' />
            {(isLoading || operationState.isPending) && (
                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                    <CircularProgress />
                </Box>
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
                <Box sx={{ mt: 2 }}>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Id</Typography>
                            <Typography>{planeTicket.id}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Price</Typography>
                            <Typography>{planeTicket.price}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Purchase Date</Typography>
                            <Typography>{planeTicket.purchaseDate}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Seat Number</Typography>
                            <Typography>{planeTicket.seatNumber}</Typography>
                        </Grid>
                    </Grid>
                    <Divider sx={{ my: 3 }} />
                    <Typography variant="h6" gutterBottom>Passenger Details</Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Id</Typography>
                            <Button
                                variant="contained"
                                size="small"
                                onClick={() => openDetails('Passengers', planeTicket.passenger.id)}
                            >
                                {planeTicket.passenger.id}
                            </Button>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">First Name</Typography>
                            <Typography>{planeTicket.passenger.firstName}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Last Name</Typography>
                            <Typography>{planeTicket.passenger.lastName}</Typography>
                        </Grid>
                    </Grid>
                    <Divider sx={{ my: 3 }} />
                    <Typography variant="h6" gutterBottom>Travel Class Details</Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Id</Typography>
                            <Button
                                variant="contained"
                                size="small"
                                onClick={() => openDetails('TravelClasses', planeTicket.travelClass.id)}
                            >
                                {planeTicket.travelClass.id}
                            </Button>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Type</Typography>
                            <Typography>{planeTicket.travelClass.type}</Typography>
                        </Grid>
                    </Grid>
                    <Divider sx={{ my: 3 }} />
                    <Typography variant="h6" gutterBottom>Flight Details</Typography>
                    <Grid container spacing={2}>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Id</Typography>
                            <Button
                                variant="contained"
                                size="small"
                                onClick={() => openDetails('Flights', planeTicket.flight.id)}
                            >
                                {planeTicket.flight.id}
                            </Button>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Departure Date</Typography>
                            <Typography>{planeTicket.flight.departureDate}</Typography>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Departure Time</Typography>
                            <Typography>{planeTicket.flight.departureTime}</Typography>
                        </Grid>
                    </Grid>
                    <PageNavigationActions
                        dataType={ENTITIES.PLANE_TICKETS}
                        dataId={id}
                        onEdit={() => handleOperation('edit')}
                        onDelete={() => handleOperation('delete')}
                    />
                </Box>
            )}
        </Box>
    );
}