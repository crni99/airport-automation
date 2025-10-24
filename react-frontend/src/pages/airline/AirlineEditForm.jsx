import React, { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useUpdate } from '../../hooks/useUpdate.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import UpdateOperationSnackbarManager from '../../components/common/feedback/UpdateOperationSnackbarManager.jsx';

const initialFormData = { name: '' };
const requiredFields = ['name'];

const transformAirlineForAPI = (formData, currentId) => ({
    Id: currentId,
    Name: formData.name,
});

const transformAirlineForForm = (fetchedData) => ({
    name: fetchedData.name || '',
});

export default function AirlineEditForm() {
    const { id } = useParams();

    const {
        name,
        success,
        formError,
        isPending,
        isFetching,
        isFetchError,
        fetchError,
        validationError,
        handleChange,
        handleSubmit,
        setFormData,
    } = useUpdate(
        ENTITIES.AIRLINES,
        ENTITY_PATHS.AIRLINES,
        id,
        initialFormData,
        requiredFields,
        transformAirlineForAPI,
        transformAirlineForForm
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title="Edit Airline" />

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
                                id="name"
                                name="name"
                                label="Name"
                                variant="outlined"
                                value={name}
                                onChange={handleChange}
                                required
                                error={!!validationError?.firstName}
                                helperText={validationError?.firstName || ' '}
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
                <BackToListAction dataType={ENTITIES.AIRLINES} />
            </Box>
        </Box>
    );
}