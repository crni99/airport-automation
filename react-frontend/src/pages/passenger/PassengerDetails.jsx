import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { deleteData } from '../../utils/delete.js';
import { editData } from '../../utils/edit.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import PageNavigationActions from '../../components/common/pagination/PageNavigationActions.jsx';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import openMap from '../../utils/openMapHelper.js';
import { ENTITIES } from '../../utils/const.js';
import MapEmbed from '../../components/common/MapEmbed.jsx';
import Typography from '@mui/material/Typography';
import CircularProgress from '@mui/material/CircularProgress';
import Link from '@mui/material/Link';
import { Box, Grid } from '@mui/material';
import CustomAlert from "../../components/common/Alert.jsx";

export default function PassengerDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: passenger, dataExist, error, isLoading } = useFetch(ENTITIES.PASSENGERS, id);
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
                operationResult = await editData(ENTITIES.PASSENGERS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(ENTITIES.PASSENGERS, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Passenger Details' />

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
                <>
                    <Grid container spacing={24} sx={{ mt: 3 }}>
                        <Grid item xs={12} md={6}>
                            <Box component="dl">
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Id</Typography>
                                    <Typography component="dd" variant="body1">{passenger.id}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>First Name</Typography>
                                    <Typography component="dd" variant="body1">{passenger.firstName}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Last Name</Typography>
                                    <Typography component="dd" variant="body1">{passenger.lastName}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>UPRN</Typography>
                                    <Typography component="dd" variant="body1">{passenger.uprn}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Passport</Typography>
                                    <Typography component="dd" variant="body1">{passenger.passport}</Typography>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Address</Typography>
                                    <Link
                                        component="dd"
                                        variant="body1"
                                        sx={{ cursor: 'pointer' }}
                                        onClick={() => openMap(passenger.address)}
                                    >
                                        {passenger.address}
                                    </Link>
                                </Box>
                                <Box sx={{ mb: 3 }}>
                                    <Typography component="dt" variant="subtitle1" sx={{ fontWeight: 'bold' }}>Phone</Typography>
                                    <Typography component="dd" variant="body1">{passenger.phone}</Typography>
                                </Box>
                            </Box>
                            <Box sx={{ mt: 5, mb: 5 }}>
                                <PageNavigationActions
                                    dataType={ENTITIES.PASSENGERS}
                                    dataId={id}
                                    onEdit={() => handleOperation('edit')}
                                    onDelete={() => handleOperation('delete')}
                                />
                            </Box>
                        </Grid>
                        <Grid sx={{ mb: 3, width: '50%' }}>
                            <MapEmbed address={passenger.address} />
                        </Grid>
                    </Grid>
                </>
            )}
        </Box>
    );
}
