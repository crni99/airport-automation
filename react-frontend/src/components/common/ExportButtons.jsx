import React from 'react';
import { useExport } from '../../hooks/useExport';

export default function CreateButton({ dataType }) {
    const { triggerExport } = useExport();

    return (
        <div className="d-flex" data-entity={dataType}>
            <button
                id="exportButtonPDF"
                className="btn btn-danger me-3"
                data-type="PDF"
                onClick={() => triggerExport(dataType, 'pdf')}
            >
                <i className="fas fa-file-pdf text-white"></i>
            </button>
            <button
                id="exportButtonEXCEL"
                className="btn btn-success"
                data-type="EXCEL"
                onClick={() => triggerExport(dataType, 'excel')}
            >
                <i className="fas fa-file-excel text-white"></i>
            </button>
        </div>
    );
}
