import React from 'react';

function PageInfo({ currentPage, totalPages, totalCount }) {
    return (
        <div
            id="pagedInfo"
            className="d-flex justify-content-between align-items-center mt-3 mb-2 p-2 border rounded small text-muted"
        >
            <div className="flex-fill text-center">
                <strong>Page:</strong> <span>{currentPage}</span>
            </div>
            <span className="mx-2">|</span>
            <div className="flex-fill text-center">
                <strong>Total Pages:</strong> <span>{totalPages}</span>
            </div>
            <span className="mx-2">|</span>
            <div className="flex-fill text-center">
                <strong>Total Records:</strong> <span>{totalCount}</span>
            </div>
        </div>
    );
}

export default PageInfo;
