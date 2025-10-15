import { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { deleteData } from '../utils/delete.js';
import { editData } from '../utils/edit.js';

export const useDataOperation = (entityType, entityId, apiUrl, redirectPath) => {
    const navigate = useNavigate();

    const [operationState, setOperationState] = useState({
        operationSuccess: null,
        operationError: null,
        isPending: false,
    });

    const handleCloseSnackbar = (event, reason) => {
        if (reason === 'clickaway') {
            return;
        }
        setOperationState(prevState => ({
            ...prevState,
            isPending: false,
            operationError: null,
            operationSuccess: null
        }));
    };

    const handleOperation = async (operation) => {
        setOperationState(prevState => ({
            ...prevState,
            isPending: true,
            operationError: null,
            operationSuccess: null
        }));

        try {
            let operationResult;

            if (operation === 'edit') {
                operationResult = await editData(entityType, entityId, apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(entityType, entityId, apiUrl, navigate);
            }

            if (operationResult && operationResult.success) {
                setOperationState(prevState => ({
                    ...prevState,
                    operationSuccess: operationResult.message,
                    ...(operation === 'delete' && { isPending: false }),
                }));

                if (operation === 'delete') {
                    setTimeout(() => {
                        navigate(redirectPath);
                    }, 3000);
                }
            }
        } catch (error) {
            const errorMessage = error.message || 'An unknown error occurred.';
            const errorType = error.type || 'Error';

            setOperationState(prevState => ({
                ...prevState,
                operationError: { type: errorType, message: errorMessage }
            }));
        } finally {
            if (operation !== 'delete') {
                setOperationState(prevState => ({ ...prevState, isPending: false }));
            }
        }
    };

    return {
        operationState,
        handleCloseSnackbar,
        handleOperation
    };
};