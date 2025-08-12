var currentPage = 1;
function getData(url, selectElement, textProperty1, textProperty2, textProperty3, currentPage) {
    var pageSize = 10;
    fetch(`${url}?page=${currentPage}&pageSize=${pageSize}`)
        .then(response => {
            if (response.ok) {
                return response.json();
            } else {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
        })
        .then(responseData => {
            if (!responseData.success) {
                console.error('Error: Empty or invalid data received');
                return;
            }

            var data = responseData.data.data;
            if (data.length > 0) {
                data.forEach(function (item) {
                    var optionElement = document.createElement('option');
                    optionElement.value = item.id;

                    var text = '';
                    if (textProperty1 && item[textProperty1]) {
                        text += item[textProperty1] + ' ';
                    }
                    if (textProperty2 && item[textProperty2]) {
                        text += item[textProperty2] + ' ';
                    }
                    if (textProperty3 && item[textProperty3]) {
                        text += item[textProperty3] + ' ';
                    }

                    optionElement.text = text.trim();
                    selectElement.appendChild(optionElement);
                });
            } else {
                throw new Error('Error: Empty or invalid data received');
            }
        })
        .catch(error => console.error('Error:', error));
}

function populateSelectFlight() {
    var airlineSelect = document.getElementById('airlineSelect');
    var destinationSelect = document.getElementById('destinationSelect');
    var pilotSelect = document.getElementById('pilotSelect');

    getData('/Airline/GetAirlines', airlineSelect, 'name', '', '', currentPage);
    getData('/Destination/GetDestinations', destinationSelect, 'city', 'airport', '', currentPage);
    getData('/Pilot/GetPilots', pilotSelect, 'firstName', 'lastName', 'uprn', currentPage);
    currentPage++;
}

document.addEventListener('DOMContentLoaded', function () {
    populateSelectFlight();
});
