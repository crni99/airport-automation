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
import openDetails from "../../utils/openDetailsHelper";
import { Entities } from '../../utils/const.js';
import openMap from '../../utils/openMapHelper.js';
import { DD } from '..//common/table/DD.jsx';
import { DT } from '..//common/table/DT.jsx';

export default function FlightDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: flight, dataExist, error, isLoading } = useFetch(Entities.FLIGHTS, id);
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
                operationResult = await editData(Entities.FLIGHTS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(Entities.FLIGHTS, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Flight Details' />
            {(isLoading || operationState.isPending) && <LoadingSpinner />}
            {error && <Alert alertType="error" alertText={error.message} />}
            {operationState.operationError && <Alert alertType="error" alertText={operationState.operationError} />}
            {dataExist && (
                <>
                    <div>
                        <br />
                        <dl className="row">
                            <DT className="col-sm-2">Id</DT>
                            <DD className="col-sm-10">{flight.id}</DD>
                            <DT className="col-sm-2">Departure Date</DT>
                            <DD className="col-sm-10">{flight.departureDate}</DD>
                            <DT className="col-sm-2">Departure Time</DT>
                            <DD className="col-sm-10">{flight.departureTime}</DD>
                        </dl>
                        <hr></hr>
                        <dl className="row">
                            <DT className="col-sm-2">Airline Id</DT>
                            <DD className="col-sm-10">
                                <button
                                    className="btn btn-info btn-sm"
                                    onClick={() => openDetails('Airlines', flight.airline.id)}
                                >
                                    {flight.airline.id}
                                </button>
                            </DD>
                            <DT className="col-sm-2">Name</DT>
                            <DD className="col-sm-10">{flight.airline.name}</DD>
                        </dl>
                        <hr></hr>
                        <dl className="row">
                            <DT className="col-sm-2">Destination Id</DT>
                            <DD className="col-sm-10">
                                <button
                                    className="btn btn-info btn-sm"
                                    onClick={() => openDetails('Destinations', flight.destination.id)}
                                >
                                    {flight.destination.id}
                                </button>
                            </DD>
                            <DT className="col-sm-2">City</DT>
                            <DD className="col-sm-10 clickable-row link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover"
                                onClick={() => openMap(flight.destination.city)}>{flight.destination.city}
                            </DD>
                            <DT className="col-sm-2">Airport</DT>
                            <DD className="col-sm-10 clickable-row link-primary link-offset-2 link-underline-opacity-25 link-underline-opacity-100-hover"
                                onClick={() => openMap(flight.destination.airport)}>{flight.destination.airport}
                            </DD>
                        </dl>
                        <hr></hr>
                        <dl className="row">
                            <DT className="col-sm-2">Pilot Id</DT>
                            <DD className="col-sm-10">
                                <button
                                    className="btn btn-info btn-sm"
                                    onClick={() => openDetails('Pilots', flight.pilot.id)}
                                >
                                    {flight.pilot.id}
                                </button>
                            </DD>
                            <DT className="col-sm-2">First Name</DT>
                            <DD className="col-sm-10">{flight.pilot.firstName}</DD>
                            <DT className="col-sm-2">Last Name</DT>
                            <DD className="col-sm-10">{flight.pilot.lastName}</DD>
                            <DT className="col-sm-2">UPRN</DT>
                            <DD className="col-sm-10">{flight.pilot.uprn}</DD>
                            <DT className="col-sm-2">Flying Hours</DT>
                            <DT className="col-sm-10">{flight.pilot.flyingHours}</DT>
                        </dl>
                    </div>
                    <PageNavigationActions dataType={Entities.FLIGHTS} dataId={id} onEdit={() => handleOperation('edit')}
                        onDelete={() => handleOperation('delete')} />
                </>
            )}
        </>
    );
}
