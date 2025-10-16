import React, { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import { useEditForm } from '../../hooks/useEditForm.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import { formatTime } from '../../utils/formatting.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import UpdateSnackbarManager from '../../components/common/feedback/UpdateSnackbarManager.jsx';

const initialFormData = {
    departureDate: '',
    departureTime: '',
    airlineId: '',
    destinationId: '',
    pilotId: '',
};

const requiredFields = ['departureDate', 'departureTime', 'airlineId', 'destinationId', 'pilotId'];

const transformFlightForAPI = (formData, currentId) => {
    const formattedDepartureTime = formatTime(formData.departureTime);

    return {
        Id: currentId,
        DepartureDate: formData.departureDate,
        DepartureTime: formattedDepartureTime,
        AirlineId: formData.airlineId,
        DestinationId: formData.destinationId,
        PilotId: formData.pilotId,
    };
};

const transformFlightForForm = (fetchedData) => ({
    departureDate: fetchedData.departureDate || '',
    departureTime: fetchedData.departureTime || '',
    airlineId: String(fetchedData.airlineId || ''),
    destinationId: String(fetchedData.destinationId || ''),
    pilotId: String(fetchedData.pilotId || ''),
});

export default function FlightEditForm() {
    const { id } = useParams();

    const {
        departureDate,
        departureTime,
        airlineId,
        destinationId,
        pilotId,
        success,
        formError,
        isPending,
        isFetching,
        isFetchError,
        fetchError,
        handleChange,
        handleSubmit,
        setFormData,
    } = useEditForm(
        ENTITIES.FLIGHTS,
        ENTITY_PATHS.FLIGHTS,
        id,
        initialFormData,
        requiredFields,
        transformFlightForAPI,
        transformFlightForForm
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Edit Flight' />

            {(isFetching || isPending) && (
                <CircularProgress sx={{ mb: 2 }} />
            )}

            <UpdateSnackbarManager
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
                    <Grid container spacing={4}>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
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
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="airlineId"
                                name="airlineId"
                                label="Airline Id"
                                type="number"
                                variant="outlined"
                                value={airlineId}
                                slotProps={{ inputLabel: { shrink: true } }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="destinationId"
                                name="destinationId"
                                label="Destination Id"
                                type="number"
                                variant="outlined"
                                value={destinationId}
                                slotProps={{ inputLabel: { shrink: true } }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="pilotId"
                                name="pilotId"
                                label="Pilot Id"
                                type="number"
                                variant="outlined"
                                value={pilotId}
                                slotProps={{ inputLabel: { shrink: true } }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="departureDate"
                                name="departureDate"
                                label="Departure Date"
                                type="date"
                                variant="outlined"
                                value={departureDate}
                                onChange={handleChange}
                                required
                                fullWidth
                                slotProps={{ inputLabel: { shrink: true } }}
                                error={!!formError && requiredFields.includes('departureDate')}
                                helperText={formError && requiredFields.includes('departureDate') ? formError : ''}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="departureTime"
                                name="departureTime"
                                label="Departure Time"
                                type="time"
                                variant="outlined"
                                value={departureTime}
                                onChange={handleChange}
                                required
                                fullWidth
                                slotProps={{ inputLabel: { shrink: true } }}
                                error={!!formError && requiredFields.includes('departureTime')}
                                helperText={formError && requiredFields.includes('departureTime') ? formError : ''}
                            />
                        </Grid>
                        <Grid size={{ xs: 12 }} sx={{ mt: 1 }}>
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
                <BackToListAction dataType={ENTITIES.FLIGHTS} />
            </Box>
        </Box>
    );
}