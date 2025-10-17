import React from 'react';
import { useParams } from 'react-router-dom';
import useFetch from '../../hooks/useFetch';
import { useNavigate } from 'react-router-dom';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import DetailActionSnackbarManager from '../../components/common/feedback/DetailActionSnackbarManager.jsx';
import { useDeleteOperation } from '../../hooks/useDeleteOperation.jsx';

export default function AirlineDetails() {
    const { id } = useParams();
    const { data: airline, dataExist, error, isLoading } = useFetch(ENTITIES.AIRLINES, id);

    const navigate = useNavigate();

    const { operationState, handleCloseSnackbar, handleOperation } = useDeleteOperation(
        ENTITIES.AIRLINES,
        id,
        ENTITY_PATHS.AIRLINES
    );

    if (isLoading) {
        return (
            <Box sx={{ mt: 5 }}>
                <PageTitle title='Airline Details' />
                <CircularProgress sx={{ mb: 0 }} />
            </Box>
        );
    }

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Airline Details' />

            {operationState.isPending && (
                <CircularProgress sx={{ mb: 0 }} />
            )}

            <DetailActionSnackbarManager
                operationState={operationState}
                error={error}
                handleCloseSnackbar={handleCloseSnackbar}
            />

            {dataExist && (
                <Box sx={{ mt: 3 }}>
                    <Grid container spacing={6}>
                        <Grid>
                            <Typography variant="subtitle1" fontWeight="bold">Id</Typography>
                            <Typography variant="body1" sx={{ mt: 1 }}>
                                {airline.id}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" fontWeight="bold">Name</Typography>
                            <Typography variant="body1" sx={{ mt: 1 }}>
                                {airline.name}
                            </Typography>
                        </Grid>
                    </Grid>
                    <Box sx={{ mt: 5 }}>
                        <PageNavigationActions
                            dataType={ENTITIES.AIRLINES}
                            dataId={id}
                            onEdit={() => navigate(`${ENTITY_PATHS.AIRLINES}/edit/${id}`)}
                            onDelete={() => handleOperation('delete')}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    );
}