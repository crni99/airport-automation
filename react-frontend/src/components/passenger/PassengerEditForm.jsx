import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import Alert from '../common/Alert.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import useFetch from '../../hooks/useFetch.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import AlertTitle from '@mui/material/AlertTitle';
import Grid from '@mui/material/Grid';
import { Stack } from '@mui/material';

export default function PassengerEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        uprn: '',
        passport: '',
        address: '',
        phone: '',
        error: null,
        isPending: false,
    });

    const { data: passengerData, isLoading, isError, error } = useFetch(ENTITIES.PASSENGERS, id);

    useEffect(() => {
        if (passengerData) {
            setFormData((prevState) => ({
                ...prevState,
                firstName: passengerData.firstName || '',
                lastName: passengerData.lastName || '',
                uprn: passengerData.uprn || '',
                passport: passengerData.passport || '',
                address: passengerData.address || '',
                phone: passengerData.phone || '',
            }));
        }
    }, [passengerData]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(ENTITIES.PASSENGERS, formData, ['firstName', 'lastName', 'uprn', 'passport', 'address', 'phone']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const passenger = {
            Id: id,
            FirstName: formData.firstName,
            LastName: formData.lastName,
            UPRN: formData.uprn,
            Passport: formData.passport,
            Address: formData.address,
            Phone: formData.phone
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(passenger, ENTITIES.PASSENGERS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating passenger:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update passenger. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(ENTITIES.PASSENGERS, { ...prev, [name]: value }, ['firstName', 'lastName', 'uprn', 'passport', 'address', 'phone']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <Box sx={{ p: 3 }}>
            <PageTitle title='Edit Passenger' />

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
                <Box
                    component="form"
                    sx={{ mt: 2, '& .MuiTextField-root': { mb: 3, width: '100%' } }}
                    autoComplete="off"
                    onSubmit={handleSubmit}
                >
                    <Grid container spacing={2}>
                        <Grid item xs={12}>
                            <TextField
                                id="firstName"
                                name="firstName"
                                label="First Name"
                                variant="outlined"
                                value={formData.firstName}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                            <TextField
                                id="lastName"
                                name="lastName"
                                label="Last Name"
                                variant="outlined"
                                value={formData.lastName}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                            <TextField
                                id="uprn"
                                name="uprn"
                                label="UPRN"
                                variant="outlined"
                                value={formData.uprn}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                            <TextField
                                id="passport"
                                name="passport"
                                label="Passport"
                                variant="outlined"
                                value={formData.passport}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                            <TextField
                                id="address"
                                name="address"
                                label="Address"
                                variant="outlined"
                                value={formData.address}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                            <TextField
                                id="phone"
                                name="phone"
                                label="Phone"
                                variant="outlined"
                                value={formData.phone}
                                onChange={handleChange}
                                required
                                error={!!formData.error}
                                helperText={formData.error}
                            />
                            <Button
                                type="submit"
                                variant="contained"
                                color="success"
                                disabled={formData.isPending}
                            >
                                {formData.isPending ? <CircularProgress size={24} color="inherit" /> : 'Save Changes'}
                            </Button>
                        </Grid>
                    </Grid>
                    {formData.error && (
                        <Alert severity="error" sx={{ mt: 2 }}>
                            <AlertTitle>Error</AlertTitle>
                            {formData.error}
                        </Alert>
                    )}
                </Box>
            )}
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PASSENGERS} />
            </Box>
        </Box>
    );
}