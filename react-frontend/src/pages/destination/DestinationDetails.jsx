import React, { useContext } from 'react';
import { useParams } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { DataContext } from '../../store/DataContext.jsx';
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
import { CustomSnackbar } from "../../components/common/CustomSnackbar.jsx";
import { useDataOperation } from '../../hooks/useDataOperation.jsx';

export default function DestinationDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: destination, dataExist, error, isLoading } = useFetch(ENTITIES.DESTINATIONS, id);
    const { operationState, handleCloseSnackbar, handleOperation } = useDataOperation(
        ENTITIES.DESTINATIONS,
        id,
        dataCtx.apiUrl,
        ENTITY_PATHS.DESTINATIONS
    );

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Destination Details' />

            {(isLoading || operationState.isPending) && (
                <CircularProgress sx={{ mb: 0 }} />
            )}

            {operationState.operationSuccess && (
                <CustomSnackbar
                    severity='success'
                    message={operationState.operationSuccess}
                    onClose={handleCloseSnackbar}
                />
            )}

            {error && (
                <CustomSnackbar
                    severity='error'
                    message={error.message}
                    onClose={handleCloseSnackbar}
                />
            )}

            {operationState.operationError && (
                <CustomSnackbar
                    severity='error'
                    message={operationState.operationError.message}
                    onClose={handleCloseSnackbar}
                />
            )}

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
                                    onEdit={() => handleOperation('edit')}
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