import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch.jsx';
import ListHeader from "../../components/common/ListHeader.jsx";
import PilotsListTable from "../../components/tables/PilotsListTable.jsx";
import { ENTITIES } from '../../utils/const.js';
import { Box } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";
import Pagination from '../../components/common/pagination/Pagination.jsx'
import useRowsPerPage from '../../hooks/useRowsPerPage';

export default function PilotsList() {

    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [pilots, setPilots] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [searchParams, setSearchParams] = useState({});
    const [hasFetched, setHasFetched] = useState(false);
    const { rowsPerPage, handleRowsPerPageChange } = useRowsPerPage(setPageNumber);
    const { data, error, isLoading, isSearchNoResult, isError } = useFetch(ENTITIES.PILOTS, null, pageNumber, rowsPerPage, triggerFetch, searchParams)

    useEffect(() => {
        if (data) {
            if (Array.isArray(data)) {
                setPilots(data);
            } else if (data.data) {
                setPilots(data.data);
                setPageNumber(data.pageNumber);
                setTotalPages(data.totalPages);
            } else {
                setPilots([]);
            }
            setTriggerFetch(false);
            setHasFetched(true);
        }
    }, [data]);

    useEffect(() => {
        setTriggerFetch(true);
    }, [rowsPerPage, pageNumber]);

    const handlePageChange = (event, newPage) => {
        setPageNumber(newPage + 1);
    };

    return (
        <>
            <ListHeader
                dataType={ENTITIES.PILOTS}
                createButtonTitle="Create Pilot"
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
                            hasFetched && (
                                isSearchNoResult
                                    ? <CustomAlert alertType='info' type='Info' message='No results found for your search.' />
                                    : <CustomAlert alertType='info' type='Info' message='No pilots available' />
                            )
                        )}
                    </>
                )}
            </Box>
        </>
    );
}