$('#Right').click(function (e) {
    var selectedOpts = $('#selectedOpts option:selected');
    if (selectedOpts.length == 0 ) {
        e.preventDefault();
    }
    $('#AvailableOpts').append($(selectedOpts).clone());
    $(selectedOpts).remove();
    e.preventDefault();
    checkLeaderStatus()
})

$('#Left').click(function (e) {
    var availableOpts = $('#AvailableOpts option:selected');
    var totalMembers = $('#liderValue option').size() + $('#selectedOpts option').size();
    if (availableOpts.length == 0) {
        e.preventDefault();
    }
    if ( totalMembers + availableOpts.length < 6 ) {
        $('#selectedOpts').append($(availableOpts).clone());
        $(availableOpts).remove();
        e.preventDefault();
        checkLeaderStatus()
    }
})

$('#btnSubmit').click(function (e) {
    $('#selectedOpts option').prop('selected', true);
    $('#liderValue option').prop('selected', true);
})

$('#selectedOpts').click(function (e) {
    checkLeaderStatus()
})

$('#leaderButton').click(function (e) {
    var leaderButton = document.getElementById("leaderButton");
    if (leaderButton.value == "Asignar Lider") {
        var liderSelected = $('#selectedOpts option:selected');
        $('#liderValue').append($(liderSelected).clone());
        $(liderSelected).remove();
        e.preventDefault();
        leaderButton.value = "Remover Lider";
        leaderButton.className = "btn btn-danger";
    } else {
        alert("here");
        var selectedOpts = $('#liderValue option:selected');
        $('#selectedOpts').append($(selectedOpts).clone());
        $(selectedOpts).remove();
        e.preventDefault();
        leaderButton.value = "Asignar Lider";
        leaderButton.className = "btn btn-success";
        document.getElementById("leaderButton").disabled = true;
    }
})

function checkLeaderStatus() {
    var leaderButton = document.getElementById("leaderButton");
    if (leaderButton.value == "Asignar Lider") {
        if ( $('#selectedOpts option:selected').length == 1 && $('#liderValue option').size() == 0 ) {
            document.getElementById("leaderButton").disabled = false;
        } else { document.getElementById("leaderButton").disabled = true; }
    }
}


