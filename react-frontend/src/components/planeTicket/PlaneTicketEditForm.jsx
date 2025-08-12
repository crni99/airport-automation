import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useContext } from 'react';
import { DataContext } from '../../store/data-context.jsx';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import LoadingSpinner from '../common/LoadingSpinner.jsx';
import Alert from '../common/Alert.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import useFetch from '../../hooks/useFetch.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { Entities } from '../../utils/const.js';

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

    const { data: planeTicket, isLoading, isError, error } = useFetch(Entities.PLANE_TICKETS, id);

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

        const errorMessage = validateFields(Entities.PLANE_TICKETS, formData, ['price', 'purchaseDate', 'seatNumber', 'passengerId', 'travelClassId', 'flightId']);
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
            const edit = await editData(planeTicket, Entities.PLANE_TICKETS, id, dataCtx.apiUrl, navigate);

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
            const newError = validateFields(Entities.PLANE_TICKETS, { ...prev, [name]: value }, ['price', 'purchaseDate', 'seatNumber', 'passengerId', 'travelClassId', 'flightId']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <>
            <PageTitle title='Edit Plane Ticket' />
            {formData.isPending && <LoadingSpinner />}
            <form onSubmit={handleSubmit}>
                <div className='row'>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="id" className="control-label">Id</label>
                            <input
                                id="id"
                                type="number"
                                className="form-control"
                                name="id"
                                value={id}
                                required
                                readOnly
                            />
                        </div>
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
                            <button type="submit" className="btn btn-success" disabled={formData.isPending}>
                                {formData.isPending ? 'Submitting...' : 'Save Changes'}
                            </button>
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="passengerId" className="control-label">Passenger Id</label>
                            <input
                                id="passengerId"
                                type="number"
                                className="form-control"
                                name="passengerId"
                                value={formData.passengerId}
                                required
                                readOnly
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="travelClassId" className="control-label">Travel Class Id</label>
                            <input
                                id="travelClassId"
                                type="number"
                                className="form-control"
                                name="travelClassId"
                                value={formData.travelClassId}
                                required
                                readOnly
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="flightId" className="control-label">Flight Id</label>
                            <input
                                id="flightId"
                                type="number"
                                className="form-control"
                                name="flightId"
                                value={formData.flightId}
                                required
                                readOnly
                            />
                        </div>
                    </div>
                </div>
                {isLoading && <Alert alertType="info" alertText="Loading..." />}
                {isError && error && (
                    <Alert alertType="error">
                        <strong>{error.type}</strong>: {error.message}
                    </Alert>
                )}
                {formData.error && <Alert alertType="error" alertText={formData.error} />}
            </form>
            <nav aria-label="Page navigation">
                <ul className="pagination pagination-container pagination-container-absolute">
                    <BackToListAction dataType={Entities.PLANE_TICKETS} />
                </ul>
            </nav>
        </>
    );
}