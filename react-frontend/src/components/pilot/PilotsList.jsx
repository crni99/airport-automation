import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import ListHeader from "../common/ListHeader";
import PilotsListTable from "./PilotsListTable";
import { ENTITIES } from '../../utils/const.js';
import { Box } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import CustomAlert from "../common/Alert.jsx";
import Pagination from '../common/pagination/Pagination.jsx'

export default function PilotsList() {
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [pilots, setPilots] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem("rowsPerPage");
        return saved ? Number(saved) : 10;
    });
    const { data, dataExist, error, isLoading, isError } = useFetch(
        ENTITIES.PILOTS,
        null,
        pageNumber,
        triggerFetch,
        rowsPerPage
    );

    useEffect(() => {
        if (data) {
            setPilots(data.data);
            setPageNumber(data.pageNumber);
            setTotalPages(data.totalPages);
            setTriggerFetch(false);
        }
    }, [data]);

    useEffect(() => {
        setTriggerFetch(true);
    }, [rowsPerPage, pageNumber]);

    const handlePageChange = (event, newPage) => {
        setPageNumber(newPage + 1);
    };

    const handleRowsPerPageChange = (event) => {
        setRowsPerPage(parseInt(event.target.value, 10));
        setPageNumber(1);
    };

    return (
        <>
            <ListHeader
                dataExist={dataExist}
                dataType={ENTITIES.PILOTS}
                createButtonTitle="Create Pilot"
                setTriggerFetch={setTriggerFetch}
            />

            <Box sx={{ mt: 4 }}>
                {isLoading && <CircularProgress sx={{ mb: 2 }} />}

                {isError && error && (
                    <CustomAlert alertType='error' type={error.type} message={error.message} />
                )}

                {!isError && !isLoading && (
                    <>
                        {pilots && pilots.length > 0 ? (
                            <>
                                <PilotsListTable pilots={pilots} />
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
                            <CustomAlert alertType='info' type='Info' message='No pilots available' />
                        )}
                    </>
                )}
            </Box>
        </>
    );
}