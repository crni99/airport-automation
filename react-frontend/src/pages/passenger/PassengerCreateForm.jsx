import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/create.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";

export default function PassengerCreateForm() {
    const dataCtx = useContext(DataContext);
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
            FirstName: formData.firstName,
            LastName: formData.lastName,
            UPRN: formData.uprn,
            Passport: formData.passport,
            Address: formData.address,
            Phone: formData.phone
        };
        setFormData({ ...formData, isPending: true, error: null });

        try {
            const create = await createData(passenger, ENTITIES.PASSENGERS, dataCtx.apiUrl, navigate);

            if (create) {
                console.error('Error creating passenger:', create.message);
                setFormData({ ...formData, error: create.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to create passenger. Please try again.', isPending: false });
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
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Passenger' />
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
                            value={formData.firstName}
                            onChange={handleChange}
                            placeholder="Ognjen"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '80%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="lastName"
                            name="lastName"
                            label="Last Name"
                            variant="outlined"
                            value={formData.lastName}
                            onChange={handleChange}
                            placeholder="Andjelic"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '80%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="uprn"
                            name="uprn"
                            label="UPRN"
                            variant="outlined"
                            value={formData.uprn}
                            onChange={handleChange}
                            placeholder="0123456789112"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '80%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="passport"
                            name="passport"
                            label="Passport"
                            variant="outlined"
                            value={formData.passport}
                            onChange={handleChange}
                            placeholder="012345678"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '80%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="address"
                            name="address"
                            label="Address"
                            variant="outlined"
                            value={formData.address}
                            onChange={handleChange}
                            placeholder="014 Main Street, Belgrade, Serbia"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '80%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="phone"
                            name="phone"
                            label="Phone"
                            variant="outlined"
                            value={formData.phone}
                            onChange={handleChange}
                            placeholder="012-456-7890"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '80%' }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }} sx={{ mt: 2 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="success"
                            disabled={formData.isPending}
                        >
                            {formData.isPending ? <CircularProgress /> : 'Create'}
                        </Button>
                    </Grid>
                    {formData.error && (
                        <CustomAlert alertType='error' type='Error' message={formData.error} sx={{mt: 3}} />
                    )}
                </Grid>
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PASSENGERS} />
            </Box>
        </Box>
    );
}