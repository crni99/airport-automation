import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../../utils/create.js';
import PageTitle from '../common/PageTitle.jsx';
import Alert from '../common/Alert.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import { useContext } from 'react';
import { DataContext } from '../../store/data-context.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { Entities } from '../../utils/const.js';

export default function AirlineCreateForm() {
    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        name: '',
        error: null,
        isPending: false,
    });

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(Entities.AIRLINES, formData, ['name']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const airline = { Name: formData.name };
        setFormData({ ...formData, isPending: true, error: null });

        try {
            const create = await createData(airline, Entities.AIRLINES, dataCtx.apiUrl, navigate);

            if (create) {
                console.error('Error creating airline:', create.message);
                setFormData({ ...formData, error: create.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to create airline. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields('Airline', { ...prev, [name]: value }, ['name']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <>
            <PageTitle title='Create Airline' />
            <div className="col-md-4">
                <form onSubmit={handleSubmit}>
                    <div className="form-group pb-4">
                        <label htmlFor="name" className="control-label">Name</label>
                        <input
                            id="name"
                            type="text"
                            className="form-control"
                            name="name"
                            value={formData.name}
                            onChange={handleChange}
                            placeholder="Air Serbia"
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
                    <BackToListAction dataType={Entities.AIRLINES} />
                </ul>
            </nav>
        </>
    );
}