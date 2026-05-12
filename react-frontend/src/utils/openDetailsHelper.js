// Intentionally uses window.open to open entity details in a new tab,
// allowing the user to keep the current list view open.
export default function openDetails(entityType, id) {
    const url = id ? `/${entityType}/${id}` : `/${entityType}`;
    window.open(url, '_blank');
}