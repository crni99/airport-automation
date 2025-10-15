import React from 'react';
import { Snackbar, Alert } from '@mui/material';

export const CustomSnackbar = ({
    message,
    severity,
    onClose,
    duration = 6000,
    anchorOrigin = { vertical: 'bottom', horizontal: 'right' }
}) => {

    const isOpen = !!message;

    return (
        <Snackbar
            open={isOpen}
            autoHideDuration={duration}
            onClose={onClose}
            anchorOrigin={anchorOrigin}
        >
            <Alert
                onClose={onClose}
                severity={severity}
                sx={{ width: '100%' }}
                variant="filled"
            >
                {message}
            </Alert>
        </Snackbar>
    );
};