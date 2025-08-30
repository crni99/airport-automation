import React from 'react';
import { useExport } from '../../hooks/useExport';
import LoadingSpinner from './LoadingSpinner'

export default function CreateButton({ dataType }) {
    const { triggerExport, isLoading } = useExport();

    return (
        <div className="d-flex" data-entity={dataType}>
            <button
                id="exportButtonPDF"
                className="btn btn-danger me-3"
                data-type="PDF"
                onClick={() => triggerExport(dataType, 'pdf')}
                disabled={isLoading}
            >
                <i className="fas fa-file-pdf text-white"></i>
            </button>
            <button
                id="exportButtonEXCEL"
                className="btn btn-success"
                data-type="EXCEL"
                onClick={() => triggerExport(dataType, 'excel')}
                disabled={isLoading}
            >
                <i className="fas fa-file-excel text-white"></i>
            </button>

            {isLoading && (
                <div className='ms-3'>
                    <LoadingSpinner />
                </div>
            )}
        </div>
    );
}
