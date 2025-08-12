import React, { useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import useFetch from '../../hooks/useFetch';
import { deleteData } from '../../utils/delete';
import { editData } from '../../utils/edit.js';
import PageTitle from '../common/PageTitle.jsx';
import LoadingSpinner from '../common/LoadingSpinner';
import PageNavigationActions from '../common/pagination/PageNavigationActions';
import Alert from '../common/Alert';
import { useContext } from 'react';
import { DataContext } from '../../store/data-context';
import { Entities } from '../../utils/const.js';
import { DD } from '..//common/table/DD.jsx';
import { DT } from '..//common/table/DT.jsx';

export default function AirlineDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: airline, dataExist, error, isLoading } = useFetch(Entities.AIRLINES, id);
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
                operationResult = await editData(Entities.AIRLINES, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(Entities.AIRLINES, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Airline Details' />
            {(isLoading || operationState.isPending) && <LoadingSpinner />}
            {error && <Alert alertType="error" alertText={error.message} />}
            {operationState.operationError && <Alert alertType="error" alertText={operationState.operationError} />}
            {dataExist && (
                <>
                    <div>
                        <br />
                        <dl className="row">
                            <DT>Id</DT>
                            <DD>{airline.id}</DD>
                            <DT>Name</DT>
                            <DD>{airline.name}</DD>
                        </dl>
                    </div>
                    <PageNavigationActions dataType={Entities.AIRLINES} dataId={id} onEdit={() => handleOperation('edit')}
                        onDelete={() => handleOperation('delete')} />
                </>
            )}
        </>
    );
}
