import React from 'react';
import { CustomSnackbar } from './CustomSnackbar';

const CreateSnackbarManager = ({ success, formError, handleCloseSnackbar }) => {
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
    return null;
};

export default CreateSnackbarManager;