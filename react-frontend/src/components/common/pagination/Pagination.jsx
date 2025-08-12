import React, { useEffect } from "react";

export function Pagination({
    pageNumber,
    lastPage,
    onPageChange,
    rowsPerPage,
    onRowsPerPageChange
}) {
    
    useEffect(() => {
        const savedRows = localStorage.getItem("rowsPerPage");
        if (savedRows && !isNaN(savedRows)) {
            onRowsPerPageChange(Number(savedRows));
        }
    }, [onRowsPerPageChange]);

    const handleRowsChange = (e) => {
        const value = Number(e.target.value);
        localStorage.setItem("rowsPerPage", value);
        onRowsPerPageChange(value);
    };

    return (
        <nav aria-label="Page navigation" className="mb-2">
            <div className="d-flex justify-content-between align-items-center flex-wrap mt-3 mb-3">
                <ul className="pagination mb-0">
                    <li className={`page-item ${pageNumber === 1 ? "disabled" : ""}`}>
                        <button className="page-link" onClick={() => onPageChange(1)}>First Page</button>
                    </li>
                    <li className={`page-item ${pageNumber === 1 ? "disabled" : ""}`}>
                        <button className="page-link" onClick={() => onPageChange(pageNumber - 1)}>Previous</button>
                    </li>
                    <li className={`page-item ${pageNumber === lastPage ? "disabled" : ""}`}>
                        <button className="page-link" onClick={() => onPageChange(pageNumber + 1)}>Next</button>
                    </li>
                    <li className={`page-item ${pageNumber === lastPage ? "disabled" : ""}`}>
                        <button className="page-link" onClick={() => onPageChange(lastPage)}>Last Page</button>
                    </li>
                </ul>

                <div
                    className="d-flex align-items-center ms-3 flex-nowrap"
                    style={{ whiteSpace: "nowrap", minWidth: "160px" }}
                >
                    <label htmlFor="rowsPerPage" className="me-2 mb-0">
                        Items per page:
                    </label>
                    <select
                        id="rowsPerPage"
                        className="form-control form-control-sm text-center"
                        value={rowsPerPage}
                        onChange={handleRowsChange}
                    >
                        <option value={5}>5</option>
                        <option value={10}>10</option>
                        <option value={20}>20</option>
                    </select>
                </div>
            </div>
        </nav>
    );
}
