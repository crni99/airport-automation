import React from 'react';

const ClearInputButton = () => {
    const handleClear = () => {
        const inputs = document.querySelectorAll('.container input, .container select, .container textarea');

        inputs.forEach(input => {
            const type = input.type;

            if (type === 'checkbox' || type === 'radio') {
                input.checked = false;
            } else {
                input.value = '';
            }
        });
    };

    return (
        <button id="clearButton" className="btn btn-warning" onClick={handleClear}>
            Clear
        </button>
    );
};

export default ClearInputButton;
