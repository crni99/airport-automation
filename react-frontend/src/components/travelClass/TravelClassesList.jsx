import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch';
import LoadingSpinner from '../common/LoadingSpinner';
import Alert from '../common/Alert';
import TravelClassesListTable from "./TravelClassesListTable";
import { Entities } from '../../utils/const.js';

export default function TravelClassesList() {
    const [travelClasses, settravelClasses] = useState([]);
    const { data, error, isLoading, isError } = useFetch(Entities.TRAVEL_CLASSES, null, 1);

    useEffect(() => {
        if (data) {
            settravelClasses(data.data);
        }
    }, [data]);

    return (
        <>
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
                        {travelClasses && travelClasses.length > 0 ? (
                            <TravelClassesListTable travelClasses={travelClasses} />
                        ) : (
                            <Alert alertType="info" alertText="No travel classes available" />
                        )}
                    </div>
                </div>
            )}
        </>
    );
}
