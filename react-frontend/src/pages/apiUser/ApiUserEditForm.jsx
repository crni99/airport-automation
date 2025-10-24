import React, { useCallback } from 'react';
import { useParams } from 'react-router-dom';
import { useUpdate } from '../../hooks/useUpdate.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import { MenuItem } from '@mui/material';
import Grid from '@mui/material/Grid';
import UpdateOperationSnackbarManager from '../../components/common/feedback/UpdateOperationSnackbarManager.jsx';

const initialFormData = { userName: '', password: '', roles: '' };
const requiredFields = ['userName', 'roles'];

const transformApiUserForAPI = (formData, currentId) => ({
    Id: currentId,
    UserName: formData.userName,
    Password: formData.password,
    Roles: formData.roles,
});

const transformApiUserForForm = (fetchedData) => ({
    userName: fetchedData.userName || '',
    password: '',
    roles: fetchedData.roles || '',
});

export default function ApiUserEditForm() {
    const { id } = useParams();

    const {
        userName,
        roles,
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
        ENTITIES.API_USERS,
        ENTITY_PATHS.API_USERS,
        id,
        initialFormData,
        requiredFields,
        transformApiUserForAPI,
        transformApiUserForForm
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title="Edit Api User" />

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
                                id="userName"
                                name="userName"
                                label="Username"
                                variant="outlined"
                                value={userName}
                                onChange={handleChange}
                                required
                                error={!!validationError?.userName}
                                helperText={validationError?.userName || ' '}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="password"
                                name="password"
                                label="Password"
                                variant="outlined"
                                value=""
                                onChange={handleChange}
                                required
                                error={!!validationError?.password}
                                helperText={validationError?.password || ' '}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="roles"
                                name="roles"
                                label="Roles"
                                select
                                variant="outlined"
                                value={roles}
                                onChange={handleChange}
                                required
                                error={!!validationError?.roles}
                                helperText={validationError?.roles || ' '}
                                sx={{ width: '80%' }}
                            >
                                <MenuItem value="SuperAdmin">SuperAdmin</MenuItem>
                                <MenuItem value="Admin">Admin</MenuItem>
                                <MenuItem value="User">User</MenuItem>
                            </TextField>
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
                <BackToListAction dataType={ENTITIES.API_USERS} />
            </Box>
        </Box>
    );
}