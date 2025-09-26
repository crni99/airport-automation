import React, { useState, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { deleteData } from '../../utils/delete.js';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import PageNavigationActions from '../common/pagination/PageNavigationActions.jsx';
import { DataContext } from '../../store/DataContext.jsx';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import CustomAlert from '../common/Alert.jsx';

export default function ApiUserDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: apiUser, dataExist, error, isLoading } = useFetch(ENTITIES.API_USERS, id);
    const navigate = useNavigate();

    const [operationState, setOperationState] = useState({
        operationError: null,
        isPending: false,
    });

    const handleOperation = async (operation) => {
        try {
            setOperationState((prevState) => ({ ...prevState, isPending: true }));
            let operationResult;

            if (operation === 'edit') {
                operationResult = await editData(ENTITIES.API_USERS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(ENTITIES.API_USERS, id, dataCtx.apiUrl, navigate);
            }
            if (operationResult) {
                setOperationState((prevState) => ({ ...prevState, operationError: operationResult.message }));
            }
        } catch (error) {
            setOperationState((prevState) => ({ ...prevState, operationError: error.message }));
        } finally {
            setOperationState((prevState) => ({ ...prevState, isPending: false }));
        }
    };

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title="Api User Details" />

            {(isLoading || operationState.isPending) && (
                <CircularProgress sx={{ mb: 0 }} />
            )}

            {error && (
                <CustomAlert alertType='error' type={error.type} message={error.message} />
            )}

            {operationState.operationError && (
                <CustomAlert alertType='error' type='Error' message={operationState.operationError} />
            )}

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