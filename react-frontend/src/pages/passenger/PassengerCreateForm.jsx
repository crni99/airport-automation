import React, { useCallback } from 'react';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import CreateOperationSnackbarManager from '../../components/common/feedback/CreateOperationSnackbarManager.jsx';
import { useCreate } from '../../hooks/useCreate.jsx';

const initialFormData = {
    firstName: '',
    lastName: '',
    uprn: '',
    passport: '',
    address: '',
    phone: ''
};

const requiredFields = ['firstName', 'lastName', 'uprn', 'passport', 'address', 'phone'];

const transformPassengerForAPI = (formData) => ({
    FirstName: formData.firstName,
    LastName: formData.lastName,
    UPRN: formData.uprn,
    Passport: formData.passport,
    Address: formData.address,
    Phone: formData.phone
});

export default function PassengerCreateForm() {
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
        handleChange,
        handleSubmit,
        setFormData,
    } = useCreate(
        ENTITIES.PASSENGERS,
        ENTITY_PATHS.PASSENGERS,
        initialFormData,
        requiredFields,
        transformPassengerForAPI
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Passenger' />

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
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="firstName"
                            name="firstName"
                            label="First Name"
                            variant="outlined"
                            value={firstName}
                            onChange={handleChange}
                            placeholder="Ognjen"
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
                            placeholder="Andjelic"
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
                            placeholder="0123456789112"
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
                            placeholder="012345678"
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
                            placeholder="014 Main Street, Belgrade, Serbia"
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
                            placeholder="012-456-7890"
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
                            disabled={isPending}
                        >
                            {isPending ? <CircularProgress size={24} /> : 'Create'}
                        </Button>
                    </Grid>
                </Grid>
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PASSENGERS} />
            </Box>
        </Box>
    );
}