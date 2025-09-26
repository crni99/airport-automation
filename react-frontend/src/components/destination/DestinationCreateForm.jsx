import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/create.js';
import { DataContext } from '../../store/DataContext.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import Grid from '@mui/material/Grid';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import PageTitle from '../common/PageTitle.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import CustomAlert from '../common/Alert.jsx';

export default function DestinationCreateForm() {
    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        city: '',
        airport: '',
        error: null,
        isPending: false,
    });

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(ENTITIES.DESTINATIONS, formData, ['city', 'airport']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const destination = { City: formData.city, Airport: formData.airport };
        setFormData({ ...formData, isPending: true, error: null });

        try {
            const create = await createData(destination, ENTITIES.DESTINATIONS, dataCtx.apiUrl, navigate);

            if (create) {
                console.error('Error creating destination:', create.message);
                setFormData({ ...formData, error: create.message, isPending: false });
            } else {
                setFormData({ city: '', airport: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to create destination. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(ENTITIES.DESTINATIONS, { ...prev, [name]: value }, ['city', 'airport']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Destination' />
            <Box
                component="form"
                autoComplete="off"
                onSubmit={handleSubmit}
            >
                <Grid container spacing={3}>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                        <TextField
                            id="city"
                            name="city"
                            label="City"
                            variant="outlined"
                            value={formData.city}
                            onChange={handleChange}
                            placeholder="Belgrade"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                        <TextField
                            id="airport"
                            name="airport"
                            label="Airport"
                            variant="outlined"
                            value={formData.airport}
                            onChange={handleChange}
                            placeholder="Belgrade Nikola Tesla Airport"
                            required
                            error={!!formData.error}
                            helperText={formData.error}
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
                        <CustomAlert alertType='error' type='Error' message={formData.error} />
                    )}
                </Grid>
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.DESTINATIONS} />
            </Box>
        </Box >
    );
}