import { useState, useContext } from 'react';
import { useNavigate } from 'react-router-dom';
import { deleteData } from '../utils/httpDelete.js';
import { DataContext } from '../store/DataContext.jsx';

export const useDeleteOperation = (entityType, entityId, redirectPath) => {
    const dataCtx = useContext(DataContext);
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

        if (operation !== 'delete') {
            console.warn(`useDataOperation called with unsupported operation: ${operation}`);
            return;
        }

        setOperationState(prevState => ({
            ...prevState,
            isPending: true,
            operationError: null,
            operationSuccess: null
        }));

        try {
            const operationResult = await deleteData(entityType, entityId, dataCtx.apiUrl);

            if (operationResult && operationResult.success) {
                setOperationState(prevState => ({
                    ...prevState,
                    operationSuccess: operationResult.message,
                }));

                setTimeout(() => {
                    setOperationState(prevState => ({ ...prevState, isPending: false }));
                    navigate(redirectPath);
                }, 2000);
            } else {
                throw new Error(operationResult?.message || 'Delete operation failed with no message.');
            }
        } catch (error) {
            const errorMessage = error.message || 'An unknown error occurred.';
            const errorType = error.type || 'Error';

            setOperationState(prevState => ({
                ...prevState,
                isPending: false,
                operationError: { type: errorType, message: errorMessage }
            }));
        }
    };

    return {
        operationState,
        handleCloseSnackbar,
        handleOperation
    };
};