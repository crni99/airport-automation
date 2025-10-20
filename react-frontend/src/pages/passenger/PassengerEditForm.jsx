import React, { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useUpdateOperation } from '../../hooks/useUpdateOperation.jsx';
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
    passport: '',
    address: '',
    phone: ''
};

const requiredFields = [
    'firstName',
    'lastName',
    'uprn',
    'passport',
    'address',
    'phone'
];

const transformPassengerForAPI = (formData, currentId) => ({
    Id: currentId,
    FirstName: formData.firstName,
    LastName: formData.lastName,
    UPRN: formData.uprn,
    Passport: formData.passport,
    Address: formData.address,
    Phone: formData.phone
});

const transformPassengerForForm = (fetchedData) => ({
    firstName: fetchedData.firstName || '',
    lastName: fetchedData.lastName || '',
    uprn: fetchedData.uprn || '',
    passport: fetchedData.passport || '',
    address: fetchedData.address || '',
    phone: fetchedData.phone || '',
});

export default function PassengerEditForm() {
    const { id } = useParams();

    const {
        firstName,
        lastName,
        uprn,
        passport,
        address,
        phone,
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
    } = useUpdateOperation(
        ENTITIES.PASSENGERS,
        ENTITY_PATHS.PASSENGERS,
        id,
        initialFormData,
        requiredFields,
        transformPassengerForAPI,
        transformPassengerForForm
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);


    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Edit Passenger' />

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
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
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
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
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
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
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
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                            <TextField
                                id="passport"
                                name="passport"
                                label="Passport"
                                variant="outlined"
                                value={passport}
                                onChange={handleChange}
                                required
                                error={!!validationError?.passport}
                                helperText={validationError?.passport || ' '}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                            <TextField
                                id="address"
                                name="address"
                                label="Address"
                                variant="outlined"
                                value={address}
                                onChange={handleChange}
                                required
                                error={!!validationError?.address}
                                helperText={validationError?.address || ' '}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                            <TextField
                                id="phone"
                                name="phone"
                                label="Phone"
                                variant="outlined"
                                value={phone}
                                onChange={handleChange}
                                required
                                error={!!validationError?.phone}
                                helperText={validationError?.phone || ' '}
                                sx={{ width: '80%' }}
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
                <BackToListAction dataType={ENTITIES.PASSENGERS} />
            </Box>
        </Box>
    );
}