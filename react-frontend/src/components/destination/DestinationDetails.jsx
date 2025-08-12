import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch.jsx';
import { deleteData } from '../../utils/delete.js';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import LoadingSpinner from '../common/LoadingSpinner.jsx';
import PageNavigationActions from '../common/pagination/PageNavigationActions.jsx';
import Alert from '../common/Alert.jsx';
import { useContext } from 'react';
import { DataContext } from '../../store/data-context.jsx';
import openMap from '../../utils/openMapHelper.js'
import { Entities } from '../../utils/const.js';
import MapEmbed from '../common/MapEmbed.jsx';
import { DD } from '..//common/table/DD.jsx';
import { DT } from '..//common/table/DT.jsx';

export default function DestinationDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: destination, dataExist, error, isLoading } = useFetch(Entities.DESTINATIONS, id);
    const navigate = useNavigate();

    const [operationState, setOperationState] = useState({
        operationError: null,
        isPending: false
    });

    const handleOperation = async (operation) => {
        try {
            setOperationState(prevState => ({ ...prevState, isPending: true }));
            let operationResult;

            if (operation === 'edit') {
                operationResult = await editData(Entities.DESTINATIONS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(Entities.DESTINATIONS, id, dataCtx.apiUrl, navigate);
            }
            if (operationResult) {
                setOperationState(prevState => ({ ...prevState, operationError: operationResult.message }));
            }
        } catch (error) {
            setOperationState(prevState => ({ ...prevState, operationError: error.message }));
        } finally {
            setOperationState(prevState => ({ ...prevState, isPending: false }));
        }
    };

    return (
        <>
            <PageTitle title='Destination Details' />
            {(isLoading || operationState.isPending) && <LoadingSpinner />}
            {error && <Alert alertType="error" alertText={error.message} />}
            {operationState.operationError && <Alert alertType="error" alertText={operationState.operationError} />}
            {dataExist && (
                <>
                    <div className="container">
                        <div className="row">
                            <div className="col-md-6">
                                <br />
                                <dl className="row">
                                    <DT className="col-sm-2 mt-2">Id</DT>
                                    <DD className="col-sm-10 mt-2">{destination.id}</DD>
                                    <DT className="col-sm-2 mt-2">City</DT>
                                    <DD className="col-sm-10  mt-2 clickable-row link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover"
                                        onClick={() => openMap(destination.city)}>
                                        {destination.city}
                                    </DD>
                                    <DT className="col-sm-2 mt-2 ">Airport</DT>
                                    <DD className="col-sm-10  mt-2 clickable-row link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover"
                                        onClick={() => openMap(destination.airport)}>
                                        {destination.airport}
                                    </DD>
                                </dl>
                                <PageNavigationActions dataType={Entities.DESTINATIONS} dataId={id} onEdit={() => handleOperation('edit')}
                                    onDelete={() => handleOperation('delete')} />
                            </div>
                            <div className="col-md-6">
                                <MapEmbed address={`${destination.city} ${destination.airport}`} />
                            </div>
                        </div>
                    </div>
                </>
            )}
        </>
    );
}
