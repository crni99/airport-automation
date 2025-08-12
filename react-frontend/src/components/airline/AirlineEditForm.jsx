import React, { useState, useEffect } from 'react';
import { useNavigate, useParams } from 'react-router-dom';
import { useContext } from 'react';
import { DataContext } from '../../store/data-context.jsx';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import LoadingSpinner from '../common/LoadingSpinner';
import Alert from '../common/Alert.jsx';
import BackToListAction from '../common/pagination/BackToListAction.jsx';
import useFetch from '../../hooks/useFetch.jsx';
import { validateFields } from '../../utils/validation/validateFields.js';
import { Entities } from '../../utils/const.js';

export default function AirlineEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        name: '',
        error: null,
        isPending: false,
    });

    const { data: airlineData, isLoading, isError, error } = useFetch(Entities.AIRLINES, id);

    useEffect(() => {
        if (airlineData) {
            setFormData((prevState) => ({ ...prevState, name: airlineData.name || '' }));
        }
    }, [airlineData]);

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

        const airline = {
            Id: id,
            Name: formData.name,
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(airline, Entities.AIRLINES, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating airline:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update airline. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(Entities.AIRLINES, { ...prev, [name]: value }, ['name']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <>
            <PageTitle title='Edit Airline' />
            <div className="col-md-4">
                {formData.isPending && <LoadingSpinner />}
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
                    <BackToListAction dataType={Entities.AIRLINES} />
                </ul>
            </nav>
        </>
    );
}