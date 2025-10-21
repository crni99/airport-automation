function fetchData(fetchURL, entityType, page = 1) {
    var requestURL = createURL(fetchURL, entityType, page);

    var tableHead = $('#tableHead');
    var tableBody = $('#tableBody');

    $('#loadingSpinner').show();

    $.ajax({
        url: requestURL,
        type: 'GET',
        success: function (data) {
            if (data.success !== true || !data.data || data.data.totalCount === 0) {
                $('#dataNotFoundContainer').show();
                showAlertInContainer('No data found.', 'danger');
                return;
            }
            $('#dataForm').show();
            $('#paginationContainer').show();
            $('#paginationInfo').show();

            tableHead.empty();
            tableBody.empty();

            var fields = Object.keys(data.data.data[0]);
            createTableHead(tableHead, fields, entityType);

            var rowsData = data.data.data || [];
            $.each(rowsData, function (_, item) {
                createTableBody(item, tableBody, entityType);
            });
            paginationInfo(data.data.pageNumber, data.data.totalPages, data.data.totalCount);
            updatePagination(data.data.pageNumber, data.data.lastPage);
            $('[data-bs-toggle="tooltip"]').tooltip();

            if (entityType === 'TravelClass') {
                $('#paginationContainer').hide();
            }
        },
        error: function (xhr, status, error) {
            console.error('Error:', status, error);
            console.error('Response:', xhr.responseText);
            showAlertInContainer('An error occurred while fetching data.', 'danger');
        },
        complete: function () {
            $('#loadingSpinner').hide();
        }
    });
}

function createPaginationParams(page) {
    let storedPageSize = localStorage.getItem('pageSize');
    let selectedPageSize = $('#rowsPerPage').val();
    let pageSize = 10;

    if (selectedPageSize) {
        pageSize = selectedPageSize;
        if (selectedPageSize !== storedPageSize) {
            localStorage.setItem('pageSize', selectedPageSize);
        }
    } else if (storedPageSize) {
        pageSize = storedPageSize;
    }

    return `page=${page}&pageSize=${pageSize}`;
}

