import React, { useContext } from 'react';
import { useParams } from 'react-router-dom';
import useFetch from '../../hooks/useFetch';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions';
import { DataContext } from '../../store/DataContext';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import { CustomSnackbar } from "../../components/common/CustomSnackbar.jsx";
import { useDataOperation } from '../../hooks/useDataOperation.jsx';

export default function AirlineDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: airline, dataExist, error, isLoading } = useFetch(ENTITIES.AIRLINES, id);
    const { operationState, handleCloseSnackbar, handleOperation } = useDataOperation(
        ENTITIES.AIRLINES,
        id,
        dataCtx.apiUrl,
        ENTITY_PATHS.AIRLINES
    );

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Airline Details' />

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
                            onEdit={() => handleOperation('edit')}
                            onDelete={() => handleOperation('delete')}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    );
}