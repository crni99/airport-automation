import { useRef, useState, useContext, useCallback } from 'react';
import { useNavigate } from 'react-router-dom';
import { createData } from '../utils/httpCreate.js';
import { DataContext } from '../store/DataContext.jsx';
import { validateFields } from '../utils/validation/validateFields.js';
import { generateContentIdempotencyKey } from '../utils/idempotency.js';

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

        const validationMessage = validateFields(dataType, formData, requiredFields);
        if (validationMessage) {
            setFormData((prev) => ({ ...prev, success: null, formError: null, validationError: validationMessage }));
            return;
        }

        setFormData((prev) => ({ ...prev, isPending: true, formError: null, validationError: null, success: null }));

        const apiPayload = transformDataForAPI(formData);
        const idempotencyKey = await generateContentIdempotencyKey(dataType, apiPayload);

        try {
            const result = await createData(apiPayload, dataType, dataCtx.apiUrl, idempotencyKey);
            if (result?.success) {
                const resetData = Object.keys(initialDataShape).reduce((acc, key) => ({ ...acc, [key]: '' }), {});
                setFormData((s) => ({ ...s, ...resetData, success: result.message, isPending: false, formError: null, validationError: null }));
                setTimeout(() => navigate(`${dataPath}/${result.newId}`), 2000);
            } else {
                setFormData((s) => ({ ...s, success: null, formError: result?.message || 'Creation failed with an unknown response.', validationError: null, isPending: false }));
            }
        } catch (err) {
            setFormData((s) => ({ ...s, success: null, formError: err.message || `Failed to create ${dataType} due to an unexpected error.`, validationError: null, isPending: false }));
        }
    }, [dataType, dataPath, formData, requiredFields, transformDataForAPI, dataCtx.apiUrl, navigate, initialDataShape]);

    return {
        ...formData,
        handleChange,
        handleSubmit,
        setFormData
    };
};