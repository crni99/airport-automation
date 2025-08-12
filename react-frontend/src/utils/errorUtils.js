export const handleNetworkError = (error) => {
    if (error instanceof TypeError && error.message === 'Failed to fetch') {
        return {
            type: 'Network Error',
            message: `We've run into a server error. Some functions might not work right now but you can continue to use the app.`
        };
    }
    return null;
};

export const errorMessageMap = {
    400: 'Bad request: The server cannot process the request due to a client error.',
    401: 'Unauthorized: Authentication is required and has failed or has not yet been provided.',
    403: 'Forbidden: The server understood the request but refuses to authorize it.',
    404: (dataType, dataId) => `Not found: The requested data with id ${dataId} does not exist.`,
    405: 'Method not allowed: The request method is not supported for the requested resource.',
    409: (dataType, dataId) => `Conflict: The requested operation cannot be completed due to conflicts with existing data (ID: ${dataId}).`,
    429: 'Too many requests: The user has sent too many requests in a given amount of time.',
    500: 'Internal server error: The server encountered an unexpected condition that prevented it from fulfilling the request.',
    502: 'Bad gateway: The server received an invalid response from the upstream server.',
    503: 'Service unavailable: The server is currently unable to handle the request due to temporary overloading or maintenance of the server.',
    504: 'Gateway timeout: The server did not receive a timely response from the upstream server.',
    default: (dataType) => `Failed to ${dataType}: Server returned an unexpected status.`,
};

export const generateErrorMessage = (response, dataType = '', dataId = null) => {
    const errorMessageGenerator = errorMessageMap[response.status] || errorMessageMap.default;
    if (typeof errorMessageGenerator === 'function') {
        return errorMessageGenerator(dataType, dataId);
    } else if (errorMessageGenerator !== undefined) {
        console.log(errorMessageGenerator);
        return errorMessageGenerator;
    } else {
        return `Failed to ${dataType}: Server returned an unexpected status.`;
    }
};


