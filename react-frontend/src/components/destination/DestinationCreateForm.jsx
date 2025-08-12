import React, { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/create.js';
import PageTitle from '../common/PageTitle.jsx';
import Alert from '../common/Alert.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import { DataContext } from '../../store/data-context.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { Entities } from '../../utils/const.js';

export default function DestinationCreateForm() {
    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        city: '',
        airport: '',
        error: null,
        isPending: false,
    });

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

        const destination = { City: formData.city, Airport: formData.airport };
        setFormData({ ...formData, isPending: true, error: null });

        try {
            const create = await createData(destination, Entities.DESTINATIONS, dataCtx.apiUrl, navigate);

            if (create) {
                console.error('Error creating destination:', create.message);
                setFormData({ ...formData, error: create.message, isPending: false });
            } else {
                setFormData({ city: '', airport: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to create destination. Please try again.', isPending: false });
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
            <PageTitle title='Create Destination' />
            <div className="col-md-4">
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
                            placeholder="Belgrade"
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
                            placeholder="Belgrade Nikola Tesla Airport"
                            required
                        />
                    </div>
                    <div className="form-group pb-3">
                        <button type="submit" className="btn btn-success" disabled={formData.isPending}>
                            {formData.isPending ? 'Creating...' : 'Create'}
                        </button>
                    </div>
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