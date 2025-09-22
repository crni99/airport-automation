import React from 'react';
import Button from '@mui/material/Button';
import SearchIcon from '@mui/icons-material/Search';

function SearchButton({ onClick }) {
    return (
        <Button
            id="searchButton"
            variant="contained"
            color="primary"
            onClick={onClick}
            startIcon={<SearchIcon />}
        >
            Search
        </Button>
    );
}

export default SearchButton;