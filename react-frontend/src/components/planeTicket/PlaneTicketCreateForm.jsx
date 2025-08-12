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

export default function PlaneTicketCreateForm() {

    const [pageNumber, setPageNumber] = useState(1);
    const isInitialLoad = useRef(true);

    const [allPassengers, setAllPassengers] = useState([]);
    const [allTravelClasses, setAllTravelClasses] = useState([]);
    const [allFlights, setAllFlights] = useState([]);

    const { data: passengers, error: errorPassengers, isLoading: isLoadingPassengers } = useFetch(Entities.PASSENGERS, null, pageNumber);
    const { data: travelClasses, error: errorTravelClasses, isLoading: isLoadingTravelClasses } = useFetch(Entities.TRAVEL_CLASSES, null, pageNumber);
    const { data: flights, error: errorFlights, isLoading: isLoadingFlights } = useFetch(Entities.FLIGHTS, null, pageNumber);

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

        const errorMessage = validateFields(Entities.PLANE_TICKETS, formData, ['price', 'purchaseDate', 'seatNumber', 'passengerId', 'travelClassId', 'flightId']);
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
            const create = await createData(planeTicket, Entities.PLANE_TICKETS, dataCtx.apiUrl, navigate);

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
        return <LoadingSpinner />
    }

    if (errorPassengers || errorTravelClasses || errorFlights) {
        return <Alert alertType='danger' alertText='Error loading data..' />;
    }
    return (
        <>
            <PageTitle title='Create Plane Ticket' />
            <form onSubmit={handleSubmit}>
                <div className='row'>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="price" className="control-label">Price</label>
                            <input
                                id="price"
                                type="number"
                                className="form-control"
                                name="price"
                                value={formData.price}
                                onChange={handleChange}
                                placeholder="999"
                                required
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="passengerId" className="control-label">Passenger</label>
                            <select
                                id="passengerId"
                                name="passengerId"
                                className="form-control"
                                value={formData.passengerId}
                                onChange={handleChange}
                            >
                                <option value="">Select Passenger</option>
                                {allPassengers?.map((passenger) => (
                                    <option key={`passenger-${passenger.id}`} value={passenger.id}>
                                        {passenger.firstName} {passenger.lastName}
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
                                disabled={isLoadingPassengers || isLoadingTravelClasses || isLoadingFlights} />
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="purchaseDate" className="control-label">Purchase Date</label>
                            <input
                                id="purchaseDate"
                                type="date"
                                className="form-control"
                                name="purchaseDate"
                                value={formData.purchaseDate}
                                onChange={handleChange}
                                placeholder="1-dec-1999"
                                required
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="travelClassId" className="control-label">Travel Class</label>
                            <select
                                id="travelClassId"
                                name="travelClassId"
                                className="form-control"
                                value={formData.travelClassId}
                                onChange={handleChange}
                            >
                                <option value="">Select Travel Class</option>
                                {allTravelClasses?.map((travelClass) => (
                                    <option key={`destination-${travelClass.id}`} value={travelClass.id}>
                                        {travelClass.type}
                                    </option>
                                ))}
                            </select>
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="seatNumber" className="control-label">Seat Number</label>
                            <input
                                id="seatNumber"
                                type="number"
                                className="form-control"
                                name="seatNumber"
                                value={formData.seatNumber}
                                onChange={handleChange}
                                placeholder="1"
                                required
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="flightId" className="control-label">Flight</label>
                            <select
                                id="flightId"
                                name="flightId"
                                className="form-control"
                                value={formData.flightId}
                                onChange={handleChange}
                            >
                                <option value="">Select Flight</option>
                                {allFlights?.map((flight) => (
                                    <option key={`pilot-${flight.id}`} value={flight.id}>
                                        {flight.departureDate} {flight.departureTime}
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
