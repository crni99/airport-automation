function initFetchControls(typeName, fetchUrl) {
    $(document).ready(function () {
        $('#searchButton').on('click', function () {
            fetchData(fetchUrl, typeName, 1);
        });

        $('#rowsPerPage').on('change', function () {
            fetchData(fetchUrl, typeName, 1);
        });

        fetchData(fetchUrl, typeName, 1);

        $(document).on('click', '.page-link', function (e) {
            e.preventDefault();
            var page = $(this).data('page');
            if (page) {
                fetchData(fetchUrl, typeName, page);
            }
        });
    });
}
