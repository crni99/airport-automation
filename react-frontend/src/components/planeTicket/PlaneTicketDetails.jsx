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
import openDetails from "../../utils/openDetailsHelper.js";
import { Entities } from '../../utils/const.js';
import { DD } from '..//common/table/DD.jsx';
import { DT } from '..//common/table/DT.jsx';

export default function PlaneTicketDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: planeTicket, dataExist, error, isLoading } = useFetch(Entities.PLANE_TICKETS, id);
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
                operationResult = await editData(Entities.PLANE_TICKETS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(Entities.PLANE_TICKETS, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Plane Ticket Details' />
            {(isLoading || operationState.isPending) && <LoadingSpinner />}
            {error && <Alert alertType="error" alertText={error.message} />}
            {operationState.operationError && <Alert alertType="error" alertText={operationState.operationError} />}
            {dataExist && (
                <>
                    <div>
                        <br />
                        <dl className="row">
                            <DT className="col-sm-2">Id</DT>
                            <DD className="col-sm-10">{planeTicket.id}</DD>
                            <DT className="col-sm-2">Price</DT>
                            <DD className="col-sm-10">{planeTicket.price}</DD>
                            <DT className="col-sm-2">Purchase Date</DT>
                            <DD className="col-sm-10">{planeTicket.purchaseDate}</DD>
                            <DT className="col-sm-2">Seat Number</DT>
                            <DD className="col-sm-10">{planeTicket.seatNumber}</DD>
                        </dl>
                        <hr></hr>
                        <dl className="row">
                            <DT className="col-sm-2">Passenger Id</DT>
                            <DD className="col-sm-10">
                                <button
                                    className="btn btn-info btn-sm"
                                    onClick={() => openDetails('Passengers', planeTicket.passenger.id)}
                                >
                                    {planeTicket.passenger.id}
                                </button>
                            </DD>
                            <DT className="col-sm-2">First Name</DT>
                            <DD className="col-sm-10">{planeTicket.passenger.firstName}</DD>
                            <DT className="col-sm-2">Last Name</DT>
                            <DD className="col-sm-10">{planeTicket.passenger.lastName}</DD>
                        </dl>
                        <hr></hr>
                        <dl className="row">
                            <DT className="col-sm-2">Travel Class Id</DT>
                            <DD className="col-sm-10">
                                <button
                                    className="btn btn-info btn-sm"
                                    onClick={() => openDetails('TravelClasses', null)}
                                >
                                    {planeTicket.travelClass.id}
                                </button>
                            </DD>
                            <DT className="col-sm-2">Type</DT>
                            <DD className="col-sm-10">{planeTicket.travelClass.type}</DD>
                        </dl>
                        <hr></hr>
                        <dl className="row">
                            <DT className="col-sm-2">Id</DT>
                            <DD className="col-sm-10">
                                <button
                                    className="btn btn-info btn-sm"
                                    onClick={() => openDetails('Flights', planeTicket.flight.id)}
                                >
                                    {planeTicket.flight.id}
                                </button>
                            </DD>
                            <DT className="col-sm-2">Departure Date</DT>
                            <DD className="col-sm-10">{planeTicket.flight.departureDate}</DD>
                            <DT className="col-sm-2">Departure Time</DT>
                            <DD className="col-sm-10">{planeTicket.flight.departureTime}</DD>
                        </dl>
                    </div>
                    <PageNavigationActions dataType={Entities.PLANE_TICKETS} dataId={id} onEdit={() => handleOperation('edit')}
                        onDelete={() => handleOperation('delete')} />
                </>
            )}
        </>
    );
}
