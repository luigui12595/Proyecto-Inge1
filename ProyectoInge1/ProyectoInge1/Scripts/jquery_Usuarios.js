function gotoDetails(element) {
    var elementId = element.id;
    window.location.replace('Usuarios/Detalles/'+elementId)
}

function goToProjectDetails(element) {
    var elementId = element.id;
    window.location.replace('Proyectos/Detalles/' + elementId)
}

function goToReqFun(element) {
    var elementNombre = element.id;
    window.location.replace('ReqFuncional/Index/' + elementNombre)
}

function goToDetailsSolicitud(element) {
    var data = element.id;
    window.location.replace('GestCambios/Solicitudes/' + data)
}