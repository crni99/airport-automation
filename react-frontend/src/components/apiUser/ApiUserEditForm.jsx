import React, { useState, useEffect, useContext } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { DataContext } from '../../store/DataContext.jsx';
import { editData } from '../../utils/edit.js';
import useFetch from '../../hooks/useFetch.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import PageTitle from '../common/PageTitle.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import { MenuItem } from '@mui/material';
import Grid from '@mui/material/Grid';
import CustomAlert from '../common/Alert.jsx';

export default function ApiUserEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        userName: '',
        password: '',
        roles: '',
        error: null,
        isPending: false,
    });

    const { data: apiUserData, isLoading, isError, error } = useFetch(ENTITIES.API_USERS, id);

    useEffect(() => {
        if (apiUserData) {
            setFormData((prevState) => ({
                ...prevState,
                userName: apiUserData.userName || '',
                password: apiUserData.password || '',
                roles: apiUserData.roles || '',
            }));
        }
    }, [apiUserData]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(ENTITIES.API_USERS, formData, ['userName', 'password', 'roles']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const apiUser = {
            Id: id,
            UserName: formData.userName,
            Password: formData.password,
            Roles: formData.roles,
        };

        setFormData((prevState) => ({ ...prevState, isPending: true, error: null }));

        try {
            const result = await editData(apiUser, ENTITIES.API_USERS, id, dataCtx.apiUrl, navigate);

            if (result && result.message) {
                setFormData({ ...formData, error: result.message, isPending: false });
            } else {
                setFormData({ ...formData, error: null, isPending: false });
            }
        } catch (err) {
            setFormData({ ...formData, error: 'Failed to update api user. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(ENTITIES.API_USERS, { ...prev, [name]: value }, [
                'userName',
                'password',
                'roles',
            ]);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <Box sx={{ mt: 2 }}>
            <PageTitle title="Edit Api User" />

            {isLoading && (
                <CircularProgress sx={{ mb: 2 }} />
            )}

            {isError && error && (
                <CustomAlert alertType='error' type={error.type} message={error.message} />
            )}

            {!isLoading && !isError && (
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
                                value={formData.userName}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="password"
                                name="password"
                                label="Password"
                                variant="outlined"
                                value={formData.password}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="roles"
                                name="roles"
                                label="Roles"
                                select
                                variant="outlined"
                                value={formData.roles}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
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
                                disabled={formData.isPending}
                            >
                                {formData.isPending ? <CircularProgress /> : 'Save Changes'}
                            </Button>
                        </Grid>
                    </Grid>
                    {formData.error && (
                        <CustomAlert alertType='error' type='Error' message={formData.error} />
                    )}
                </Box>
            )}
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.AIRLINES} />
            </Box>
        </Box>
    );
}