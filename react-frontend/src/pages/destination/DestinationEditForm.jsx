import React, { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useUpdate } from '../../hooks/useUpdate.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import UpdateOperationSnackbarManager from '../../components/common/feedback/UpdateOperationSnackbarManager.jsx';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';

const initialFormData = { city: '', airport: '' };
const requiredFields = ['city', 'airport'];

const transformDestinationForAPI = (formData, currentId) => ({
    Id: currentId,
    City: formData.city,
    Airport: formData.airport,
});

const transformDestinationForForm = (fetchedData) => ({
    city: fetchedData.city || '',
    airport: fetchedData.airport || '',
});

export default function DestinationEditForm() {
    const { id } = useParams();

    const {
        city,
        airport,
        success,
        formError,
        validationError,
        isPending,
        isFetching,
        isFetchError,
        fetchError,
        handleChange,
        handleSubmit,
        setFormData,
    } = useUpdate(
        ENTITIES.DESTINATIONS,
        ENTITY_PATHS.DESTINATIONS,
        id,
        initialFormData,
        requiredFields,
        transformDestinationForAPI,
        transformDestinationForForm
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title="Edit Destination" />

            {(isFetching || isPending) && (
                <CircularProgress sx={{ mb: 2 }} />
            )}

            <UpdateOperationSnackbarManager
                success={success}
                formError={formError}
                fetchError={fetchError}
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
                                id="city"
                                name="city"
                                label="City"
                                variant="outlined"
                                value={city}
                                onChange={handleChange}
                                error={!!validationError?.city}
                                helperText={validationError?.city || ' '}
                                sx={{ width: '100%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="airport"
                                name="airport"
                                label="Airport"
                                variant="outlined"
                                value={airport}
                                onChange={handleChange}
                                error={!!validationError?.airport}
                                helperText={validationError?.airport || ' '}
                                sx={{ width: '150%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12 }} sx={{ mt: 2 }}>
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
                <BackToListAction dataType={ENTITIES.DESTINATIONS} />
            </Box>
        </Box>
    );
}