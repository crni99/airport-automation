import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { ENTITIES } from '../../../utils/const';

export default function BackToListAction({ dataType }) {
    const navigate = useNavigate();

    var pathType = dataType;
    if (dataType === ENTITIES.PLANE_TICKETS) {
        pathType = 'plane-tickets';
    }
    else if (dataType === ENTITIES.API_USERS) {
        pathType = 'api-users';
    }

    const handleBackToList = () => {
        navigate(`/${pathType}`);
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