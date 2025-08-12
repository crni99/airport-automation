import React, { useState, useContext, useEffect, useRef } from 'react';
import useFetch from '../../hooks/useFetch.jsx';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/create.js';
import PageTitle from '../common/PageTitle.jsx';
import Alert from '../common/Alert.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import { DataContext } from '../../store/data-context.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { Entities } from '../../utils/const.js';
import LoadingSpinner from '../common/LoadingSpinner.jsx';
import { formatTime } from '../../utils/formatting.js';

export default function FlightCreateForm() {

    const [pageNumber, setPageNumber] = useState(1);
    const isInitialLoad = useRef(true);

    const [allAirlines, setAllAirlines] = useState([]);
    const [allDestinations, setAllDestinations] = useState([]);
    const [allPilots, setAllPilots] = useState([]);

    const { data: airlines, error: errorAirlines, isLoading: isLoadingAirlines } = useFetch(Entities.AIRLINES, null, pageNumber);
    const { data: destinations, error: errorDestinations, isLoading: isLoadingDestinations } = useFetch(Entities.DESTINATIONS, null, pageNumber);
    const { data: pilots, error: errorPilots, isLoading: isLoadingPilots } = useFetch(Entities.PILOTS, null, pageNumber);

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

        const errorMessage = validateFields(Entities.FLIGHTS, formData, ['departureDate', 'departureTime', 'airlineId', 'destinationId', 'pilotId']);
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
            const create = await createData(flight, Entities.FLIGHTS, dataCtx.apiUrl, navigate);

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
        return <LoadingSpinner />
    }

    if (errorAirlines || errorDestinations || errorPilots) {
        return <Alert alertType='danger' alertText='Error loading data..' />;
    }

    return (
        <>
            <PageTitle title='Create Flight' />
            <form onSubmit={handleSubmit}>
                <div className='row'>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="departureDate" className="control-label">Departure Date</label>
                            <input
                                id="departureDate"
                                type="date"
                                className="form-control"
                                name="departureDate"
                                value={formData.departureDate}
                                onChange={handleChange}
                                placeholder="1-dec-1999"
                                required
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="airlineId" className="control-label">Airline</label>
                            <select
                                id="airlineId"
                                name="airlineId"
                                className="form-control"
                                value={formData.airlineId}
                                onChange={handleChange}
                            >
                                <option value="">Select Airline</option>
                                {allAirlines?.map((airline) => (
                                    <option key={`airline-${airline.id}`} value={airline.id}>
                                        {airline.name}
                                    </option>
                                ))}
                            </select>
                        </div>
                        <div className="form-group pb-3">
                            <button type="submit" className="btn btn-success" disabled={formData.isPending}>
                                {formData.isPending ? 'Creating...' : 'Create'}
                            </button>
                        </div>
                        <div className="form-group">
                            <input
                                type="button"
                                value="Load More"
                                className="btn btn-primary"
                                id="loadMoreButton"
                                onClick={handleLoadMore}
                                disabled={isLoadingAirlines || isLoadingDestinations || isLoadingPilots} />
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="departureTime" className="control-label">Departure Time</label>
                            <input
                                id="departureTime"
                                type="time"
                                className="form-control"
                                name="departureTime"
                                value={formData.departureTime}
                                onChange={handleChange}
                                placeholder="21:00"
                                required
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="destinationId" className="control-label">Destination</label>
                            <select
                                id="destinationId"
                                name="destinationId"
                                className="form-control"
                                value={formData.destinationId}
                                onChange={handleChange}
                            >
                                <option value="">Select Destination</option>
                                {allDestinations?.map((destination) => (
                                    <option key={`destination-${destination.id}`} value={destination.id}>
                                        {destination.city} {destination.airport}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="mb-3 form-column-align">
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="pilotId" className="control-label">Pilot</label>
                            <select
                                id="pilotId"
                                name="pilotId"
                                className="form-control"
                                value={formData.pilotId}
                                onChange={handleChange}
                            >
                                <option value="">Select Pilot</option>
                                {allPilots?.map((pilot) => (
                                    <option key={`pilot-${pilot.id}`} value={pilot.id}>
                                        {pilot.firstName} {pilot.lastName}
                                    </option>
                                ))}
                            </select>
                        </div>
                        {formData.error && <Alert alertType="error" alertText={formData.error} />}
                    </div>
                </div>
            </form>

            <nav aria-label="Page navigation">
                <ul className="pagination pagination-container pagination-container-absolute">
                    <BackToListAction dataType={Entities.FLIGHTS} />
                </ul>
            </nav>
        </>
    );
}
