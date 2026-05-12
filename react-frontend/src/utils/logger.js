const isDevelopment = process.env.NODE_ENV === 'development';

const logger = {
    log: (...args) => {
        if (isDevelopment) console.log(...args);
    },
    warn: (...args) => {
        if (isDevelopment) console.warn(...args);
    },
    error: (...args) => {
        if (isDevelopment) console.error(...args);
    },
};

export default logger;