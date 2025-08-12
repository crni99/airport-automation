import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import { Pagination } from '../common/pagination/Pagination';
import LoadingSpinner from '../common/LoadingSpinner';
import Alert from '../common/Alert';
import ListHeader from "../common/ListHeader";
import PassengersListTable from "./PassengersListTable";
import { Entities } from '../../utils/const.js';
import PageInfo from "../common/pagination/PageInfo.jsx";

export default function PassengersList() {
    const [pageNumber, setPageNumber] = useState(1);
    const [totalPages, setTotalPages] = useState(1);
    const [passengers, setPassengers] = useState([]);
    const [triggerFetch, setTriggerFetch] = useState(false);
    const [rowsPerPage, setRowsPerPage] = useState(() => {
        const saved = localStorage.getItem("rowsPerPage");
        return saved ? Number(saved) : 10;
    });
    const { data, dataExist, error, isLoading, isError } = useFetch(
        Entities.PASSENGERS,
        null,
        pageNumber,
        triggerFetch,
        rowsPerPage
    );

    useEffect(() => {
        if (data) {
            setPassengers(data.data);
            setPageNumber(data.pageNumber);
            setTotalPages(data.totalPages);
            setTriggerFetch(false);
        }
    }, [data]);

    function handlePageChange(newPageNumber) {
        setPageNumber(newPageNumber);
    }

    useEffect(() => {
        setTriggerFetch(true);
    }, [rowsPerPage]);

    return (
        <>
            <ListHeader dataExist={dataExist} dataType={Entities.PASSENGERS} createButtonTitle="Create Passenger" setTriggerFetch={setTriggerFetch} />
            <br />
            {isLoading && <LoadingSpinner />}
            {isError && error && (
                <Alert alertType="error">
                    <strong>{error.type}</strong>: {error.message}
                </Alert>
            )}
            {!isError && !isLoading && (
                <div className="form-horizontal">
                    <div className="form-group">
                        {passengers && passengers.length > 0 ? (
                            <PassengersListTable passengers={passengers} />
                        ) : (
                            <Alert alertType="info" alertText="No passengers available" />
                        )}
                        <PageInfo
                            currentPage={pageNumber}
                            totalPages={totalPages}
                            totalCount={data?.totalCount ?? 0}
                        />
                        <div>
                            <Pagination
                                pageNumber={pageNumber}
                                lastPage={totalPages}
                                onPageChange={handlePageChange}
                                rowsPerPage={rowsPerPage}
                                onRowsPerPageChange={setRowsPerPage}
                            />
                        </div>
                    </div>
                </div>
            )}
        </>
    );
}
