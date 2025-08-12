import React from 'react';
import EditAction from './EditAction';
import DeleteAction from './DeleteAction';
import BackToListAction from './BackToListAction';
import { getRole } from "../../../utils/auth";

const isUser = getRole();

export default function PageNavigationActions({ dataType, dataId, onEdit, onDelete }) {

    return (
        <div>
            {isUser !== 'User' && (
                <nav aria-label="Page navigation">
                    <ul className="pagination pagination-container">
                        <EditAction dataType={dataType} dataId={dataId} onEdit={onEdit} />
                        <DeleteAction dataType={dataType} dataId={dataId} onDelete={onDelete} />
                        <BackToListAction dataType={dataType} />
                    </ul>
                </nav>
            )}
        </div>
    );
}
