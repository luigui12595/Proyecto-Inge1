$('#Right').click(function (e) {
    var selectedOpts = $('#selectedOpts option:selected');
    if (selectedOpts.length == 0 ) {
        alert("Nada que mover");
        e.preventDefault();
    }
    $('#AvailableOpts').append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
})

$('#Left').click(function (e) {
    var availableOpts = $('#AvailableOpts option:selected');
    if (availableOpts.length == 0) {
        alert("Nada que asignar");
        e.preventDefault();
    }
    $('#selectedOpts').append($(availableOpts).clone());
    $(availableOpts).remove();
    e.preventDefault();
})

$('#btnSubmit').click(function (e) {
    alert("here catch");
    $('#selectedOpts option').prop('selected', true);
})