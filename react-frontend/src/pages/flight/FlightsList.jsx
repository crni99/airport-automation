import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import Pagination from '../../components/common/pagination/Pagination';
import ListHeader from "../../components/common/ListHeader";
import FlightsListTable from "../../components/tables/FlightsListTable.jsx";
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";

export default function FlightsList() {
    
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [flights, setFlights] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [searchParams, setSearchParams] = useState({});
    const [hasFetched, setHasFetched] = useState(false);
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem("rowsPerPage");
        return saved ? Number(saved) : 10;
    });
    const { data, dataExist, error, isLoading, isError } = useFetch(ENTITIES.FLIGHTS, null, pageNumber, rowsPerPage, triggerFetch, searchParams)

    useEffect(() => {
        if (data) {
            if (Array.isArray(data)) {
                setFlights(data);
            } else if (data.data) {
                setFlights(data.data);
                setPageNumber(data.pageNumber);
                setTotalPages(data.totalPages);
            } else {
                setFlights([]);
            }
            setTriggerFetch(false);
            setHasFetched(true);
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
                dataType={ENTITIES.FLIGHTS}
                createButtonTitle="Create Flight"
                setTriggerFetch={setTriggerFetch}
                setSearchParams={setSearchParams}
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
                            hasFetched && <CustomAlert alertType='info' type='Info' message='No flights available' />
                        )}
                    </>
                )}
            </Box>
        </>
    );
}