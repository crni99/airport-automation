import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import DeleteOperationSnackbarManager from '../../components/common/feedback/DeleteOperationSnackbarManager.jsx';
import { useDeleteOperation } from '../../hooks/useDeleteOperation.jsx';

export default function PilotDetails() {
    const { id } = useParams();
    const [triggerFetch, setTriggerFetch] = useState(true);
    const { data: pilot, dataExist, error, isLoading } = useFetch(ENTITIES.PILOTS, id, undefined, undefined, triggerFetch)

    const navigate = useNavigate();

    const { operationState, handleCloseSnackbar, handleOperation } = useDeleteOperation(
        ENTITIES.PILOTS,
        id,
        ENTITY_PATHS.PILOTS
    );

    useEffect(() => {
        if (pilot) {
            setTriggerFetch(false);
        }
    }, [pilot]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title="Pilot Details" />

            {(isLoading || operationState.isPending) && (
                <CircularProgress sx={{ mb: 0 }} />
            )}

            <DeleteOperationSnackbarManager
                operationState={operationState}
                error={error}
                handleCloseSnackbar={handleCloseSnackbar}
            />

            {dataExist && (
                <Box sx={{ mt: 3 }}>
                    <Grid container spacing={6}>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                Id
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {pilot.id}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                First Name
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {pilot.firstName}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                Last Name
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {pilot.lastName}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                UPRN
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {pilot.uprn}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                Flying Hours
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {pilot.flyingHours}
                            </Typography>
                        </Grid>
                    </Grid>
                    <Box sx={{ mt: 5 }}>
                        <PageNavigationActions
                            dataType={ENTITIES.PILOTS}
                            dataId={id}
                            onEdit={() => navigate(`${ENTITY_PATHS.PILOTS}/edit/${id}`)}
                            onDelete={() => handleOperation('delete')}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    );
}