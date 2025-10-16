import React from 'react';
import { Snackbar, Alert } from '@mui/material';
import Slide from '@mui/material/Slide';

function Transition(props) {
    return <Slide {...props} direction="up" />;
}

export const CustomSnackbar = ({
    message,
    severity,
    onClose,
    duration = 4000,
    anchorOrigin = { vertical: 'bottom', horizontal: 'right' }
}) => {

    const isOpen = !!message;

    return (
        <Snackbar
            open={isOpen}
            autoHideDuration={duration}
            onClose={onClose}
            anchorOrigin={anchorOrigin}
            slots={{ transition: Transition }}
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