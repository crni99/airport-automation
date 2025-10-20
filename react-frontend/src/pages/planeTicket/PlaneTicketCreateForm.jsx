import React, { useState, useEffect, useRef, useCallback } from 'react';
import useFetch from '../../hooks/useFetch.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import CustomAlert from '../../components/common/feedback/CustomAlert.jsx';
import CreateOperationSnackbarManager from '../../components/common/feedback/CreateOperationSnackbarManager.jsx';
import { useCreateOperation } from '../../hooks/useCreateOperation.jsx';
import {
    Box,
    CircularProgress,
    Grid,
    TextField,
    Button,
    FormControl,
    InputLabel,
    Select,
    MenuItem
} from '@mui/material';

const initialFormData = {
    price: '',
    purchaseDate: '',
    seatNumber: '',
    passengerId: '',
    travelClassId: '',
    flightId: '',
};

const requiredFields = ['price', 'purchaseDate', 'seatNumber', 'passengerId', 'travelClassId', 'flightId'];

const transformTicketForAPI = (formData) => ({
    Price: parseFloat(formData.price),
    PurchaseDate: formData.purchaseDate,
    SeatNumber: parseInt(formData.seatNumber, 10),
    PassengerId: parseInt(formData.passengerId, 10),
    TravelClassId: parseInt(formData.travelClassId, 10),
    FlightId: parseInt(formData.flightId, 10),
});

export default function PlaneTicketCreateForm() {
    const [pageNumber, setPageNumber] = useState(1);
    const isInitialLoad = useRef(true);
    const [allPassengers, setAllPassengers] = useState([]);
    const [allTravelClasses, setAllTravelClasses] = useState([]);
    const [allFlights, setAllFlights] = useState([]);

    const { data: passengers, error: errorPassengers, isLoading: isLoadingPassengers } = useFetch(ENTITIES.PASSENGERS, null, pageNumber);
    const { data: travelClasses, error: errorTravelClasses, isLoading: isLoadingTravelClasses } = useFetch(ENTITIES.TRAVEL_CLASSES, null, pageNumber);
    const { data: flights, error: errorFlights, isLoading: isLoadingFlights } = useFetch(ENTITIES.FLIGHTS, null, pageNumber);

    const {
        price,
        purchaseDate,
        seatNumber,
        passengerId,
        travelClassId,
        flightId,
        success,
        formError,
        validationError,
        isPending,
        handleChange,
        handleSubmit,
        setFormData,
    } = useCreateOperation(
        ENTITIES.PLANE_TICKETS,
        ENTITY_PATHS.PLANE_TICKETS,
        initialFormData,
        requiredFields,
        transformTicketForAPI
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null, validationError: null }));
    }, [setFormData]);

    useEffect(() => {
        if (isInitialLoad.current) {
            isInitialLoad.current = false;
            return;
        }

        const updateData = (newData, setter) => {
            if (newData?.data && newData.data.length > 0) {
                setter((prev) => {
                    const newItems = newData.data.filter(
                        (item) => !prev.some((prevItem) => prevItem.id === item.id)
                    );
                    return [...prev, ...newItems];
                });
            }
        };

        updateData(passengers, setAllPassengers);
        updateData(travelClasses, setAllTravelClasses);
        updateData(flights, setAllFlights);

    }, [pageNumber, passengers, travelClasses, flights, setAllPassengers, setAllTravelClasses, setAllFlights]);


    const handleLoadMore = () => {
        setPageNumber((prevPageNumber) => prevPageNumber + 1);
    };

    const isDataLoading = isLoadingPassengers || isLoadingTravelClasses || isLoadingFlights;
    const dataError = errorPassengers || errorTravelClasses || errorFlights;

    if (dataError) {
        return <Box sx={{ mt: 5 }}><CustomAlert alertType='danger' type='Error' message='Error loading dependency data.' /></Box>;
    }

    if (isDataLoading && allPassengers.length === 0 && allTravelClasses.length === 0 && allFlights.length === 0) {
        return <CircularProgress sx={{ mt: 5 }} />;
    }

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Plane Ticket' />

            <CreateOperationSnackbarManager
                success={success}
                formError={formError}
                handleCloseSnackbar={handleCloseSnackbar}
            />

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
                                value={passengerId}
                                label="Passenger"
                                onChange={handleChange}
                                error={!!validationError?.passengerId}
                                helperText={validationError?.passengerId || ' '}
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
                                value={flightId}
                                label="Flight"
                                onChange={handleChange}
                                error={!!validationError?.flightId}
                                helperText={validationError?.flightId || ' '}
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
                                value={travelClassId}
                                label="Travel Class"
                                onChange={handleChange}
                                error={!!validationError?.travelClassId}
                                helperText={validationError?.travelClassId || ' '}
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
                            label="Price (€)"
                            type="number"
                            variant="outlined"
                            value={price}
                            onChange={handleChange}
                            placeholder="600"
                            fullWidth
                            error={!!validationError?.price}
                            helperText={validationError?.price || ' '}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="seatNumber"
                            name="seatNumber"
                            label="Seat Number"
                            variant="outlined"
                            value={seatNumber}
                            onChange={handleChange}
                            placeholder="16"
                            fullWidth
                            error={!!validationError?.seatNumber}
                            helperText={validationError?.seatNumber || ' '}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <TextField
                            id="purchaseDate"
                            name="purchaseDate"
                            label="Purchase Date"
                            type="date"
                            variant="outlined"
                            value={purchaseDate}
                            onChange={handleChange}
                            fullWidth
                            slotProps={{ inputLabel: { shrink: true } }}
                            error={!!validationError?.purchaseDate}
                            helperText={validationError?.purchaseDate || ' '}
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }} sx={{ mt: 3 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="success"
                            disabled={isPending || isDataLoading}
                            sx={{ mr: 3 }}
                        >
                            {isPending ? <CircularProgress size={24} color="inherit" /> : 'Create'}
                        </Button>
                        <Button
                            variant="outlined"
                            onClick={handleLoadMore}
                            disabled={isDataLoading}
                        >
                            {isDataLoading ? <CircularProgress size={24} color="inherit" /> : 'Load More'}
                        </Button>
                    </Grid>
                </Grid>
            </Box>
            <Box sx={{ mt: 3 }}>
                <BackToListAction dataType={ENTITIES.PLANE_TICKETS} />
            </Box>
        </Box>
    );
}