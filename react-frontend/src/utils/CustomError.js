export class CustomAPIError extends Error {
    constructor(type, message) {
        super(message);
        this.name = 'CustomAPIError';
        this.type = type;
        if (Error.captureStackTrace) {
            Error.captureStackTrace(this, CustomAPIError);
        }
    }
}