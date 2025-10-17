import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/httpCreate.js';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import { DataContext } from '../../store/DataContext.jsx';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";

export default function AirlineCreateForm() {
    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        name: '',
        error: null,
        isPending: false,
    });

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(ENTITIES.AIRLINES, formData, ['name']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const airline = { Name: formData.name };
        setFormData({ ...formData, isPending: true, error: null });

        try {
            const result = await createData(airline, ENTITIES.AIRLINES, dataCtx.apiUrl, navigate);

            if (result && result.message) {
                setFormData({ ...formData, error: result.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            setFormData({ ...formData, error: 'Failed to create airline. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields('Airline', { ...prev, [name]: value }, ['name']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Airline' />
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
                            value={formData.name}
                            onChange={handleChange}
                            placeholder="Air Serbia"
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
                </Grid>
                {formData.error && (
                    <CustomAlert alertType='error' type='Error' message={formData.error} sx={{mt: 3}} />
                )}
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.AIRLINES} />
            </Box>
        </Box>
    );
}