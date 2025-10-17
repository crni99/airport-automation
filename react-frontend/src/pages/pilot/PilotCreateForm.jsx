import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/httpCreate.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";
import {
    Box,
    CircularProgress,
    Grid,
    TextField,
    Button
} from '@mui/material';

export default function PilotCreateForm() {
    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        uprn: '',
        flyingHours: '',
        error: null,
        isPending: false,
    });

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
        setFormData({ ...formData, isPending: true, error: null });

        try {
            const create = await createData(pilot, ENTITIES.PILOTS, dataCtx.apiUrl, navigate);

            if (create) {
                console.error('Error creating pilot:', create.message);
                setFormData({ ...formData, error: create.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to create pilot. Please try again.', isPending: false });
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
            <PageTitle title='Create Pilot' />
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
                            placeholder="Ognjen"
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
                            placeholder="Andjelic"
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
                            placeholder="0123456789112"
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
                            placeholder="60"
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
                            {formData.isPending ? <CircularProgress /> : 'Create'}
                        </Button>
                    </Grid>
                </Grid>
                {formData.error && (
                    <CustomAlert alertType='error' type='Error' message={formData.error} sx={{mt: 3}} />
                )}
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PILOTS} />
            </Box>
        </Box>
    );
}