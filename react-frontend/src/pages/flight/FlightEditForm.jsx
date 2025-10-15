import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useContext } from 'react';
import { DataContext } from '../../store/DataContext.jsx';
import { editData } from '../../utils/edit.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import useFetch from '../../hooks/useFetch.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import { formatTime } from '../../utils/formatting.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";

export default function FlightEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        departureDate: '',
        departureTime: '',
        airlineId: '',
        destinationId: '',
        pilotId: '',
        error: null,
        isPending: false,
    });

    const { data: flightData, isLoading, isError, error } = useFetch(ENTITIES.FLIGHTS, id);

    useEffect(() => {
        if (flightData) {
            setFormData((prevState) => ({
                ...prevState,
                departureDate: flightData.departureDate || '',
                departureTime: flightData.departureTime || '',
                airlineId: flightData.airlineId || '',
                destinationId: flightData.destinationId || '',
                pilotId: flightData.pilotId || '',
            }));
        }
    }, [flightData]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(ENTITIES.FLIGHTS, formData, ['departureDate', 'departureTime', 'airlineId', 'destinationId', 'pilotId']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const formattedDepartureTime = formatTime(formData.departureTime);

        const flight = {
            Id: id,
            DepartureDate: formData.departureDate,
            DepartureTime: formattedDepartureTime,
            AirlineId: formData.airlineId,
            DestinationId: formData.destinationId,
            PilotId: formData.pilotId,
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(flight, ENTITIES.FLIGHTS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating flight:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update flight. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(ENTITIES.FLIGHTS, { ...prev, [name]: value }, ['departureDate', 'departureTime', 'airlineId', 'destinationId', 'pilotId']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Edit Flight' />

            {isLoading && (
                <CircularProgress sx={{ mb: 2 }} />
            )}

            {formData.error && (
                <CustomAlert alertType='error' type={error.type} message={error.message} />
            )}

            {!isLoading && !isError && (
                <Box
                    component="form"
                    autoComplete="off"
                    onSubmit={handleSubmit}
                >
                    <Grid container spacing={4}>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="id"
                                name="id"
                                label="Id"
                                type="number"
                                variant="outlined"
                                value={id}
                                InputProps={{ readOnly: true }}
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="airlineId"
                                name="airlineId"
                                label="Airline Id"
                                type="number"
                                variant="outlined"
                                value={formData.airlineId}
                                InputProps={{ readOnly: true }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="destinationId"
                                name="destinationId"
                                label="Destination Id"
                                type="number"
                                variant="outlined"
                                value={formData.destinationId}
                                InputProps={{ readOnly: true }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="pilotId"
                                name="pilotId"
                                label="Pilot Id"
                                type="number"
                                variant="outlined"
                                value={formData.pilotId}
                                InputProps={{ readOnly: true }}
                                required
                                fullWidth
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 3, xl: 3 }}>
                            <TextField
                                id="departureDate"
                                name="departureDate"
                                label="Departure Date"
                                type="date"
                                variant="outlined"
                                value={formData.departureDate}
                                onChange={handleChange}
                                required
                                fullWidth
                                InputLabelProps={{
                                    shrink: true,
                                }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                            <TextField
                                id="departureTime"
                                name="departureTime"
                                label="Departure Time"
                                type="time"
                                variant="outlined"
                                value={formData.departureTime}
                                onChange={handleChange}
                                required
                                fullWidth
                                InputLabelProps={{
                                    shrink: true,
                                }}
                            />
                        </Grid>
                        <Grid size={{ xs: 12 }} sx={{ mt: 1 }}>
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
                </Box>
            )}
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.FLIGHTS} />
            </Box>
        </Box>
    );
}