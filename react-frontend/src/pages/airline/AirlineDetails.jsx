import React, { useState, useContext } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch';
import { deleteData } from '../../utils/delete';
import { editData } from '../../utils/edit.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions';
import { DataContext } from '../../store/DataContext';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import Typography from '@mui/material/Typography';
import CustomAlert from "../../components/common/Alert.jsx";

export default function AirlineDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: airline, dataExist, error, isLoading } = useFetch(ENTITIES.AIRLINES, id);
    const navigate = useNavigate();

    const [operationState, setOperationState] = useState({
        operationError: null,
        isPending: false,
    });

    const handleOperation = async (operation) => {
        try {
            setOperationState(prevState => ({ ...prevState, isPending: true }));
            let operationResult;

            if (operation === 'edit') {
                operationResult = await editData(ENTITIES.AIRLINES, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(ENTITIES.AIRLINES, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Airline Details' />

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
                            onEdit={() => navigate(`/airlines/edit/${id}`)}
                            onDelete={() => handleOperation('delete')}
                        />
                    </Box>
                </Box>
            )}
        </Box>
    );
}