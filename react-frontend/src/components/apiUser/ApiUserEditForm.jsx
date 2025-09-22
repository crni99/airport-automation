import React, { useState, useEffect, useContext } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { DataContext } from '../../store/DataContext.jsx';
import { editData } from '../../utils/edit.js';
import useFetch from '../../hooks/useFetch.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';

// Material-UI Components
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Alert from '@mui/material/Alert';
import AlertTitle from '@mui/material/AlertTitle';
import PageTitle from '../common/PageTitle.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import { Stack, MenuItem } from '@mui/material';

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
        <Box sx={{ p: 3 }}>
            <PageTitle title="Edit Api User" />

            {isLoading && (
                <Stack direction="row" justifyContent="center" sx={{ mt: 4 }}>
                    <CircularProgress />
                </Stack>
            )}

            {isError && error && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    <AlertTitle>{error.type}</AlertTitle>
                    {error.message}
                </Alert>
            )}

            {!isLoading && !isError && (
                <>

                    <Box
                        component="form"
                        onSubmit={handleSubmit}
                        sx={{ mt: 2, '& .MuiTextField-root': { width: '50%' } }}
                    >
                        <Stack spacing={3}>
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

                        </Stack>
                        {formData.error && (
                            <Alert severity="error" sx={{ mt: 2 }}>
                                {formData.error}
                            </Alert>
                        )}
                    </Box>
                    <Button
                        type="submit"
                        variant="contained"
                        color="success"
                        disabled={formData.isPending}
                        sx={{ mt: 3 }}
                    >
                        {formData.isPending ? <CircularProgress size={24} /> : 'Save Changes'}
                    </Button>
                </>
            )}

            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.API_USERS} />
            </Box>
        </Box>
    );
}