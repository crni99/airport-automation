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

export default function PlaneTicketEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        price: '',
        purchaseDate: '',
        seatNumber: '',
        passengerId: '',
        travelClassId: '',
        flightId: '',
        error: null,
        isPending: false,
    });

    const { data: planeTicket, isLoading, isError, error } = useFetch(ENTITIES.PLANE_TICKETS, id);

    useEffect(() => {
        if (planeTicket) {
            setFormData((prevState) => ({
                ...prevState,
                price: planeTicket.price || '',
                purchaseDate: planeTicket.purchaseDate || '',
                seatNumber: planeTicket.seatNumber || '',
                passengerId: planeTicket.passengerId || '',
                travelClassId: planeTicket.travelClassId || '',
                flightId: planeTicket.flightId || '',
            }));
        }
    }, [planeTicket]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(ENTITIES.PLANE_TICKETS, formData, ['price', 'purchaseDate', 'seatNumber', 'passengerId', 'travelClassId', 'flightId']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const planeTicket = {
            Id: id,
            Price: formData.price,
            PurchaseDate: formData.purchaseDate,
            SeatNumber: formData.seatNumber,
            PassengerId: formData.passengerId,
            TravelClassId: formData.travelClassId,
            FlightId: formData.flightId,
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(planeTicket, ENTITIES.PLANE_TICKETS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating plane ticket:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update plane ticket. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(ENTITIES.PLANE_TICKETS, { ...prev, [name]: value }, ['price', 'purchaseDate', 'seatNumber', 'passengerId', 'travelClassId', 'flightId']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <Box sx={{ p: 3 }}>
            <PageTitle title='Edit Plane Ticket' />
            {isLoading && (
                <Box sx={{ display: 'flex', justifyContent: 'center', mt: 4 }}>
                    <CircularProgress />
                </Box>
            )}
            {isError && error && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    <AlertTitle>Error</AlertTitle>
                    {error.message}
                </Alert>
            )}
            {formData.error && (
                <Alert severity="error" sx={{ mt: 2 }}>
                    <AlertTitle>Error</AlertTitle>
                    {formData.error}
                </Alert>
            )}
            {!isLoading && !isError && (
                <Box
                    component="form"
                    sx={{ mt: 2, '& .MuiTextField-root': { mb: 3, width: '100%' } }}
                    autoComplete="off"
                    onSubmit={handleSubmit}
                >
                    <Grid container spacing={4}>
                        <Grid item xs={12} md={4}>
                            <TextField
                                id="id"
                                name="id"
                                label="Id"
                                type="number"
                                variant="outlined"
                                value={id}
                                InputProps={{ readOnly: true }}
                                fullWidth
                                sx={{ mb: 3 }}
                            />
                            <TextField
                                id="price"
                                name="price"
                                label="Price"
                                type="number"
                                variant="outlined"
                                value={formData.price}
                                onChange={handleChange}
                                required
                                fullWidth
                                sx={{ mb: 3 }}
                            />
                            <TextField
                                id="purchaseDate"
                                name="purchaseDate"
                                label="Purchase Date"
                                type="date"
                                variant="outlined"
                                value={formData.purchaseDate}
                                onChange={handleChange}
                                required
                                fullWidth
                                InputLabelProps={{
                                    shrink: true,
                                }}
                                sx={{ mb: 3 }}
                            />
                            <TextField
                                id="seatNumber"
                                name="seatNumber"
                                label="Seat Number"
                                type="number"
                                variant="outlined"
                                value={formData.seatNumber}
                                onChange={handleChange}
                                required
                                fullWidth
                                sx={{ mb: 3 }}
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
                        <Grid item xs={12} md={4}>
                            <TextField
                                id="passengerId"
                                name="passengerId"
                                label="Passenger Id"
                                type="number"
                                variant="outlined"
                                value={formData.passengerId}
                                InputProps={{ readOnly: true }}
                                required
                                fullWidth
                                sx={{ mb: 3 }}
                            />
                            <TextField
                                id="travelClassId"
                                name="travelClassId"
                                label="Travel Class Id"
                                type="number"
                                variant="outlined"
                                value={formData.travelClassId}
                                InputProps={{ readOnly: true }}
                                required
                                fullWidth
                                sx={{ mb: 3 }}
                            />
                            <TextField
                                id="flightId"
                                name="flightId"
                                label="Flight Id"
                                type="number"
                                variant="outlined"
                                value={formData.flightId}
                                InputProps={{ readOnly: true }}
                                required
                                fullWidth
                                sx={{ mb: 3 }}
                                disabled
                            />
                        </Grid>
                    </Grid>
                </Box>
            )}
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PLANE_TICKETS} />
            </Box>
        </Box>
    );
}