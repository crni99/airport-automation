import React, { useCallback } from 'react';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import CreateOperationSnackbarManager from '../../components/common/feedback/CreateOperationSnackbarManager.jsx';
import { useCreateOperation } from '../../hooks/useCreateOperation.jsx';
import {
    Box,
    CircularProgress,
    Grid,
    TextField,
    Button
} from '@mui/material';

const initialFormData = {
    firstName: '',
    lastName: '',
    uprn: '',
    flyingHours: ''
};

const requiredFields = ['firstName', 'lastName', 'uprn', 'flyingHours'];

const transformPilotForAPI = (formData) => ({
    FirstName: formData.firstName,
    LastName: formData.lastName,
    UPRN: formData.uprn,
    // Ensure flyingHours is treated as a number in the API payload
    FlyingHours: Number(formData.flyingHours),
});

export default function PilotCreateForm() {
    const {
        firstName,
        lastName,
        uprn,
        flyingHours,
        success,
        formError,
        isPending,
        handleChange,
        handleSubmit,
        setFormData,
    } = useCreateOperation(
        ENTITIES.PILOTS,
        ENTITY_PATHS.PILOTS,
        initialFormData,
        requiredFields,
        transformPilotForAPI
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Pilot' />

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
                <Grid container spacing={2}>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
                        <TextField
                            id="firstName"
                            name="firstName"
                            label="First Name"
                            variant="outlined"
                            value={firstName}
                            onChange={handleChange}
                            placeholder="Ognjen"
                            required
                            error={!!formError}
                            helperText={formError}
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
                            placeholder="Andjelic"
                            required
                            error={!!formError}
                            helperText={formError}
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
                            placeholder="0123456789112"
                            required
                            error={!!formError}
                            helperText={formError}
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
                            placeholder="60"
                            required
                            inputProps={{ min: "0", max: "40000" }}
                            error={!!formError}
                            helperText={formError}
                            sx={{ width: '80%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }} sx={{ mt: 3 }}>
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
                <BackToListAction dataType={ENTITIES.PILOTS} />
            </Box>
        </Box>
    );
}