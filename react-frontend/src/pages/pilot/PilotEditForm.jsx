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

const initialFormData = {
    firstName: '',
    lastName: '',
    uprn: '',
    flyingHours: '',
};

const requiredFields = ['firstName', 'lastName', 'uprn', 'flyingHours'];

const transformPilotForAPI = (formData, currentId) => ({
    Id: currentId,
    FirstName: formData.firstName,
    LastName: formData.lastName,
    UPRN: formData.uprn,
    FlyingHours: formData.flyingHours,
});

const transformPilotForForm = (fetchedData) => ({
    firstName: fetchedData.firstName || '',
    lastName: fetchedData.lastName || '',
    uprn: fetchedData.uprn || '',
    flyingHours: String(fetchedData.flyingHours || ''),
});

export default function PilotEditForm() {
    const { id } = useParams();

    const {
        firstName,
        lastName,
        uprn,
        flyingHours,
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
        ENTITIES.PILOTS,
        ENTITY_PATHS.PILOTS,
        id,
        initialFormData,
        requiredFields,
        transformPilotForAPI,
        transformPilotForForm
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Edit Pilot' />

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
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
                            <TextField
                                id="firstName"
                                name="firstName"
                                label="First Name"
                                variant="outlined"
                                value={firstName}
                                onChange={handleChange}
                                required
                                error={!!validationError?.firstName}
                                helperText={validationError?.firstName || ' '}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
                            <TextField
                                id="lastName"
                                name="lastName"
                                label="Last Name"
                                variant="outlined"
                                value={lastName}
                                onChange={handleChange}
                                required
                                error={!!validationError?.lastName}
                                helperText={validationError?.lastName || ' '}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
                            <TextField
                                id="uprn"
                                name="uprn"
                                label="UPRN"
                                variant="outlined"
                                value={uprn}
                                onChange={handleChange}
                                required
                                error={!!validationError?.uprn}
                                helperText={validationError?.uprn || ' '}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
                            <TextField
                                id="flyingHours"
                                name="flyingHours"
                                label="Flying Hours"
                                type="number"
                                variant="outlined"
                                value={flyingHours}
                                onChange={handleChange}
                                required
                                slotProps={{ input: { min: "0", max: "40000" } }}
                                error={!!validationError?.flyingHours}
                                helperText={validationError?.flyingHours || ' '}
                                sx={{ width: '80%' }}
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
                <BackToListAction dataType={ENTITIES.PILOTS} />
            </Box>
        </Box>
    );
}