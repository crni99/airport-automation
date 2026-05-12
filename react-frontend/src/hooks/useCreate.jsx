import { useState, useContext, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../utils/httpCreate.js';
import { DataContext } from '../store/DataContext.jsx';
import { validateFields } from '../utils/validation/validateFields.js';

export const useCreate = (dataType, dataPath, initialDataShape, requiredFields, transformDataForAPI) => {
    const dataCtx = useContext(DataContext);
    const navigate = useNavigate();

    const [formData, setFormData] = useState({
        ...initialDataShape,
        success: null,
        formError: null,
        validationError: null,
        isPending: false,
    });

    const handleChange = useCallback((event) => {
        const { name, value } = event.target;
        setFormData((prev) => {
            const newError = validateFields(dataType, { ...prev, [name]: value }, requiredFields);
            return {
                ...prev,
                [name]: value,
                success: null,
                formError: null,
                validationError: newError,
            };
        });
    }, [dataType, requiredFields]);

    const handleSubmit = useCallback(async (event) => {
        event.preventDefault();

        setFormData((prev) => {
            const validationMessage = validateFields(dataType, prev, requiredFields);
            if (validationMessage) {
                return { ...prev, success: null, formError: null, validationError: validationMessage };
            }

            const apiPayload = transformDataForAPI(prev);

            createData(apiPayload, dataType, dataCtx.apiUrl)
                .then((result) => {
                    if (result && result.success) {
                        const resetData = Object.keys(initialDataShape).reduce((acc, key) => ({ ...acc, [key]: '' }), {});
                        setFormData((s) => ({
                            ...s,
                            ...resetData,
                            success: result.message,
                            isPending: false,
                            formError: null,
                            validationError: null,
                        }));
                        setTimeout(() => navigate(`${dataPath}/${result.newId}`), 2000);
                    } else {
                        const errorMessage = result?.message || 'Creation failed with an unknown response.';
                        setFormData((s) => ({ ...s, success: null, formError: errorMessage, validationError: null, isPending: false }));
                    }
                })
                .catch((err) => {
                    const errorMessage = err.message || `Failed to create ${dataType} due to an unexpected error.`;
                    setFormData((s) => ({ ...s, success: null, formError: errorMessage, validationError: null, isPending: false }));
                });

            return { ...prev, isPending: true, formError: null, validationError: null, success: null };
        });
    }, [dataType, dataPath, requiredFields, transformDataForAPI, dataCtx.apiUrl, navigate, initialDataShape]);

    return {
        ...formData,
        handleChange,
        handleSubmit,
        setFormData
    };
};