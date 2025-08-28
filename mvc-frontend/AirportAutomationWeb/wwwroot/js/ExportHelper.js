$(document).ready(function () {
    $('[id^="exportButton"]').on('click', function () {
        const container = $(this).closest('[data-entity]');
        const entityType = container.data('entity');
        const fileType = $(this).data('type').toLowerCase();

        console.log(container);
        console.log(entityType);
        console.log(fileType);
        if (entityType) {
            exportData(entityType, fileType);
        } else {
            console.warn('Entity type is not set.');
        }
    });
});

function exportData(entityType, fileType = 'excel') {
    const currentPage = parseInt($('#currentPage').text()) || 1;
    let exportURL = createURL('', entityType, currentPage); // e.g. "/Passenger/GetPassengersByFilter?..."

    let urlParts = exportURL.split('?');
    let path = urlParts[0];          // "/Passenger" or "/Passenger/GetPassengersByFilter"
    let query = urlParts[1] || '';   // "firstName=A&..."

    const originalPath = path;

    // Replace known data-fetching endpoints with 'Export'
    path = path.replace(/Get\w+ByFilter|Get\w+ByName|GetFlightsBetweenDates/, 'Export');

    // If no endpoint was replaced, and path doesn't already end with /Export, then append it
    if (path === originalPath && !path.endsWith('/Export')) {
        // Ensure it includes the entity (e.g. /Passenger)
        if (!path.startsWith(`/${entityType}`)) {
            path = `/${entityType}`;
        }

        path += path.endsWith('/') ? 'Export' : '/Export';
    }

    // Remove pagination params
    query = query
        .replace(/page=\d+&?/g, '')
        .replace(/pageSize=\d+&?/g, '')
        .replace(/[&?]+$/, '');

    // Convert query to params
    const params = new URLSearchParams(query);

    // Determine if there are any filters (any non-empty values)
    let hasFilters = false;
    for (const [key, value] of params.entries()) {
        if (value.trim() !== '') {
            hasFilters = true;
            break;
        }
    }

    // Add getAll=true if no filters
    if (!hasFilters) {
        params.append('getAll', 'true');
    }

    // Always add fileType
    params.append('fileType', fileType);

    const finalURL = `${path}?${params.toString()}`;

    // Debug (optional)
    console.log('Final export URL:', finalURL);

    // Trigger download
    setTimeout(() => {
        window.location.href = finalURL;
    }, 3000);
}
