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

export default function PassengerEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        firstName: '',
        lastName: '',
        uprn: '',
        passport: '',
        address: '',
        phone: '',
        error: null,
        isPending: false,
    });

    const { data: passengerData, isLoading, isError, error } = useFetch(Entities.PASSENGERS, id);

    useEffect(() => {
        if (passengerData) {
            setFormData((prevState) => ({
                ...prevState,
                firstName: passengerData.firstName || '',
                lastName: passengerData.lastName || '',
                uprn: passengerData.uprn || '',
                passport: passengerData.passport || '',
                address: passengerData.address || '',
                phone: passengerData.phone || '',
            }));
        }
    }, [passengerData]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(Entities.PASSENGERS, formData, ['firstName', 'lastName', 'uprn', 'passport', 'address', 'phone']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const passenger = {
            Id: id,
            FirstName: formData.firstName,
            LastName: formData.lastName,
            UPRN: formData.uprn,
            Passport: formData.passport,
            Address: formData.address,
            Phone: formData.phone
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(passenger, Entities.PASSENGERS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating passenger:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update passenger. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(Entities.PASSENGERS, { ...prev, [name]: value }, ['firstName', 'lastName', 'uprn', 'passport', 'address', 'phone']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <>
            <PageTitle title='Edit Passenger' />
            <div className="col-md-4">
                {formData.isPending && <LoadingSpinner />}
                <form onSubmit={handleSubmit}>
                    <div className="form-group pb-3">
                        <label htmlFor="firstName" className="control-label">First Name</label>
                        <input
                            id="firstName"
                            type="text"
                            className="form-control"
                            name="firstName"
                            value={formData.firstName}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-3">
                        <label htmlFor="lastName" className="control-label">Last Name</label>
                        <input
                            id="lastName"
                            type="text"
                            className="form-control"
                            name="lastName"
                            value={formData.lastName}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-3">
                        <label htmlFor="uprn" className="control-label">UPRN</label>
                        <input
                            id="uprn"
                            type="text"
                            className="form-control"
                            name="uprn"
                            value={formData.uprn}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-3">
                        <label htmlFor="passport" className="control-label">Passport</label>
                        <input
                            id="passport"
                            type="text"
                            className="form-control"
                            name="passport"
                            value={formData.passport}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-3">
                        <label htmlFor="address" className="control-label">Address</label>
                        <input
                            id="address"
                            type="text"
                            className="form-control"
                            name="address"
                            value={formData.address}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-4">
                        <label htmlFor="phone" className="control-label">Phone</label>
                        <input
                            id="phone"
                            type="text"
                            className="form-control"
                            name="phone"
                            value={formData.phone}
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
                    <BackToListAction dataType={Entities.PASSENGERS} />
                </ul>
            </nav>
        </>
    );
}