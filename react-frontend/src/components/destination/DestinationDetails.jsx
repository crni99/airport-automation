import React, { useState, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { deleteData } from '../../utils/delete.js';
import { editData } from '../../utils/edit.js';
import { DataContext } from '../../store/DataContext.jsx';
import { ENTITIES } from '../../utils/const.js';
import openMap from '../../utils/openMapHelper.js'
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import Link from '@mui/material/Link';
import { Stack } from '@mui/material';
import PageTitle from '../common/PageTitle.jsx';
import PageNavigationActions from '../common/pagination/PageNavigationActions.jsx';
import MapEmbed from '../common/MapEmbed.jsx';

export default function DestinationDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: destination, dataExist, error, isLoading } = useFetch(ENTITIES.DESTINATIONS, id);
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
                operationResult = await editData(ENTITIES.DESTINATIONS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(ENTITIES.DESTINATIONS, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Destination Details' />

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
                        </Grid>
                        <Grid sx={{ mb: 3, width: '50%' }}>
                            <MapEmbed address={`${destination.city} ${destination.airport}`} />
                        </Grid>
                    </Grid>
                    <PageNavigationActions
                        dataType={ENTITIES.DESTINATIONS}
                        dataId={id}
                        onEdit={() => handleOperation('edit')}
                        onDelete={() => handleOperation('delete')}
                    />
                </>
            )}
        </Box>
    );
}