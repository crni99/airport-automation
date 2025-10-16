import { useState, useEffect, useContext, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { DataContext } from '../store/DataContext.jsx';
import { editData } from '../utils/edit.js';
import useFetch from './useFetch.jsx';
import { validateFields } from '../utils/validation/validateFields.js';

export const useEditForm = (dataType, dataPath, dataId, initialDataShape, requiredFields, transformDataForAPI, transformDataForForm) => {
    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        ...initialDataShape,
        success: null,
        formError: null,
        isPending: false,
    });

    const { data: fetchedData, isLoading: isFetching, isError: isFetchError, error: fetchError } = useFetch(dataType, dataId);

    const transformDataForFormCallback = useCallback(transformDataForForm, [transformDataForForm]);

    useEffect(() => {
        if (fetchedData) {
            const transformed = transformDataForFormCallback(fetchedData);
            setFormData((prevState) => ({
                ...prevState,
                ...transformed,
                success: null,
                formError: null
            }));
        }
    }, [fetchedData, transformDataForFormCallback]);

    const handleChange = useCallback((event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(dataType, { ...prev, [name]: value }, requiredFields);
            return {
                ...prev,
                [name]: value,
                success: null,
                formError: newError
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
                formError: validationMessage,
            }));
            return;
        }
        const apiPayload = transformDataForAPI(formData, dataId);

        setFormData((prevState) => ({ ...prevState, isPending: true, formError: null, success: null }));

        try {
            const result = await editData(apiPayload, dataType, dataId, dataCtx.apiUrl);

            if (result && result.success) {
                setFormData((prevState) => ({
                    ...prevState,
                    success: result.message,
                    isPending: false,
                    formError: null,
                }));
                setTimeout(() => {
                    navigate(`${dataPath}/${dataId}`);
                }, 2000);
            } else {
                setFormData((prevState) => ({ ...prevState, success: null, formError: 'Update failed with an unknown response.', isPending: false }));
            }
        } catch (err) {
            console.error('Submission Error:', err);
            const errorMessage = err.message || 'Failed to update data due to an unexpected error.';

            setFormData((prevState) => ({
                ...prevState,
                success: null,
                formError: errorMessage,
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
        fetchedData
    };
};