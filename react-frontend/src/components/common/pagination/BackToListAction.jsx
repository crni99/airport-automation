import React from 'react';
import { useNavigate } from 'react-router-dom';
import Button from '@mui/material/Button';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { ENTITIES, ENTITY_PATHS } from '../../../utils/const';

export default function BackToListAction({ dataType }) {
    
    const navigate = useNavigate();

    const pathType = ENTITY_PATHS[Object.keys(ENTITIES).find(k => ENTITIES[k] === dataType)];

    const handleBackToList = () => {
        navigate(pathType);
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