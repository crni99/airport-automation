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

export default function DestinationEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        city: '',
        airport: '',
        error: null,
        isPending: false,
    });

    const { data: destinationData, isLoading, isError, error } = useFetch(Entities.DESTINATIONS, id);

    useEffect(() => {
        if (destinationData) {
            setFormData((prevState) => ({ ...prevState, city: destinationData.city || '', airport: destinationData.airport || '' }));
        }
    }, [destinationData]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(Entities.DESTINATIONS, formData, ['city', 'airport']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const destination = {
            Id: id,
            City: formData.city,
            Airport: formData.airport
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(destination, Entities.DESTINATIONS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating destination:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update destination. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(Entities.DESTINATIONS, { ...prev, [name]: value }, ['city', 'airport']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <>
            <PageTitle title='Edit Destination' />
            <div className="col-md-4">
                {formData.isPending && <LoadingSpinner />}
                <form onSubmit={handleSubmit}>
                    <div className="form-group pb-3">
                        <label htmlFor="city" className="control-label">City</label>
                        <input
                            id="city"
                            type="text"
                            className="form-control"
                            name="city"
                            value={formData.city}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-4">
                        <label htmlFor="airport" className="control-label">Airport</label>
                        <input
                            id="airport"
                            type="text"
                            className="form-control"
                            name="airport"
                            value={formData.airport}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-3">
                        <button type="submit" className="btn btn-success" disabled={formData.isPending}>
                            {formData.isPending ? 'Submitting...' : 'Save Changes'}
                        </button>
                    </div>
                    {isLoading && <Alert alertType="info" alertText="Loading..." />}
                    {isError && error && (
                        <Alert alertType="error">
                            <strong>{error.type}</strong>: {error.message}
                        </Alert>
                    )}
                    {formData.error && <Alert alertType="error" alertText={formData.error} />}
                </form>
            </div>
            <nav aria-label="Page navigation">
                <ul className="pagination pagination-container pagination-container-absolute">
                    <BackToListAction dataType={Entities.DESTINATIONS} />
                </ul>
            </nav>
        </>
    );
}