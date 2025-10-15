import React, { useState, useContext, useEffect, useRef } from 'react';
import useFetch from '../../hooks/useFetch.jsx';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/create.js';
import PageTitle from '../../components/common/PageTitle.jsx';
import BackToListAction from '../../components/common/pagination/BackToListAction.jsx';
import { DataContext } from '../../store/DataContext.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { ENTITIES } from '../../utils/const.js';
import { formatTime } from '../../utils/formatting.js';
import Box from '@mui/material/Box';
import TextField from '@mui/material/TextField';
import Button from '@mui/material/Button';
import CircularProgress from '@mui/material/CircularProgress';
import Grid from '@mui/material/Grid';
import FormControl from '@mui/material/FormControl';
import InputLabel from '@mui/material/InputLabel';
import Select from '@mui/material/Select';
import MenuItem from '@mui/material/MenuItem';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";

export default function FlightCreateForm() {

    const [pageNumber, setPageNumber] = useState(1);
    const isInitialLoad = useRef(true);

    const [allAirlines, setAllAirlines] = useState([]);
    const [allDestinations, setAllDestinations] = useState([]);
    const [allPilots, setAllPilots] = useState([]);

    const { data: airlines, error: errorAirlines, isLoading: isLoadingAirlines } = useFetch(ENTITIES.AIRLINES, null, pageNumber);
    const { data: destinations, error: errorDestinations, isLoading: isLoadingDestinations } = useFetch(ENTITIES.DESTINATIONS, null, pageNumber);
    const { data: pilots, error: errorPilots, isLoading: isLoadingPilots } = useFetch(ENTITIES.PILOTS, null, pageNumber);

    const dataCtx = useContext(DataContext);
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
            DepartureDate: formData.departureDate,
            DepartureTime: formattedDepartureTime,
            AirlineId: formData.airlineId,
            DestinationId: formData.destinationId,
            PilotId: formData.pilotId,
        };

        setFormData({ ...formData, isPending: true, error: null });

        try {
            const create = await createData(flight, ENTITIES.FLIGHTS, dataCtx.apiUrl, navigate);

            if (create) {
                console.error('Error creating flight:', create.message);
                setFormData({ ...formData, error: create.message, isPending: false });
            } else {
                setFormData({ ...formData, error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to create flight. Please try again.', isPending: false });
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
        if (airlines?.data && airlines.data.length > 0) {
            setAllAirlines((prev) => {
                const newAirlines = airlines.data.filter(
                    (airline) => !prev.some((prevAirline) => prevAirline.id === airline.id)
                );
                return [...prev, ...newAirlines];
            });
        }
        if (destinations?.data && destinations.data.length > 0) {
            setAllDestinations((prev) => {
                const newDestinations = destinations.data.filter(
                    (destination) => !prev.some((prevDestination) => prevDestination.id === destination.id)
                );
                return [...prev, ...newDestinations];
            });
        }
        if (pilots?.data && pilots.data.length > 0) {
            setAllPilots((prev) => {
                const newPilots = pilots.data.filter(
                    (pilot) => !prev.some((prevPilot) => prevPilot.id === pilot.id)
                );
                return [...prev, ...newPilots];
            });
        }
    }, [pageNumber, airlines?.data, destinations?.data, pilots?.data]);

    if (isLoadingAirlines || isLoadingDestinations || isLoadingPilots) {
        return <CircularProgress />
    }

    if (errorAirlines || errorDestinations || errorPilots) {
        return <CustomAlert alertType='danger' alertText='Error loading data..' />;
    }

    return (
        <Box sx={{ mt: 5 }}>
            <PageTitle title='Create Flight' />

            {(isLoadingAirlines || isLoadingDestinations || isLoadingPilots) && (
                <CircularProgress sx={{ mb: 2 }} />
            )}

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
                                value={formData.pilotId}
                                label="Pilot"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">Select Pilot</MenuItem>
                                {allPilots?.map((pilot) => (
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
                                value={formData.airlineId}
                                label="Airline"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">Select Airline</MenuItem>
                                {allAirlines?.map((airline) => (
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
                                value={formData.destinationId}
                                label="Destination"
                                onChange={handleChange}
                                required
                            >
                                <MenuItem value="">Select Destination</MenuItem>
                                {allDestinations?.map((destination) => (
                                    <MenuItem key={`destination-${destination.id}`} value={destination.id}>
                                        {destination.city} {destination.airport}
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
                            {formData.isPending ? <CircularProgress size={24} color="inherit" /> : 'Create'}
                        </Button>
                        <Button
                            sx={{ ml: 3 }}
                            variant="outlined"
                            onClick={handleLoadMore}
                            disabled={isLoadingAirlines || isLoadingDestinations || isLoadingPilots}
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
                <BackToListAction dataType={ENTITIES.FLIGHTS} />
            </Box>
        </Box>
    );
}