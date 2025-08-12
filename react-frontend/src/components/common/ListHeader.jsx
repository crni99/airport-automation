import React from 'react';
import CreateButton from "../common/CreateButton";
import SearchInputWithButton from "../common/SearchInputWithButton";
import { getRole } from "../../utils/auth";
import { Entities } from '../../utils/const.js';

export default function ListHeader({ dataExist, dataType, createButtonTitle, setTriggerFetch }) {

    const isUser = getRole();

    return (
        <div className="container container-spacing-top">
            <div className="row justify-content-between">
                <div className="col-md-12">
                    <div className="d-flex justify-content-between mt-2 mb-2" style={{ width: '100%' }}>
                        {isUser !== 'User' && dataType !== Entities.API_USERS && (
                            <CreateButton destination={`/${dataType}/Create`} title={createButtonTitle} />
                        )}
                        {dataExist && (
                            <div className="d-flex">
                                <SearchInputWithButton type={dataType} setTriggerFetch={setTriggerFetch} />
                            </div>
                        )}
                    </div>
                </div>
            </div>
        </div>
    )
}