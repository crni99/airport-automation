import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';

export default function BackToListAction({ dataType }) {
    const navigate = useNavigate();

    const handleBackToList = () => {
        navigate(`/${dataType}`);
    };

    return (
        <Button
            variant="outlined"
            color="primary"
            onClick={handleBackToList}
            startIcon={<ArrowBackIcon />}
        >
            Back to List
        </Button>
    );
}