$(document).ready(function () {
    $('#clearButton').on('click', function () {
        $('.container').find('input, select, textarea').each(function () {
            const type = $(this).attr('type');

            if (type === 'checkbox' || type === 'radio') {
                $(this).prop('checked', false);
            } else {
                $(this).val('');
            }
        });
    });
});