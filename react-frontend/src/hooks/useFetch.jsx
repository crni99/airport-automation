import { useState, useEffect, useCallback } from 'react';
import { useContext } from 'react';
import { DataContext } from '../store/DataContext.jsx';
import { fetchData } from '../utils/httpFetch.js'; 
import { handleNetworkError } from '../utils/errorUtils.js';

export default function useFetch(dataType, dataId, page = 1, triggerFetch, rowsPerPage) {
    const dataCtx = useContext(DataContext);
    const [data, setData] = useState(null);
    const [dataExist, setDataExist] = useState(false);
    const [error, setError] = useState(null);
    const [isLoading, setIsLoading] = useState(true);
    const [isError, setIsError] = useState(false);

    const handleFetchError = useCallback((error) => {
        const networkError = handleNetworkError(error);
        
        let errorData = {};
        if (networkError) {
            errorData = {
                type: networkError.type || 'Network Error',
                message: networkError.message || 'A network issue occurred.'
            };
        } else {
            errorData = {
                type: error.type || error.name || 'Fetch Error',
                message: error.message || 'An unknown error occurred.'
            };
        }
        
        setError(errorData);
        setIsError(true);
    }, []);

    useEffect(() => {

        if (!triggerFetch) {
            setIsLoading(false); 
            return;
        }

        const controller = new AbortController();
        const signal = controller.signal;

        async function doFetch() {
            setIsLoading(true);
            setError(null);
            setIsError(false);
            
            try {
                const result = await fetchData(
                    dataType, 
                    dataId, 
                    dataCtx.apiUrl, 
                    page, 
                    rowsPerPage, 
                    signal
                );

                if (!signal.aborted) {
                    setData(result.data);
                    setDataExist(result.dataExist);
                }
            } catch (error) {
                if (error.name !== 'AbortError') {
                    handleFetchError(error);
                }
            } finally {
                if (!signal.aborted) {
                    setIsLoading(false);
                }
            }
        }
        
        doFetch();

        return () => {
            controller.abort();
        };
    }, [dataType, dataId, page, dataCtx.apiUrl, triggerFetch, rowsPerPage, handleFetchError]); 

    return { data, dataExist, error, isLoading, isError };
}