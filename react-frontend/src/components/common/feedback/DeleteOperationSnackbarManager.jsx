import React from 'react';
import { CustomSnackbar } from './CustomSnackbar';

const DeleteOperationSnackbarManager = ({ operationState, error, handleCloseSnackbar }) => {
    if (operationState?.operationSuccess) {
        return (
            <CustomSnackbar
                severity='success'
                message={operationState.operationSuccess}
                onClose={handleCloseSnackbar}
            />
        );
    }
    if (error) {
        return (
            <CustomSnackbar
                severity='error'
                message={error.message}
                onClose={handleCloseSnackbar}
            />
        );
    }
    if (operationState?.operationError) {
        return (
            <CustomSnackbar
                severity='error'
                message={operationState.operationError.message}
                onClose={handleCloseSnackbar}
            />
        );
    }
    return null;
};

export default DeleteOperationSnackbarManager;