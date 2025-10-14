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
import Grid from '@mui/material/Grid';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import CustomAlert from "../../components/common/Alert.jsx";

export default function DestinationEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        city: '',
        airport: '',
        error: null,
        isPending: false,
    });

    const { data: destinationData, isLoading, isError, error } = useFetch(ENTITIES.DESTINATIONS, id);

    useEffect(() => {
        if (destinationData) {
            setFormData((prevState) => ({
                ...prevState,
                city: destinationData.city || '',
                airport: destinationData.airport || '',
            }));
        }
    }, [destinationData]);

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

        const destination = {
            Id: id,
            City: formData.city,
            Airport: formData.airport
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(destination, ENTITIES.DESTINATIONS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating destination:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update destination. Please try again.', isPending: false });
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
        <Box sx={{ mt: 5}}>
            <PageTitle title='Edit Destination' />

            {isLoading && (
                <CircularProgress sx={{ mb: 2 }} />
            )}

            {isError && error && (
                <CustomAlert alertType='error' type={error.type} message={error.message} />
            )}

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
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '100%' }}
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
                            required
                            error={!!formData.error}
                            helperText={formData.error}
                            sx={{ width: '150%' }}
                        />
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
                    <CustomAlert alertType='error' type='Error' message={formData.error} sx={{mt: 3}} />
                )}
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.DESTINATIONS} />
            </Box>
        </Box>
    );
}