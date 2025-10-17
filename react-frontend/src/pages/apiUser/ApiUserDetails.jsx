import React from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import DetailActionSnackbarManager from '../../components/common/feedback/DetailActionSnackbarManager.jsx';
import { useDeleteOperation } from '../../hooks/useDeleteOperation.jsx';

export default function ApiUserDetails() {
    const { id } = useParams();
    const { data: apiUser, dataExist, error, isLoading } = useFetch(ENTITIES.API_USERS, id);

    const navigate = useNavigate();

    const { operationState, handleCloseSnackbar, handleOperation } = useDeleteOperation(
        ENTITIES.API_USERS,
        id,
        ENTITY_PATHS.API_USERS
    );

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title="Api User Details" />

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
                    <Grid container spacing={6}>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                Id
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {apiUser.apiUserId}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                Username
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {apiUser.userName}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                Password
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {apiUser.password}
                            </Typography>
                        </Grid>
                        <Grid>
                            <Typography variant="subtitle1" component="dt" sx={{ fontWeight: 'bold' }}>
                                Roles
                            </Typography>
                            <Typography variant="body1" component="dd" sx={{ mt: 1 }}>
                                {apiUser.roles}
                            </Typography>
                        </Grid>
                    </Grid>
                    <Box sx={{ mt: 5 }}>
                        <PageNavigationActions
                            dataType={ENTITIES.API_USERS}
                            dataId={id}
                            onEdit={() => navigate(`/api-users/edit/${id}`)}
                            onDelete={() => handleOperation('delete')}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    );
}