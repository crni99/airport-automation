import React from 'react';

function SearchButton({ onClick }) {
    return (
        <button
            id="searchButton"
            className="btn btn-primary"
            onClick={onClick}
        >
            Search
        </button>
    );
}

export default SearchButton;
