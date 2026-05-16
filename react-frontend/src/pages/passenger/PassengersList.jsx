import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import Pagination from '../../components/common/pagination/Pagination';
import ListHeader from "../../components/common/ListHeader";
import PassengersListTable from "../../components/tables/PassengersListTable.jsx";
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";
import useRowsPerPage from '../../hooks/useRowsPerPage';

export default function PassengersList() {
    
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [passengers, setPassengers] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [searchParams, setSearchParams] = useState({});
    const [hasFetched, setHasFetched] = useState(false);
    const { rowsPerPage, handleRowsPerPageChange } = useRowsPerPage(setPageNumber);
    const { data, error, isLoading, isSearchNoResult, isError } = useFetch(ENTITIES.PASSENGERS, null, pageNumber, rowsPerPage, triggerFetch, searchParams)

    useEffect(() => {
        if (data) {
            if (Array.isArray(data)) {
                setPassengers(data);
            } else if (data.data) {
                setPassengers(data.data);
                setPageNumber(data.pageNumber);
                setTotalPages(data.totalPages);
            } else {
                setPassengers([]);
            }
            setTriggerFetch(false);
            setHasFetched(true);
        }
    }, [data]);

    useEffect(() => {
        setTriggerFetch(true);
    }, [rowsPerPage]);

    const handlePageChange = (event, newPageNumber) => {
        setPageNumber(newPageNumber + 1);
        setTriggerFetch(true);
    };

    return (
        <>
            <ListHeader
                dataType={ENTITIES.PASSENGERS}
                createButtonTitle="Create Passenger"
                setTriggerFetch={setTriggerFetch}
                setSearchParams={setSearchParams}
            />

            <Box sx={{ mt: 3, mb: -1 }}>
                {isLoading && <CircularProgress sx={{ mb: 2 }} />}

                {isError && error && (
                    <CustomAlert alertType='error' type={error.type} message={error.message} />
                )}

                {!isError && !isLoading && (
                    <>
                        {passengers && passengers.length > 0 ? (
                            <>
                                <PassengersListTable passengers={passengers} />
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
                            hasFetched && (
                                isSearchNoResult
                                    ? <CustomAlert alertType='info' type='Info' message='No results found for your search.' />
                                    : <CustomAlert alertType='info' type='Info' message='No passengers available' />
                            )
                        )}
                    </>
                )}
            </Box>
        </>
    );
}