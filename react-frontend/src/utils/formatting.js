export const currencyFormatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'USD'
});

/**
 * Format the time string to the "HH:mm:ss" format.
 * If the time is in "HH:mm" format, it will append ":00" for seconds.
 * If the time is in "HH:mm:ss.sss" format, it will remove milliseconds.
 * @param {string} time - The time string to be formatted.
 * @returns {string} - The formatted time string in "HH:mm:ss" format.
 */
export const formatTime = (time) => {
    const timeParts = time.split(":");
    if (timeParts.length === 2) {
        return `${timeParts[0]}:${timeParts[1]}:00`;
    }
    if (timeParts.length === 3) {
        return `${timeParts[0]}:${timeParts[1]}:${timeParts[2].split('.')[0]}`;
    }
    return time;
};