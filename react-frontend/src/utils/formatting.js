export const currencyFormatter = new Intl.NumberFormat('en-US', {
    style: 'currency',
    currency: 'EUR'
});

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