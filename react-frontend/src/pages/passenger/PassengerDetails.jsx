import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions.jsx';
import openMap from '../../utils/openMapHelper.js';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import MapEmbed from '../../components/common/MapEmbed.jsx';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import Link from '@mui/material/Link';
import { Box, Grid } from '@mui/material';
import DeleteOperationSnackbarManager from '../../components/common/feedback/DeleteOperationSnackbarManager.jsx';
import { useDelete } from '../../hooks/useDelete.jsx';

export default function PassengerDetails() {
    const { id } = useParams();
    const [triggerFetch, setTriggerFetch] = useState(true);
    const { data: passenger, dataExist, error, isLoading } = useFetch(ENTITIES.PASSENGERS, id, undefined, undefined, triggerFetch)

    const navigate = useNavigate();

    const { operationState, handleCloseSnackbar, handleOperation } = useDelete(
        ENTITIES.PASSENGERS,
        id,
        ENTITY_PATHS.PASSENGERS
    );

    useEffect(() => {
        if (passenger) {
            setTriggerFetch(false);
        }
    }, [passenger]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Passenger Details' />

            {(isLoading || operationState.isPending) && (
                <CircularProgress sx={{ mb: 0 }} />
            )}

            <DeleteOperationSnackbarManager
                operationState={operationState}
                error={error}
                handleCloseSnackbar={handleCloseSnackbar}
            />

            {dataExist && (
                <>
                    <Grid container spacing={24} sx={{ mt: 3 }}>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                            <Box component="dl">
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Id</Typography>
                                    <Typography component="dd" variant="body1">{passenger.id}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>First Name</Typography>
                                    <Typography component="dd" variant="body1">{passenger.firstName}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Last Name</Typography>
                                    <Typography component="dd" variant="body1">{passenger.lastName}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>UPRN</Typography>
                                    <Typography component="dd" variant="body1">{passenger.uprn}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Passport</Typography>
                                    <Typography component="dd" variant="body1">{passenger.passport}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Address</Typography>
                                    <Link
                                        component="dd"
                                        variant="body1"
                                        sx={{ cursor: 'pointer' }}
                                        onClick={() => openMap(passenger.address)}
                                    >
                                        {passenger.address}
                                    </Link>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Phone</Typography>
                                    <Typography component="dd" variant="body1">{passenger.phone}</Typography>
                                </Box>
                            </Box>

                        </Grid>
                        <Grid sx={{ mb: 3, width: '50%' }}>
                            <MapEmbed address={passenger.address} />
                            <Box sx={{ mt: 5, mb: 5 }}>
                                <PageNavigationActions
                                    dataType={ENTITIES.PASSENGERS}
                                    dataId={id}
                                    onEdit={() => navigate(`${ENTITY_PATHS.PASSENGERS}/edit/${id}`)}
                                    onDelete={() => handleOperation('delete')}
                                />
                            </Box>
                        </Grid>
                    </Grid>
                </>
            )}
        </Box>
    );
}
