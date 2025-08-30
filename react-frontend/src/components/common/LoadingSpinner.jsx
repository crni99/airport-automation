import React from 'react';

export default function LoadingSpinner({ loadingText = 'Loading...' }) {
    return (
        <div className="text-center">
            <div className="spinner-border" role="status">
                <span className="visually-hidden">{loadingText}</span>
            </div>
        </div>
    );
}
