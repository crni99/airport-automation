import { useState, useEffect, useContext, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { DataContext } from '../store/DataContext.jsx';
import { updateData } from '../utils/httpUpdate.js';
import useFetch from './useFetch.jsx';
import { validateFields } from '../utils/validation/validateFields.js';

export const useUpdate = (dataType, dataPath, dataId, initialDataShape, requiredFields, transformDataForAPI, transformDataForForm) => {
    const dataCtx = useContext(DataContext);
    const [triggerFetch, setTriggerFetch] = useState(true);
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        ...initialDataShape,
        success: null,
        formError: null,
        validationError: null,
        isPending: false,
    });

    const { data: fetchedData, isLoading: isFetching, isError: isFetchError, error: fetchError } = useFetch(dataType, dataId, null, null, triggerFetch);

    useEffect(() => {
        if (fetchedData) {
            setTriggerFetch(false);
        }
    }, [fetchedData]);

    useEffect(() => {
        if (fetchedData) {
            const transformed = transformDataForForm(fetchedData);
            setFormData((prevState) => ({
                ...prevState,
                ...transformed,
                success: null,
                formError: null,
                validationError: null
            }));
        }
    }, [fetchedData, transformDataForForm]);

    const handleChange = useCallback((event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(dataType, { ...prev, [name]: value }, requiredFields);
            return {
                ...prev,
                [name]: value,
                success: null,
                formError: null,
                validationError: newError
            };
        });
    }, [dataType, requiredFields]);

    const handleSubmit = useCallback(async (event) => {
        event.preventDefault();

        const validationMessage = validateFields(dataType, formData, requiredFields);
        if (validationMessage) {
            setFormData((prev) => ({
                ...prev,
                success: null,
                formError: null,
                validationError: validationMessage
            }));
            return;
        }
        const apiPayload = transformDataForAPI(formData, dataId);

        setFormData((prevState) => ({ ...prevState, isPending: true, formError: null, validationError: null, success: null }));

        try {
            const result = await updateData(apiPayload, dataType, dataId, dataCtx.apiUrl);

            if (result && result.success) {
                setFormData((prevState) => ({
                    ...prevState,
                    success: result.message,
                    isPending: false,
                    formError: null,
                    validationError: null
                }));
                setTimeout(() => {
                    navigate(`${dataPath}/${dataId}`);
                }, 2000);
            } else {
                setFormData((prevState) => ({ ...prevState, success: null, formError: 'Update failed with an unknown response.', validationError: null, isPending: false }));
            }
        } catch (err) {
            console.error('Submission Error:', err);
            const errorMessage = err.message || 'Failed to update data due to an unexpected error.';

            setFormData((prevState) => ({
                ...prevState,
                success: null,
                formError: errorMessage,
                validationError: null,
                isPending: false
            }));
        }
    }, [dataType, dataPath, dataId, formData, requiredFields, transformDataForAPI, dataCtx.apiUrl, navigate]);

    return {
        ...formData,
        isFetching,
        isFetchError,
        fetchError,
        handleChange,
        handleSubmit,
        fetchedData,
        setFormData
    };
};