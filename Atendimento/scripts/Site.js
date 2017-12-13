
function collapseEvent(colArea, colIcon) {
    $(colArea).on('shown.bs.collapse', { iconClass: colIcon }, changeIcon);
    $(colArea).on('hidden.bs.collapse', { iconClass: colIcon }, changeIcon);
}

function changeIcon(event) {
    $(event.data.iconClass).toggleClass('fa-caret-down fa-caret-right');
}

function ajaxError(error) {
    console.log(error);
    r = error;

    var $listaErros = $('<div></div>');

    if (angular.isDefined(error)) {
        if ((typeof error) == 'string')
            $listaErros.append('<div>' + error + '<div>');
        if (!angular.isDefined(error.ModelState) && angular.isDefined(error.Message)) {
            $listaErros.append('<div>' + error.Message + '<div>');
        }

        for (var key in error.ModelState) {
            for (var i in error.ModelState[key]) {
                $listaErros.append('<div>' + error.ModelState[key][i] + '<div>');
            }
        }
    }
    else if (angular.isDefined(error.responseText)) {
        $listaErros.append('<div>' + error.responseText + '<div>');
    }

    var texto = "";
    for (var i = 0; i < $listaErros.length; i++) {
        texto += $listaErros[i].innerHTML;
    }
    $.notify({ // options
        message: texto
    }, { // settings
        type: 'danger',
        delay: 5000,
        animate: {
            enter: 'animated fadeInDown'
            ,exit: 'animated fadeOutUp'
        },
        mouse_over: 'pause',
        z_index: 2500,
    });
}