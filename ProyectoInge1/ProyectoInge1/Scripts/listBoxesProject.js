$('#Right').click(function (e) {
    var selectedOpts = $('#selectedOpts option:selected');
    if (selectedOpts.length == 0 ) {
        e.preventDefault();
    }
    $('#AvailableOpts').append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
})

$('#Left').click(function (e) {
    var availableOpts = $('#AvailableOpts option:selected');
    var totalMembers = $('#selectedOpts option').size();
    if (availableOpts.length == 0) {
        e.preventDefault();
    }
    if ( totalMembers + availableOpts.length < 5 ) {
        $('#selectedOpts').append($(availableOpts).clone());
        $(availableOpts).remove();
        e.preventDefault();
    }
})

$('#btnSubmit').click(function (e) {
    $('#selectedOpts option').prop('selected', true);
})

$("#leaderSel").on('focus', function () {
    var ddl = $(this);
    ddl.data('previous', ddl.val());
}).on('change', function () {
    var ddl = $(this);
    var previous = ddl.data('previous');
    var liderSelector = document.getElementById("leaderSel");
    for (var i = 0; i < liderSelector.options.length; ++i)
        if (previous == liderSelector.options[i].value && previous != "" && $("#AvailableOpts option[value='" + previous + "']").length == 0)
            $('#AvailableOpts').append($(liderSelector.options[i]).clone());
    var availableOpts = document.getElementById('AvailableOpts');
    var selectedOpts = document.getElementById('selectedOpts');
    for (var i = 0; i < availableOpts.options.length; ++i)
        if ($(this).val() == availableOpts.options[i].value)
            availableOpts.remove(i);
    for (var i = 0; i < selectedOpts.options.length; ++i)
        if ($(this).val() == selectedOpts.options[i].value)
            selectedOpts.remove(i);
    ddl.data('previous', ddl.val());
});


