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
import { useCreateOperation } from '../../hooks/useCreateOperation.jsx';

const initialFormData = { name: '' };
const requiredFields = ['name'];

const transformAirlineForAPI = (data) => ({
    Name: data.name
});

export default function AirlineCreateForm() {
    const {
        name,
        success,
        formError,
        validationError,
        isPending,
        handleChange,
        handleSubmit,
        setFormData,
    } = useCreateOperation(
        ENTITIES.AIRLINES,
        ENTITY_PATHS.AIRLINES,
        initialFormData,
        requiredFields,
        transformAirlineForAPI
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Airline' />

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
                            id="name"
                            name="name"
                            label="Name"
                            variant="outlined"
                            value={name}
                            onChange={handleChange}
                            placeholder="Air Serbia"
                            error={!!validationError?.name}
                            helperText={validationError?.name || ' '}
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }} sx={{ mt: 2 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="success"
                            disabled={isPending}
                        >
                            {isPending ? <CircularProgress /> : 'Create'}
                        </Button>
                    </Grid>
                </Grid>
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.AIRLINES} />
            </Box>
        </Box>
    );
}