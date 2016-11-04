function gotoDetails(element) {
    var elementId = element.id;
    window.location.replace('Usuarios/Detalles/'+elementId)
}

function goToProjectDetails(element) {
    var elementId = element.id;
    alert(elementId);
    window.location.replace('Proyectos/Detalles/' + elementId)
}

function goToReqFun(element) {
    var elementId = element.id;
    window.location.replace('ReqFuncional/Index/' + elementId)
}