import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import { ENTITIES } from '../../utils/const.js';
import ApiUsersListTable from "../../components/tables/ApiUsersListTable.jsx";
import ListHeader from "../../components/common/ListHeader";
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Pagination from '../../components/common/pagination/Pagination';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";
import useRowsPerPage from '../../hooks/useRowsPerPage';

export default function ApiUsersList() {

    const [triggerFetch, setTriggerFetch] = useState(false);
    const [searchParams, setSearchParams] = useState({});
    const [pageNumber, setPageNumber] = useState(1);
    const [apiUsers, setApiUsers] = useState([]);
    const [totalPages, setTotalPages] = useState(1);
    const [hasFetched, setHasFetched] = useState(false);
    const { rowsPerPage, handleRowsPerPageChange } = useRowsPerPage(setPageNumber);
    const { data, error, isLoading, isSearchNoResult, isError } = useFetch(ENTITIES.API_USERS, null, pageNumber, rowsPerPage, triggerFetch, searchParams)

    useEffect(() => {
        if (data) {
            if (Array.isArray(data)) {
                setApiUsers(data);
            } else if (data.data) {
                setApiUsers(data.data);
                setPageNumber(data.pageNumber);
                setTotalPages(data.totalPages);
            } else {
                setApiUsers([]);
            }
            setTriggerFetch(false);
            setHasFetched(true);
        }
    }, [data]);

    useEffect(() => {
        setTriggerFetch(true);
    }, [rowsPerPage, pageNumber]);

    function handlePageChange(newPageNumber) {
        setPageNumber(newPageNumber);
    }

    return (
        <>
            <ListHeader
                dataType={ENTITIES.API_USERS}
                createButtonTitle="Create Airline"
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
                        {apiUsers && apiUsers.length > 0 ? (
                            <>
                                <ApiUsersListTable apiUsers={apiUsers} />
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
                                    : <CustomAlert alertType='info' type='Info' message='No api users available' />
                            )
                        )}
                    </>
                )}
            </Box>
        </>
    );
}