import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import { ENTITIES } from '../../utils/const.js';
import ApiUsersListTable from "../../components/tables/ApiUsersListTable.jsx";
import ListHeader from "../../components/common/ListHeader";
import Box from '@mui/material/Box';
import CircularProgress from '@mui/material/CircularProgress';
import Pagination from '../../components/common/pagination/Pagination';
import CustomAlert from "../../components/common/Alert.jsx";

export default function ApiUsersList() {
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [pageNumber, setPageNumber] = useState(1);
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem("rowsPerPage");
        return saved ? Number(saved) : 10;
    });

    const { data, dataExist, error, isLoading, isError } = useFetch(
        ENTITIES.API_USERS,
        null,
        pageNumber,
        triggerFetch,
        rowsPerPage
    );

    const [apiUsers, setApiUsers] = useState([]);
    const [totalPages, setTotalPages] = useState(1);

    useEffect(() => {
        if (data) {
            setApiUsers(data.data);
            setPageNumber(data.pageNumber);
            setTotalPages(data.totalPages);
            setTriggerFetch(false);
        }
    }, [data]);

    useEffect(() => {
        setTriggerFetch(true);
    }, [rowsPerPage, pageNumber]);

    function handlePageChange(newPageNumber) {
        setPageNumber(newPageNumber);
    }

    function handleRowsPerPageChange(newRowsPerPage) {
        localStorage.setItem("rowsPerPage", newRowsPerPage);
        setRowsPerPage(newRowsPerPage);
        setPageNumber(1);
    }

    return (
        <>
            <ListHeader
                dataExist={dataExist}
                dataType={ENTITIES.API_USERS}
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
                            <CustomAlert alertType='info' type='Info' message='No api users available' />
                        )}
                    </>
                )}
            </Box>
        </>
    );
}