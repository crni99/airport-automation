function createAlertRow(message, type) {
    var alertClass = 'alert-' + type;
    return '<div class="alert ' + alertClass + '" role="alert">' +
        message +
        '</div>';
}

function autoCloseAlert() {
    setTimeout(function () {
        $('.alert').remove();
    }, 4000);
}

function showAlertInContainer(message, type) {
    var alertRow = createAlertRow(message, type);
    $('#alert').html(alertRow);
    autoCloseAlert();
}