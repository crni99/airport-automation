import React from 'react';
import { CustomSnackbar } from './CustomSnackbar';

const UpdateOperationSnackbarManager = ({ success, formError, fetchError, handleCloseSnackbar }) => {
    if (success) {
        return (
            <CustomSnackbar
                severity='success'
                message={success}
                onClose={handleCloseSnackbar}
            />
        );
    }
    if (formError) {
        return (
            <CustomSnackbar
                severity='error'
                message={formError}
                onClose={handleCloseSnackbar}
            />
        );
    }
    if (fetchError) {
        const errorMessage = fetchError.message || "Failed to load item details.";
        return (
            <CustomSnackbar
                severity='error'
                message={errorMessage}
                onClose={handleCloseSnackbar}
            />
        );
    }
    return null;
};

export default UpdateOperationSnackbarManager;