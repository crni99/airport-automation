import React, { useState, useContext, useEffect, useRef } from 'react';
import useFetch from '../../hooks/useFetch.jsx';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/create.js';
import PageTitle from '../common/PageTitle.jsx';
import Alert from '../common/Alert.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import { DataContext } from '../../store/DataContext.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import CustomAlert from '../common/Alert.jsx';

export default function PlaneTicketCreateForm() {

    const [pageNumber, setPageNumber] = useState(1);
    const isInitialLoad = useRef(true);

    const [allPassengers, setAllPassengers] = useState([]);
    const [allTravelClasses, setAllTravelClasses] = useState([]);
    const [allFlights, setAllFlights] = useState([]);

    const { data: passengers, error: errorPassengers, isLoading: isLoadingPassengers } = useFetch(ENTITIES.PASSENGERS, null, pageNumber);
    const { data: travelClasses, error: errorTravelClasses, isLoading: isLoadingTravelClasses } = useFetch(ENTITIES.TRAVEL_CLASSES, null, pageNumber);
    const { data: flights, error: errorFlights, isLoading: isLoadingFlights } = useFetch(ENTITIES.FLIGHTS, null, pageNumber);

    const dataCtx = useContext(DataContext);
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
            Price: formData.price,
            PurchaseDate: formData.purchaseDate,
            SeatNumber: formData.seatNumber,
            PassengerId: formData.passengerId,
            TravelClassId: formData.travelClassId,
            FlightId: formData.flightId,
        };

        setFormData({ ...formData, isPending: true, error: null });

        try {
            const create = await createData(planeTicket, ENTITIES.PLANE_TICKETS, dataCtx.apiUrl, navigate);

            if (create) {
                console.error('Error creating plane ticket:', create.message);
                setFormData({ ...formData, error: create.message, isPending: false });
            } else {
                setFormData({ ...formData, error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to create plane ticket. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => ({
            ...prev,
            [name]: value,
        }));
    };

    const handleLoadMore = () => {
        setPageNumber((prevPageNumber) => prevPageNumber + 1);
    };

    useEffect(() => {
        if (isInitialLoad.current) {
            isInitialLoad.current = false;
            return;
        }
        if (passengers?.data && passengers.data.length > 0) {
            setAllPassengers((prev) => {
                const newPassengers = passengers.data.filter(
                    (passenger) => !prev.some((prevPassenger) => prevPassenger.id === passenger.id)
                );
                return [...prev, ...newPassengers];
            });
        }
        if (travelClasses?.data && travelClasses.data.length > 0) {
            setAllTravelClasses((prev) => {
                const newTravelClasses = travelClasses.data.filter(
                    (travelClass) => !prev.some((prevTravelClass) => prevTravelClass.id === travelClass.id)
                );
                return [...prev, ...newTravelClasses];
            });
        }
        if (flights?.data && flights.data.length > 0) {
            setAllFlights((prev) => {
                const newFlights = flights.data.filter(
                    (flight) => !prev.some((prevFlight) => prevFlight.id === flight.id)
                );
                return [...prev, ...newFlights];
            });
        }
    }, [pageNumber, passengers?.data, travelClasses?.data, flights?.data]);

    if (isLoadingPassengers || isLoadingTravelClasses || isLoadingFlights) {
        return <CircularProgress />
    }

    if (errorPassengers || errorTravelClasses || errorFlights) {
        return <Alert alertType='danger' alertText='Error loading data..' />;
    }

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Plane Ticket' />
            <Box
                component="form"
                autoComplete="off"
                onSubmit={handleSubmit}
            >
                <Grid container spacing={3}>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <FormControl fullWidth >
                            <InputLabel id="passenger-select-label">Passenger</InputLabel>
                            <Select
                                labelId="passenger-select-label"
                                id="passengerId"
                                name="passengerId"
                                value={formData.passengerId}
                                label="Passenger"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">
                                    <em>Select Passenger</em>
                                </MenuItem>
                                {allPassengers.map((passenger) => (
                                    <MenuItem key={`passenger-${passenger.id}`} value={passenger.id}>
                                        {passenger.firstName} {passenger.lastName}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <FormControl fullWidth >
                            <InputLabel id="flight-select-label">Flight</InputLabel>
                            <Select
                                labelId="flight-select-label"
                                id="flightId"
                                name="flightId"
                                value={formData.flightId}
                                label="Flight"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">
                                    <em>Select Flight</em>
                                </MenuItem>
                                {allFlights.map((flight) => (
                                    <MenuItem key={`flight-${flight.id}`} value={flight.id}>
                                        {flight.departureDate} {flight.departureTime}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <FormControl fullWidth >
                            <InputLabel id="travel-class-select-label">Travel Class</InputLabel>
                            <Select
                                labelId="travel-class-select-label"
                                id="travelClassId"
                                name="travelClassId"
                                value={formData.travelClassId}
                                label="Travel Class"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">
                                    <em>Select Travel Class</em>
                                </MenuItem>
                                {allTravelClasses.map((travelClass) => (
                                    <MenuItem key={`travel-class-${travelClass.id}`} value={travelClass.id}>
                                        {travelClass.type}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="price"
                            name="price"
                            label="Price"
                            type="number"
                            variant="outlined"
                            value={formData.price}
                            onChange={handleChange}
                            placeholder="600"
                            required
                            fullWidth
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="seatNumber"
                            name="seatNumber"
                            label="Seat Number"
                            type="number"
                            variant="outlined"
                            value={formData.seatNumber}
                            onChange={handleChange}
                            placeholder="120"
                            required
                            fullWidth
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
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
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }} sx={{ mt: 3 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="success"
                            disabled={formData.isPending}
                            sx={{ mr: 3 }}
                        >
                            {formData.isPending ? <CircularProgress /> : 'Create'}
                        </Button>
                        <Button
                            variant="outlined"
                            onClick={handleLoadMore}
                            disabled={isLoadingPassengers || isLoadingTravelClasses || isLoadingFlights}
                        >
                            Load More
                        </Button>
                    </Grid>
                </Grid>
                {formData.error && (
                    <CustomAlert alertType='error' type='Error' message={formData.error} sx={{mt: 3}} />
                )}
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PLANE_TICKETS} />
            </Box>
        </Box>
    );
}