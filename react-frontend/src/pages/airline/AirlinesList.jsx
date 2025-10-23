import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import { ENTITIES } from '../../utils/const.js';
import ListHeader from "../../components/common/ListHeader";
import AirlinesListTable from "../../components/tables/AirlinesListTable.jsx";
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";
import { Box } from '@mui/material';
import Pagination from '../../components/common/pagination/Pagination.jsx'
import CircularProgress from '@mui/material/CircularProgress';

export default function AirlineList() {
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [airlines, setAirlines] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem("rowsPerPage");
        return saved ? Number(saved) : 10;
    });

    const { data, dataExist, error, isLoading, isError } = useFetch(ENTITIES.AIRLINES, null, pageNumber, rowsPerPage, triggerFetch)

    useEffect(() => {
        if (data) {
            if (Array.isArray(data)) {
                setAirlines(data);
            } else if (data.data) {
                setAirlines(data.data);
                setPageNumber(data.pageNumber);
                setTotalPages(data.totalPages);
            } else {
                setAirlines([]);
            }
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
                dataType={ENTITIES.AIRLINES}
                createButtonTitle="Create Airline"
                setTriggerFetch={setTriggerFetch}
            />

            <Box sx={{ mt: 5 }}>
                {isLoading && <CircularProgress sx={{ mb: 2 }} />}

                {isError && error && (
                    <CustomAlert alertType='error' type={error.type} message={error.message} />
                )}

                {!isError && !isLoading && (
                    <>
                        {airlines && airlines.length > 0 ? (
                            <>
                                <AirlinesListTable airlines={airlines} />
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
                            <CustomAlert alertType='info' type='Info' message='No airlines available' />
                        )}
                    </>
                )}
            </Box>
        </>
    );
}