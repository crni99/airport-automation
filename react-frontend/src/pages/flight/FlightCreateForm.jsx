import React, { useState, useEffect, useRef, useCallback } from 'react';
import useFetch from '../../hooks/useFetch.jsx';
import { ENTITIES, ENTITY_PATHS } from '../../utils/const.js';
import { formatTime } from '../../utils/formatting.js';
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
    departureDate: '',
    departureTime: '',
    airlineId: '',
    destinationId: '',
    pilotId: '',
};

const requiredFields = ['departureDate', 'departureTime', 'airlineId', 'destinationId', 'pilotId'];

const transformFlightForAPI = (formData) => ({
    DepartureDate: formData.departureDate,
    DepartureTime: formatTime(formData.departureTime),
    AirlineId: Number(formData.airlineId),
    DestinationId: Number(formData.destinationId),
    PilotId: Number(formData.pilotId),
});

export default function FlightCreateForm() {
    const [pageNumber, setPageNumber] = useState(1);
    const isInitialLoad = useRef(true);
    const [allAirlines, setAllAirlines] = useState([]);
    const [allDestinations, setAllDestinations] = useState([]);
    const [allPilots, setAllPilots] = useState([]);

    const { data: airlines, error: errorAirlines, isLoading: isLoadingAirlines } = useFetch(ENTITIES.AIRLINES, null, pageNumber);
    const { data: destinations, error: errorDestinations, isLoading: isLoadingDestinations } = useFetch(ENTITIES.DESTINATIONS, null, pageNumber);
    const { data: pilots, error: errorPilots, isLoading: isLoadingPilots } = useFetch(ENTITIES.PILOTS, null, pageNumber);

    const {
        departureDate,
        departureTime,
        airlineId,
        destinationId,
        pilotId,
        success,
        formError,
        isPending,
        handleChange,
        handleSubmit,
        setFormData,
    } = useCreateOperation(
        ENTITIES.FLIGHTS,
        ENTITY_PATHS.FLIGHTS,
        initialFormData,
        requiredFields,
        transformFlightForAPI
    );

    const handleCloseSnackbar = useCallback(() => {
        setFormData(prev => ({ ...prev, success: null, formError: null }));
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

        updateData(airlines, setAllAirlines);
        updateData(destinations, setAllDestinations);
        updateData(pilots, setAllPilots);

    }, [pageNumber, airlines, destinations, pilots, setAllAirlines, setAllDestinations, setAllPilots]);

    const handleLoadMore = () => {
        setPageNumber((prevPageNumber) => prevPageNumber + 1);
    };

    const isDataLoading = isLoadingAirlines || isLoadingDestinations || isLoadingPilots;
    const dataError = errorAirlines || errorDestinations || errorPilots;

    if (dataError) {
        return <Box sx={{ mt: 5 }}><CustomAlert alertType='danger' type='Error' message='Error loading dependency data.' /></Box>;
    }

    if (isDataLoading && allAirlines.length === 0 && allDestinations.length === 0 && allPilots.length === 0) {
        return <CircularProgress sx={{ mt: 5 }} />;
    }

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Flight' />

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
                <Grid container spacing={4}>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <FormControl fullWidth >
                            <InputLabel id="pilot-select-label">Pilot</InputLabel>
                            <Select
                                labelId="pilot-select-label"
                                id="pilotId"
                                name="pilotId"
                                value={pilotId}
                                label="Pilot"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">Select Pilot</MenuItem>
                                {allPilots.map((pilot) => (
                                    <MenuItem key={`pilot-${pilot.id}`} value={pilot.id}>
                                        {pilot.firstName} {pilot.lastName}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <FormControl fullWidth >
                            <InputLabel id="airline-select-label">Airline</InputLabel>
                            <Select
                                labelId="airline-select-label"
                                id="airlineId"
                                name="airlineId"
                                value={airlineId}
                                label="Airline"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">Select Airline</MenuItem>
                                {allAirlines.map((airline) => (
                                    <MenuItem key={`airline-${airline.id}`} value={airline.id}>
                                        {airline.name}
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 4 }}>
                        <FormControl fullWidth >
                            <InputLabel id="destination-select-label">Destination</InputLabel>
                            <Select
                                labelId="destination-select-label"
                                id="destinationId"
                                name="destinationId"
                                value={destinationId}
                                label="Destination"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">Select Destination</MenuItem>
                                {allDestinations.map((destination) => (
                                    <MenuItem key={`destination-${destination.id}`} value={destination.id}>
                                        {destination.city} ({destination.airport})
                                    </MenuItem>
                                ))}
                            </Select>
                        </FormControl>
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                        <TextField
                            id="departureDate"
                            name="departureDate"
                            label="Departure Date"
                            type="date"
                            variant="outlined"
                            value={departureDate}
                            onChange={handleChange}
                            required
                            fullWidth
                            InputLabelProps={{ shrink: true }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12, sm: 6, lg: 4, xl: 3 }}>
                        <TextField
                            id="departureTime"
                            name="departureTime"
                            label="Departure Time"
                            type="time"
                            variant="outlined"
                            value={departureTime}
                            onChange={handleChange}
                            required
                            fullWidth
                            InputLabelProps={{ shrink: true }}
                        />
                    </Grid>
                    <Grid size={{ xs: 12 }} sx={{ mt: 1 }}>
                        <Button
                            type="submit"
                            variant="contained"
                            color="success"
                            disabled={isPending || isDataLoading}
                        >
                            {isPending ? <CircularProgress size={24} color="inherit" /> : 'Create'}
                        </Button>
                        <Button
                            sx={{ ml: 3 }}
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
                <BackToListAction dataType={ENTITIES.FLIGHTS} />
            </Box>
        </Box>
    );
}