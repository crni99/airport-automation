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
import { Entities } from '../../utils/const.js';
import { DD } from '..//common/table/DD.jsx';
import { DT } from '..//common/table/DT.jsx';

export default function ApiUserDetails() {
    const dataCtx = useContext(DataContext);
    const { id } = useParams();
    const { data: apiUser, dataExist, error, isLoading } = useFetch(Entities.API_USERS, id);
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
                operationResult = await editData(Entities.API_USERS, id, dataCtx.apiUrl, navigate);
            } else if (operation === 'delete') {
                operationResult = await deleteData(Entities.API_USERS, id, dataCtx.apiUrl, navigate);
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
            <PageTitle title='Api User Details' />
            {(isLoading || operationState.isPending) && <LoadingSpinner />}
            {error && <Alert alertType="error" alertText={error.message} />}
            {operationState.operationError && <Alert alertType="error" alertText={operationState.operationError} />}
            {dataExist && (
                <>
                    <div>
                        <br />
                        <dl className="row">
                            <DT className="col-sm-2 mt-2">Id</DT>
                            <DD className="col-sm-10 mt-2">{apiUser.apiUserId}</DD>
                            <DT className="col-sm-2 mt-2">Username</DT>
                            <DD className="col-sm-10 mt-2">{apiUser.userName}</DD>
                            <DT className="col-sm-2 mt-2">Password</DT>
                            <DD className="col-sm-10 mt-2">{apiUser.password}</DD>
                            <DT className="col-sm-2 mt-2">Roles</DT>
                            <DD className="col-sm-10 mt-2">{apiUser.roles}</DD>
                        </dl>
                    </div>
                    <PageNavigationActions dataType={Entities.API_USERS} dataId={id} onEdit={() => handleOperation('edit')}
                        onDelete={() => handleOperation('delete')} />
                </>
            )}
        </>
    );
}
