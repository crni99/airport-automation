import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import useFetch from '../../hooks/useFetch.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import CustomAlert from '../common/Alert.jsx';

export default function PilotEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        uprn: '',
        flyingHours: '',
        error: null,
        isPending: false,
    });

    const { data: pilotData, isLoading, isError, error } = useFetch(ENTITIES.PILOTS, id);

    useEffect(() => {
        if (pilotData) {
            setFormData((prevState) => ({
                ...prevState,
                firstName: pilotData.firstName || '',
                lastName: pilotData.lastName || '',
                uprn: pilotData.uprn || '',
                flyingHours: pilotData.flyingHours || '',
            }));
        }
    }, [pilotData]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(ENTITIES.PILOTS, formData, ['firstName', 'lastName', 'uprn', 'flyingHours']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const pilot = {
            FirstName: formData.firstName,
            LastName: formData.lastName,
            UPRN: formData.uprn,
            FlyingHours: formData.flyingHours,
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(pilot, ENTITIES.PILOTS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating pilot:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update pilot. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(ENTITIES.PILOTS, { ...prev, [name]: value }, ['firstName', 'lastName', 'uprn', 'flyingHours']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Edit Pilot' />

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
                    <Grid container spacing={2}>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
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
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
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
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
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
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 6 }}>
                            <TextField
                                id="flyingHours"
                                name="flyingHours"
                                label="Flying Hours"
                                type="number"
                                variant="outlined"
                                value={formData.flyingHours}
                                onChange={handleChange}
                                required
                                inputProps={{ min: "0", max: "40000" }}
                                error={!!formData.error}
                                helperText={formData.error}
                                sx={{ width: '80%' }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12 }} sx={{ mt: 3 }}>
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
                <BackToListAction dataType={ENTITIES.PILOTS} />
            </Box>
        </Box>
    );
}