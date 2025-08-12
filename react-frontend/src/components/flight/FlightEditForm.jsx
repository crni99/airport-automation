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
import { formatTime } from '../../utils/formatting.js';

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

    const { data: flightData, isLoading, isError, error } = useFetch(Entities.FLIGHTS, id);

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
            Id: id,
            DepartureDate: formData.departureDate,
            DepartureTime: formattedDepartureTime,
            AirlineId: formData.airlineId,
            DestinationId: formData.destinationId,
            PilotId: formData.pilotId,
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(flight, Entities.FLIGHTS, id, dataCtx.apiUrl, navigate);

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
            const newError = validateFields(Entities.FLIGHTS, { ...prev, [name]: value }, ['departureDate', 'departureTime', 'airlineId', 'destinationId', 'pilotId']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <>
            <PageTitle title='Edit Flight' />
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
                            <button type="submit" className="btn btn-success" disabled={formData.isPending}>
                                {formData.isPending ? 'Submitting...' : 'Save Changes'}
                            </button>
                        </div>
                    </div>
                    <div className="col-md-4">
                        <div className="form-group pb-3">
                            <label htmlFor="airlineId" className="control-label">Airline Id</label>
                            <input
                                id="airlineId"
                                type="number"
                                className="form-control"
                                name="airlineId"
                                value={formData.airlineId}
                                required
                                readOnly
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="destinationId" className="control-label">Destination Id</label>
                            <input
                                id="destinationId"
                                type="number"
                                className="form-control"
                                name="destinationId"
                                value={formData.destinationId}
                                required
                                readOnly
                            />
                        </div>
                        <div className="form-group pb-3">
                            <label htmlFor="pilotId" className="control-label">Pilot Id</label>
                            <input
                                id="pilotId"
                                type="number"
                                className="form-control"
                                name="pilotId"
                                value={formData.pilotId}
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
                    <BackToListAction dataType={Entities.FLIGHTS} />
                </ul>
            </nav>
        </>
    );
}