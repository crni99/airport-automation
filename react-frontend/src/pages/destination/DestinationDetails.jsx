import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import openMap from '../../utils/openMapHelper.js'
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import Link from '@mui/material/Link';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions.jsx';
import MapEmbed from '../../components/common/MapEmbed.jsx';
import DeleteOperationSnackbarManager from '../../components/common/feedback/DeleteOperationSnackbarManager.jsx';
import { useDeleteOperation } from '../../hooks/useDeleteOperation.jsx';

export default function DestinationDetails() {
    const { id } = useParams();
    const { data: destination, dataExist, error, isLoading } = useFetch(ENTITIES.DESTINATIONS, id);

    const navigate = useNavigate();

    const { operationState, handleCloseSnackbar, handleOperation } = useDeleteOperation(
        ENTITIES.DESTINATIONS,
        id,
        ENTITY_PATHS.DESTINATIONS
    );

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Destination Details' />

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
                        <Grid>
                            <Box component="dl">
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Id</Typography>
                                    <Typography component="dd" variant="body1">{destination.id}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>City</Typography>
                                    <Link
                                        component="dd"
                                        variant="body1"
                                        sx={{ cursor: 'pointer' }}
                                        onClick={() => openMap(destination.city)}
                                    >
                                        {destination.city}
                                    </Link>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Airport</Typography>
                                    <Link
                                        component="dd"
                                        variant="body1"
                                        sx={{ cursor: 'pointer' }}
                                        onClick={() => openMap(destination.airport)}
                                    >
                                        {destination.airport}
                                    </Link>
                                </Box>
                            </Box>
                            <Box sx={{ mt: 5 }}>
                                <PageNavigationActions
                                    dataType={ENTITIES.DESTINATIONS}
                                    dataId={id}
                                    onEdit={() => navigate(`${ENTITY_PATHS.DESTINATIONS}/edit/${id}`)}
                                    onDelete={() => handleOperation('delete')}
                                />
                            </Box>
                        </Grid>
                        <Grid sx={{ mb: 3, width: '50%' }}>
                            <MapEmbed address={`${destination.city} ${destination.airport}`} />
                        </Grid>
                    </Grid>

                </>
            )}
        </Box>
    );
}