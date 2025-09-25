import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import Pagination from '../common/pagination/Pagination';
import ListHeader from "../common/ListHeader";
import FlightsListTable from "./FlightsListTable";
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import CustomAlert from "../common/Alert.jsx";

export default function FlightsList() {
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [flights, setFlights] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem("rowsPerPage");
        return saved ? Number(saved) : 10;
    });
    const { data, dataExist, error, isLoading, isError } = useFetch(
        ENTITIES.FLIGHTS,
        null,
        pageNumber,
        triggerFetch,
        rowsPerPage
    );

    useEffect(() => {
        if (data) {
            setFlights(data.data);
            setPageNumber(data.pageNumber);
            setTotalPages(data.totalPages);
            setTriggerFetch(false);
        }
    }, [data]);

    const handlePageChange = (event, newPageNumber) => {
        setPageNumber(newPageNumber + 1);
        setTriggerFetch(true);
    };

    const handleRowsPerPageChange = (event) => {
        const newRowsPerPage = parseInt(event.target.value, 10);
        setRowsPerPage(newRowsPerPage);
        setPageNumber(1);
        localStorage.setItem("rowsPerPage", newRowsPerPage);
    };

    useEffect(() => {
        setTriggerFetch(true);
    }, [rowsPerPage]);

    return (
        <>
            <ListHeader
                dataExist={dataExist}
                dataType={ENTITIES.FLIGHTS}
                createButtonTitle="Create Flight"
                setTriggerFetch={setTriggerFetch}
            />

            <Box sx={{ mt: 5 }}>
                {isLoading && <CircularProgress sx={{ mb: 2 }} />}

                {isError && error && (
                    <CustomAlert alertType='error' type={error.type} message={error.message} />
                )}

                {!isError && !isLoading && (
                    <>
                        {flights && flights.length > 0 ? (
                            <>
                                <FlightsListTable flights={flights} />
                                <Pagination
                                    data={data}
                                    pageNumber={pageNumber}
                                    totalPages={totalPages}
                                    rowsPerPage={rowsPerPage}
                                    handlePageChange={handlePageChange}
                                    handleRowsPerPageChange={handleRowsPerPageChange}
                                />
                            </>
                        ) : (
                            <CustomAlert alertType='info' type='Info' message='No flights available' />
                        )}
                    </>
                )}
            </Box>
        </>
    );
}