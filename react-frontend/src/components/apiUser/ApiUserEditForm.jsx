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

export default function ApiUserEditForm() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        userName: '',
        password: '',
        roles: '',
        error: null,
        isPending: false,
    });

    const { data: apiUserData, isLoading, isError, error } = useFetch(Entities.API_USERS, id);

    useEffect(() => {
        if (apiUserData) {
            setFormData((prevState) => ({
                ...prevState,
                userName: apiUserData.userName || '',
                password: apiUserData.password || '',
                roles: apiUserData.roles || ''
            }));
        }
    }, [apiUserData]);

    const handleSubmit = async (event) => {
        event.preventDefault();

        const errorMessage = validateFields(Entities.API_USERS, formData, ['userName', 'password', 'roles']);
        if (errorMessage) {
            setFormData({
                ...formData,
                error: errorMessage,
            });
            return;
        }

        const apiUser = {
            Id: id,
            UserName: formData.userName,
            Password: formData.password,
            Roles: formData.roles
        };

        setFormData((prevState) => ({ ...prevState, isPending: true }));

        try {
            const edit = await editData(apiUser, Entities.API_USERS, id, dataCtx.apiUrl, navigate);

            if (edit) {
                console.error('Error updating api user:', edit.message);
                setFormData({ ...formData, error: edit.message, isPending: false });
            } else {
                setFormData({ name: '', error: null, isPending: false });
            }
        } catch (err) {
            console.error('Error during API call:', err);
            setFormData({ ...formData, error: 'Failed to update api user. Please try again.', isPending: false });
        }
    };

    const handleChange = (event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(Entities.API_USERS, { ...prev, [name]: value }, ['userName', 'password', 'roles']);
            return { ...prev, [name]: value, error: newError };
        });
    };

    return (
        <>
            <PageTitle title='Edit Api User' />
            <div className="col-md-4">
                {formData.isPending && <LoadingSpinner />}
                <form onSubmit={handleSubmit}>
                    <div className="form-group pb-4">
                        <label htmlFor="userName" className="control-label">Username</label>
                        <input
                            id="userName"
                            type="text"
                            className="form-control"
                            name="userName"
                            value={formData.userName}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-4">
                        <label htmlFor="password" className="control-label">Password</label>
                        <input
                            id="password"
                            type="text"
                            className="form-control"
                            name="password"
                            value={formData.password}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group pb-4">
                        <label htmlFor="roles" className="control-label">Roles</label>
                        <input
                            id="roles"
                            type="text"
                            className="form-control"
                            name="roles"
                            value={formData.roles}
                            onChange={handleChange}
                            required
                        />
                    </div>
                    <div className="form-group">
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
                    <BackToListAction dataType={Entities.API_USERS} />
                </ul>
            </nav>
        </>
    );
}