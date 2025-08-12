export default function openDetails(entityType, id) {
    const url = id ? `/${entityType}/${id}` : `/${entityType}`;
    window.open(url, '_blank');
}