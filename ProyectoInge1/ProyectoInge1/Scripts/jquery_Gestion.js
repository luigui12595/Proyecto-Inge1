﻿function gotoDetails(element) {
    var elementId = element.id;
    window.location.replace('GestCambios/Detalles/' + elementId)
}

function goToProjectDetails(element) {
    var elementId = element.id;
    window.location.replace('Proyectos/Detalles/' + elementId)
}

function goToReqFun(element) {
    var elementNombre = element.id;
    window.location.replace('ReqFuncional/Index/' + elementNombre)
}