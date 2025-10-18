import React, { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import UpdateOperationSnackbarManager from '../../components/common/feedback/UpdateOperationSnackbarManager.jsx';
import { useUpdateOperation } from '../../hooks/useUpdateOperation.jsx';

const initialFormData = {
    price: '',
    purchaseDate: '',
    seatNumber: '',
    passengerId: '',
    travelClassId: '',
    flightId: '',
};

const requiredFields = ['price', 'purchaseDate', 'seatNumber', 'passengerId', 'travelClassId', 'flightId'];

const transformPlaneTicketForAPI = (formData, currentId) => ({
    Id: currentId,
    Price: formData.price,
    PurchaseDate: formData.purchaseDate,
    SeatNumber: formData.seatNumber,
    PassengerId: formData.passengerId,
    TravelClassId: formData.travelClassId,
    FlightId: formData.flightId,
});

const transformPlaneTicketForForm = (fetchedData) => ({
    price: String(fetchedData.price || ''),
    purchaseDate: fetchedData.purchaseDate || '',
    seatNumber: fetchedData.seatNumber || '',
    passengerId: String(fetchedData.passengerId || ''),
    travelClassId: String(fetchedData.travelClassId || ''),
    flightId: String(fetchedData.flightId || ''),
});

export default function PlaneTicketEditForm() {
    const { id } = useParams();

    const {
        price,
        purchaseDate,
        seatNumber,
        passengerId,
        travelClassId,
        flightId,
        success,
        formError,
        isPending,
        isFetching,
        isFetchError,
        fetchError,
        handleChange,
        handleSubmit,
        setFormData,
    } = useUpdateOperation(
        ENTITIES.PLANE_TICKETS,
        ENTITY_PATHS.PLANE_TICKETS,
        id,
        initialFormData,
        requiredFields,
        transformPlaneTicketForAPI,
        transformPlaneTicketForForm
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Edit Plane Ticket' />

            {(isFetching || isPending) && (
                <CircularProgress sx={{ mb: 2 }} />
            )}

            <UpdateOperationSnackbarManager
                success={success}
                formError={formError}
                fetchError={isFetchError ? fetchError : null}
                handleCloseSnackbar={handleCloseSnackbar}
            />

            {!isFetching && !isFetchError && (
                <Box
                    component="form"
                    autoComplete="off"
                    onSubmit={handleSubmit}
                >
                    <Grid container spacing={3}>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="id"
                                name="id"
                                label="Id"
                                type="number"
                                variant="outlined"
                                value={id}
                                slotProps={{ inputLabel: { shrink: true } }}
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="passengerId"
                                name="passengerId"
                                label="Passenger Id"
                                type="number"
                                variant="outlined"
                                value={passengerId}
                                slotProps={{ inputLabel: { shrink: true } }}
                                required
                                fullWidth
                            />

                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="travelClassId"
                                name="travelClassId"
                                label="Travel Class Id"
                                type="number"
                                variant="outlined"
                                value={travelClassId}
                                slotProps={{ inputLabel: { shrink: true } }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="flightId"
                                name="flightId"
                                label="Flight Id"
                                type="number"
                                variant="outlined"
                                value={flightId}
                                slotProps={{ inputLabel: { shrink: true } }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                            <TextField
                                id="price"
                                name="price"
                                label="Price"
                                type="number"
                                variant="outlined"
                                value={price}
                                onChange={handleChange}
                                required
                                fullWidth
                                error={!!formError && requiredFields.includes('price')}
                                helperText={formError && requiredFields.includes('price') ? formError : ''}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                            <TextField
                                id="seatNumber"
                                name="seatNumber"
                                label="Seat Number"
                                type="number"
                                variant="outlined"
                                value={seatNumber}
                                onChange={handleChange}
                                required
                                fullWidth
                                error={!!formError && requiredFields.includes('seatNumber')}
                                helperText={formError && requiredFields.includes('seatNumber') ? formError : ''}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                            <TextField
                                id="purchaseDate"
                                name="purchaseDate"
                                label="Purchase Date"
                                type="date"
                                variant="outlined"
                                value={purchaseDate}
                                onChange={handleChange}
                                required
                                fullWidth
                                slotProps={{ inputLabel: { shrink: true } }}
                                error={!!formError && requiredFields.includes('purchaseDate')}
                                helperText={formError && requiredFields.includes('purchaseDate') ? formError : ''}
                            />
                        </Grid>
                        <Grid size={{ xs: 12 }} sx={{ mt: 3 }}>
                            <Button
                                type="submit"
                                variant="contained"
                                color="success"
                                disabled={isPending || !!formError}
                            >
                                {isPending ? <CircularProgress size={24} /> : 'Save Changes'}
                            </Button>
                        </Grid>
                    </Grid>
                </Box>
            )}
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PLANE_TICKETS} />
            </Box>
        </Box>
    );
}