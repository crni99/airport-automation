import { useState } from 'react';

const STORAGE_KEY = 'rowsPerPage';
const DEFAULT_ROWS = 10;

export default function useRowsPerPage(setPageNumber) {
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem(STORAGE_KEY);
        return saved ? Number(saved) : DEFAULT_ROWS;
    });

    const handleRowsPerPageChange = (event) => {
        const newRowsPerPage = parseInt(event.target.value, 10);
        localStorage.setItem(STORAGE_KEY, newRowsPerPage);
        setRowsPerPage(newRowsPerPage);
        if (setPageNumber) setPageNumber(1);
    };

    return { rowsPerPage, handleRowsPerPageChange };
}