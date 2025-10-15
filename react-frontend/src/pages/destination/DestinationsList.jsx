import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import { ENTITIES } from '../../utils/const.js';
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import ListHeader from "../../components/common/ListHeader";
import DestinationsListTable from "../../components/tables/DestinationsListTable.jsx";
import Pagination from '../../components/common/pagination/Pagination';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";

export default function DestinationsList() {
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [destinations, setDestinations] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem("rowsPerPage");
        return saved ? Number(saved) : 10;
    });
    const { data, dataExist, error, isLoading, isError } = useFetch(
        ENTITIES.DESTINATIONS,
        null,
        pageNumber,
        triggerFetch,
        rowsPerPage
    );

    useEffect(() => {
        if (data) {
            setDestinations(data.data);
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
                dataType={ENTITIES.DESTINATIONS}
                createButtonTitle="Create Destination"
                setTriggerFetch={setTriggerFetch}
            />

            <Box sx={{ mt: 5 }}>
                {isLoading && <CircularProgress sx={{ mb: 2 }} />}

                {isError && error && (
                    <CustomAlert alertType='error' type={error.type} message={error.message} />
                )}

                {!isError && !isLoading && (
                    <>
                        {destinations && destinations.length > 0 ? (
                            <>
                                <DestinationsListTable destinations={destinations} />
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
                            <CustomAlert alertType='info' type='Info' message='No destinations available' />
                        )}
                    </>
                )}
            </Box>
        </>
    );
}