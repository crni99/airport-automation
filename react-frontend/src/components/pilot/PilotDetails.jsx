import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { deleteData } from '../../utils/delete.js';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import PageNavigationActions from '../common/pagination/PageNavigationActions.jsx';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import CustomAlert from '../common/Alert.jsx';

export default function PilotDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: pilot, dataExist, error, isLoading } = useFetch(ENTITIES.PILOTS, id);
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
                operationResult = await editData(ENTITIES.PILOTS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(ENTITIES.PILOTS, id, dataCtx.apiUrl, navigate);
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
        <Box sx={{ mt: 5 }}>
            <PageTitle title="Pilot Details" />

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
                            onEdit={() => navigate(`/pilots/edit/${id}`)}
                            onDelete={() => handleOperation('delete')}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    );
}