function createURL(fetchURL, entityType, page) {
   
    var paginationParams = createPaginationParams(page);

    switch (entityType) {

        case 'Airline':
            var searchName = $('#searchInput').val();
            if (!searchName || searchName.trim() === '') {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetAirlinesByName?name=${encodeURIComponent(searchName)}&${paginationParams}`;

        case 'ApiUser':
            var username = $('#usernameInput').val();
            var searchRole = $('#roleSelect').val();
            if ((!username || username.trim() === '') && (!searchRole || searchRole.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetApiUsersByFilter?username=${encodeURIComponent(username)}&roles=${encodeURIComponent(searchRole)}&${paginationParams}`;

        case 'Destination':
            var city = $('#city').val();
            var airport = $('#airport').val();
            if ((!city || city.trim() === '') && (!airport || airport.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetDestinationsByFilter?city=${encodeURIComponent(city)}&airport=${encodeURIComponent(airport)}&${paginationParams}`;

        case 'Flight':
            var startDate = $('#startDate').val();
            var endDate = $('#endDate').val();
            if ((!startDate || startDate.trim() === '') && (!endDate || endDate.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetFlightsBetweenDates?startDate=${encodeURIComponent(startDate)}&endDate=${encodeURIComponent(endDate)}&${paginationParams}`;

        case 'Passenger':
            var firstName = $('#firstName').val();
            var lastName = $('#lastName').val();
            var uprn = $('#uprn').val();
            var passport = $('#passport').val();
            var address = $('#address').val();
            var phone = $('#phone').val();
            if ((!firstName || firstName.trim() === '') && (!lastName || lastName.trim() === '') && (!uprn || uprn.trim() === '')
                && (!passport || passport.trim() === '') && (!address || address.trim() === '') && (!phone || phone.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetPassengersByFilter?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&uprn=${encodeURIComponent(uprn)}&passport=${encodeURIComponent(passport)}&address=${encodeURIComponent(address)}&phone=${encodeURIComponent(phone)}&${paginationParams}`;

        case 'Pilot':
            var firstName = $('#firstName').val();
            var lastName = $('#lastName').val();
            var uprn = $('#uprn').val();
            var flyingHours = $('#flyingHours').val();
            if ((!firstName || firstName.trim() === '') && (!lastName || lastName.trim() === '') &&
                (!uprn || uprn.trim() === '') && (!flyingHours || flyingHours.trim() === '')) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetPilotsByFilter?firstName=${encodeURIComponent(firstName)}&lastName=${encodeURIComponent(lastName)}&uprn=${encodeURIComponent(uprn)}&flyingHours=${encodeURIComponent(flyingHours)}&${paginationParams}`;

        case 'PlaneTicket':
            var price = $('#price').val();
            var purchaseDate = $('#purchaseDate').val();
            var seatNumber = $('#seatNumber').val();
            if (
                (!price || price.trim() === '') &&
                (!purchaseDate || purchaseDate.trim() === '') &&
                (!seatNumber || seatNumber.trim() === '')
            ) {
                return `${fetchURL}?${paginationParams}`;
            }
            return `/${entityType}/GetPlaneTicketsByFilter?price=${encodeURIComponent(price)}&purchaseDate=${encodeURIComponent(purchaseDate)}&seatNumber=${encodeURIComponent(seatNumber)}&${paginationParams}`;

        default:
            return `${fetchURL}?${paginationParams}`;
    }
}

function createTableHead(tableHead, fields, entityType) {
    var headerRow = $('<tr>');

    if (entityType === 'Flight') {
        headerRow.append($('<th>').text('ID'));
        headerRow.append($('<th>').text('Departure Date'));
        headerRow.append($('<th>').text('Departure Time'));
    }
    else if (entityType === 'PlaneTicket') {
        headerRow.append($('<th>').text('ID'));
        headerRow.append($('<th>').text('Price'));
        headerRow.append($('<th>').text('Purchase Date'));
        headerRow.append($('<th>').text('Seat Number'));
    }
    else {
        fields.forEach(function (field) {
            var header = $('<th>');
            header.text(field);
            headerRow.append(header);
        });
    }
    if (entityType !== 'TravelClass') {
        headerRow.append($('<th>').text('Actions').addClass('text-decoration-underline fw-bold'));
    }
    tableHead.append(headerRow);
}

function createTableBody(item, tableBody, entityType) {
    var row = $('<tr class="">');

    switch (entityType) {

        case 'Airline':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var nameCell = $('<td>');
            nameCell.text(item.name);
            row.append(nameCell);

            row.append(generateActionButtons(item.id, entityType));

            break;

        case 'ApiUser':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var userNameCell = $('<td>');
            userNameCell.text(item.userName);
            row.append(userNameCell);

            var passwordCell = $('<td>');
            passwordCell.text(item.password);
            row.append(passwordCell);

            var rolesCell = $('<td>');
            rolesCell.text(item.roles);
            row.append(rolesCell);

            row.append(generateActionButtons(item.id, entityType));

            break;

        case 'Destination':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var cityCell = $('<td class="link-primary clickable-row">');
            cityCell.html('<span data-bs-toggle="tooltip" data-bs-placement="right" title="Open Map">' + item.city + '</span>');
            cityCell.on("click", function () {
                openMap(item.city);
            });
            row.append(cityCell);

            var airportCell = $('<td class="link-primary clickable-row">');
            airportCell.html('<span data-bs-toggle="tooltip" data-bs-placement="right" title="Open Map">' + item.airport + '</span>');
            airportCell.on("click", function () {
                openMap(item.airport);
            });
            row.append(airportCell);

            row.append(generateActionButtons(item.id, entityType));

            break;

        case 'Flight':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var departureDateCell = $('<td>');
            departureDateCell.text(item.departureDate);
            row.append(departureDateCell);

            var departureTimeCell = $('<td>');
            departureTimeCell.text(item.departureTime);
            row.append(departureTimeCell);

            row.append(generateActionButtons(item.id, entityType));

            break;

        case 'Passenger':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var firstNameCell = $('<td>');
            firstNameCell.text(item.firstName);
            row.append(firstNameCell);

            var lastNameCell = $('<td>');
            lastNameCell.text(item.lastName);
            row.append(lastNameCell);

            var uprnCell = $('<td>');
            uprnCell.text(item.uprn);
            row.append(uprnCell);

            var passportCell = $('<td>');
            passportCell.text(item.passport);
            row.append(passportCell);

            var addressCell = $('<td class="link-primary clickable-row">');
            addressCell.html('<span data-bs-toggle="tooltip" data-bs-placement="right" title="Open Map">' + item.address + '</span>');
            addressCell.on("click", function () {
                openMap(item.address);
            });
            row.append(addressCell);

            var phoneCell = $('<td>');
            phoneCell.text(item.phone);
        
            row.append(phoneCell);

            row.append(generateActionButtons(item.id, entityType));

            break;

        case 'Pilot':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var firstNameCell = $('<td>');
            firstNameCell.text(item.firstName);
            row.append(firstNameCell);

            var lastNameCell = $('<td>');
            lastNameCell.text(item.lastName);
            row.append(lastNameCell);

            var uprnCell = $('<td>');
            uprnCell.text(item.uprn);
            row.append(uprnCell);

            var flyingHoursCell = $('<td>');
            flyingHoursCell.text(item.flyingHours);
            row.append(flyingHoursCell);

            row.append(generateActionButtons(item.id, entityType));

            break;

        case 'PlaneTicket':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var priceCell = $('<td>');
            priceCell.text(item.price);
            row.append(priceCell);

            var purchaseDateCell = $('<td>');
            purchaseDateCell.text(item.purchaseDate);
            row.append(purchaseDateCell);

            var seatNumberCell = $('<td>');
            seatNumberCell.text(item.seatNumber);
            row.append(seatNumberCell);

            row.append(generateActionButtons(item.id, entityType));

            break;

        case 'TravelClass':
            var idCell = $('<td>');
            idCell.text(item.id);
            row.append(idCell);

            var typeCell = $('<td>');
            typeCell.text(item.type);
            row.append(typeCell);
            break;

        default:
            console.warn('Unknown entity type: ' + entityType);
            break;
    }
    tableBody.append(row);
}

function paginationInfo(dataPageNumber, dataTotalPages, dataTotalCount) {
    var currentPage = $('#currentPage');
    var totalPages = $('#totalPages');
    var totalCount = $('#totalCount');

    currentPage.text(dataPageNumber);
    totalPages.text(dataTotalPages);
    totalCount.text(dataTotalCount);
}

function updatePagination(currentPage, lastPage) {
    $('.page-item').removeClass('disabled');
    if (currentPage == 1) {
        $('.page-item:first').addClass('disabled');
        $('.page-item:nth-child(2)').addClass('disabled');
    }
    if (currentPage == lastPage) {
        $('.page-item:last').addClass('disabled');
        $('.page-item:nth-child(3)').addClass('disabled');
    }
    $('.page-item:nth-child(2) a').data('page', currentPage - 1);
    $('.page-item:nth-child(3) a').data('page', currentPage + 1);
    $('.page-item:first a').data('page', 1);
    $('.page-item:last a').data('page', lastPage);
}

function generateActionButtons(id, entityType) {
    var openUrl = `/${entityType}/${id}`;
    var editUrl = `/${entityType}/Edit/${id}`;
    var deleteUrl = `/${entityType}/Delete/${id}`;

    var buttons = `
        <div class="btn-group btn-group-sm" role="group">
             <a href="${openUrl}" 
                 class="btn btn-icon-open me-3" 
                 target="_blank"
                 data-bs-toggle="tooltip"
			     data-bs-placement="right"
                 title="Open"
             >
                <i class="fa-solid fa-square-up-right fa-xl"></i>
            </a>`;

    if (currentUserRole !== 'User') {
        buttons += `
             <a href="${editUrl}" 
                 class="btn btn-icon-edit me-3" 
                 target="_blank"
                 data-bs-toggle="tooltip"
			     data-bs-placement="right"
                 title="Edit"
             >
                <i class="fa-solid fa-square-pen fa-xl"></i>
            </a>`;

        if (entityType !== 'ApiUser') {
            buttons += `
                <a href="${deleteUrl}" 
                    class="btn btn-icon-delete" 
                    target="_blank"
                    data-bs-toggle="tooltip"
			        data-bs-placement="right"
                    title="Delete"
                >
                    <i class="fa-solid fa-trash-can fa-xl"></i>
                </a>`;
        }
    }
    buttons += `</div>`;
    return $('<td>').html(buttons);
}
