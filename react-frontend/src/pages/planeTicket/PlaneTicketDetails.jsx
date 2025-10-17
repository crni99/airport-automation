import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions.jsx';
import openDetails from "../../utils/openDetailsHelper.js";
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import Typography from '@mui/material/Typography';
import Link from '@mui/material/Link';
import Grid from '@mui/material/Grid';
import CircularProgress from '@mui/material/CircularProgress';
import Divider from '@mui/material/Divider';
import DetailActionSnackbarManager from '../../components/common/feedback/DetailActionSnackbarManager.jsx';
import { useDeleteOperation } from '../../hooks/useDeleteOperation.jsx';

export default function PlaneTicketDetails() {
    const { id } = useParams();
    const { data: planeTicket, dataExist, error, isLoading } = useFetch(ENTITIES.PLANE_TICKETS, id);

    const navigate = useNavigate();

    const { operationState, handleCloseSnackbar, handleOperation } = useDeleteOperation(
        ENTITIES.PLANE_TICKETS,
        id,
        ENTITY_PATHS.PLANE_TICKETS
    );

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Plane Ticket Details' />

            {(isLoading || operationState.isPending) && (
                <CircularProgress sx={{ mb: 0 }} />
            )}

            <DetailActionSnackbarManager
                operationState={operationState}
                error={error}
                handleCloseSnackbar={handleCloseSnackbar}
            />

            {dataExist && (
                <Box sx={{ mt: 3 }}>
                    <Grid container spacing={8}>
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
                    <Divider sx={{ my: 4 }} />
                    <Typography variant="h4" gutterBottom>Passenger Details</Typography>
                    <Grid container spacing={8}>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Id</Typography>
                            <Link
                                component="button"
                                variant="body1"
                                onClick={() => openDetails('Passengers', planeTicket.passenger.id)}
                            >
                                {planeTicket.passenger.id}
                            </Link>
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
                    <Divider sx={{ my: 4 }} />
                    <Typography variant="h4" gutterBottom>Travel Class Details</Typography>
                    <Grid container spacing={8}>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Id</Typography>
                            <Link
                                component="button"
                                variant="body1"
                                onClick={() => openDetails('TravelClasses', planeTicket.travelClass.id)}
                            >
                                {planeTicket.travelClass.id}
                            </Link>
                        </Grid>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" fontWeight="bold">Type</Typography>
                            <Typography>{planeTicket.travelClass.type}</Typography>
                        </Grid>
                    </Grid>
                    <Divider sx={{ my: 4 }} />
                    <Typography variant="h4" gutterBottom>Flight Details</Typography>
                    <Grid container spacing={8}>
                        <Grid item xs={12} sm={6}>
                            <Typography variant="subtitle1" component="div" sx={{ fontWeight: 'bold' }}>Id</Typography>
                            <Link
                                component="button"
                                variant="body1"
                                onClick={() => openDetails('Flights', planeTicket.flight.id)}
                            >
                                {planeTicket.flight.id}
                            </Link>
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
                    <Divider sx={{ my: 4 }} />
                    <Box sx={{ mt: 5, mb: 5 }}>
                        <PageNavigationActions
                            dataType={ENTITIES.PLANE_TICKETS}
                            dataId={id}
                            onEdit={() => navigate(`${ENTITY_PATHS.PLANE_TICKETS}/edit/${id}`)}
                            onDelete={() => handleOperation('delete')}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    );
}