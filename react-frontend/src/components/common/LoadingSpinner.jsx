import React from 'react';

export default function LoadingSpinner({ loadingText = 'Loading...' }) {
    return (
        <div className="text-center mt-4 mb-4">
            <div className="spinner-border" role="status">
                <span className="visually-hidden">{loadingText}</span>
            </div>
        </div>
    );
}
