import React, { useState, useEffect } from "react";
import useFetch from '../../hooks/useFetch.jsx';
import TravelClassesListTable from "../../components/tables/TravelClassesListTable.jsx";
import ListHeader from "../../components/common/ListHeader.jsx";
import { ENTITIES } from '../../utils/const.js';
import { Box } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import CustomAlert from "../../components/common/feedback/CustomAlert.jsx";

export default function TravelClassesList() {
    const [travelClasses, settravelClasses] = useState([]);
    const { data, dataExist, error, isLoading, isError } = useFetch(ENTITIES.TRAVEL_CLASSES, null, 1);

    useEffect(() => {
        if (data) {
            settravelClasses(data.data);
        }
    }, [data]);

    return (
        <>
            <ListHeader
                dataExist={dataExist}
                dataType={ENTITIES.TRAVEL_CLASSES}
            />

            <Box sx={{ mt: 2 }}>
                {isLoading && <CircularProgress sx={{ mb: 2 }} />}

                {isError && error && (
                    <CustomAlert alertType='error' type={error.type} message={error.message} />
                )}

                {!isError && !isLoading && (
                    <>
                        {travelClasses && travelClasses.length > 0 ? (
                            <>
                                <TravelClassesListTable travelClasses={travelClasses} />
                            </>
                        ) : (
                            <CustomAlert alertType='info' type='Info' message='No travel classes available' />
                        )}
                    </>
                )}
            </Box>
        </>
    );
}