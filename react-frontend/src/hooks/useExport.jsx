import { useState, useContext } from 'react';
import { buildExportURL } from '../utils/export.js';
import { DataContext } from '../store/data-context.jsx';
import { getAuthToken } from '../utils/auth';

export function useExport() {

    const dataCtx = useContext(DataContext);
    const [isLoading, setLoading] = useState(false); 

    async function triggerExport(dataType, exportType) {
        if (!dataCtx || !dataCtx.apiUrl) {
            console.warn('API URL not set');
            return;
        }

        setLoading(true);

        const { baseUrl, params } = buildExportURL(dataCtx.apiUrl, dataType, exportType);

        const token = getAuthToken();
        if (!token) {
            console.warn('No auth token found');
            return;
        }

        const url = new URL(baseUrl);
        Object.entries(params).forEach(([key, val]) => url.searchParams.append(key, val));

        try {
            const response = await fetch(url.toString(), {
                headers: {
                    'Authorization': `Bearer ${token}`,
                },
            });

            if (!response.ok) {
                throw new Error(`Export failed: ${response.status} ${response.statusText}`);
            }

            const blob = await response.blob();

            let filename;
            const disposition = response.headers.get('content-disposition');
            if (disposition) {
                const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;
                const matches = filenameRegex.exec(disposition);
                if (matches != null && matches[1]) {
                    filename = matches[1].replace(/['"]/g, '');
                }
            }

            if (!filename) {
                const ext = exportType.toLowerCase() === 'pdf' ? 'pdf' : 'xlsx';
                const now = new Date();
                const dateTime = now.toISOString().replace(/[:.]/g, '-');
                filename = `${dataType}-${dateTime}.${ext}`;
            }

            const downloadUrl = window.URL.createObjectURL(blob);
            const a = document.createElement('a');
            a.href = downloadUrl;
            a.download = filename;
            document.body.appendChild(a);
            a.click();
            a.remove();
            window.URL.revokeObjectURL(downloadUrl);

        } catch (err) {
            console.error(err);
            alert('Export failed. Please try again.');
        } finally {
            setLoading(false);
        }
    }

    return { triggerExport, isLoading };
}
