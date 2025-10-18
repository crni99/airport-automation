import React, { useCallback } from 'react';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import CreateOperationSnackbarManager from '../../components/common/feedback/CreateOperationSnackbarManager.jsx';
import { useCreateOperation } from '../../hooks/useCreateOperation.jsx';

const initialFormData = { city: '', airport: '' };
const requiredFields = ['city', 'airport'];

const transformDestinationForAPI = (formData) => ({
    City: formData.city,
    Airport: formData.airport
});

export default function DestinationCreateForm() {
    const {
        city,
        airport,
        success,
        formError,
        isPending,
        handleChange,
        handleSubmit,
        setFormData,
    } = useCreateOperation(
        ENTITIES.DESTINATIONS,
        ENTITY_PATHS.DESTINATIONS,
        initialFormData,
        requiredFields,
        transformDestinationForAPI
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Destination' />

            <CreateOperationSnackbarManager
                success={success}
                formError={formError}
                handleCloseSnackbar={handleCloseSnackbar}
            />

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
                            placeholder="Belgrade"
                            required
                            error={!!formError}
                            helperText={formError}
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
                            placeholder="Belgrade Nikola Tesla Airport"
                            required
                            error={!!formError}
                            helperText={formError}
                            sx={{ width: '150%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }} sx={{ mt: 2 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="success"
                            disabled={isPending}
                        >
                            {isPending ? <CircularProgress size={24} /> : 'Create'}
                        </Button>
                    </Grid>
                </Grid>
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.DESTINATIONS} />
            </Box>
        </Box >
    );
}