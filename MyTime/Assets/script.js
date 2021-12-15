
function toggleSidebar() {

    $("#sidebar").toggleClass('active');

}


function loadSpinner() {
    var divSpinner = document.getElementById("divSpinner");
    divSpinner.style.display = 'block';
}


function unloadSpinner() {
    var divSpinner = document.getElementById("divSpinner");
    divSpinner.style.display = 'none';
}


function loadDivCRUD() {

    //$('#divList').removeClass('col-md-12').addClass('col-md-6 d-none d-sm-block');
    //$('#divCRUD').show();

    //$('#divList').hide();
    //$('#divCRUD').show();

    $('#modalCRUD').modal('show');
   
};

function unloadDivCRUD() {

    unloadSpinner();

    //$('#divCRUD').hide();
    //$('#divList').removeClass('col-md-6 d-none d-sm-block').addClass('col-md-12');

    //$('#divCRUD').hide();
    //$('#divList').show();

    $('#modalCRUD').modal('hide');

};

function showDivBody() {
    var divBody = document.getElementById('divBody');
    setTimeout(function () { divBody.style.opacity = "100"; }, 500);
};


function addElementEvent(element, url) {

    //var element = document.getElementById(elementName);

    if (!element == null) {
        element.addEventListener('click', function () {

            window.location.href = url;

        });
    }
    else {

    }

    return false;

}

function setDateTimePickerLocale(culture) {

    if (culture.trim() == 'ms-MY') {

        $.datepicker.regional['ms-MY'] = {
            closeText: "Tutup",
            prevText: "&#x3C;Sebelum",
            nextText: "Selepas&#x3E;",
            currentText: "hari ini",
            monthNames: ["Januari", "Februari", "Mac", "April", "Mei", "Jun",
                "Julai", "Ogos", "September", "Oktober", "November", "Disember"],
            monthNamesShort: ["Jan", "Feb", "Mac", "Apr", "Mei", "Jun",
                "Jul", "Ogo", "Sep", "Okt", "Nov", "Dis"],
            dayNames: ["Ahad", "Isnin", "Selasa", "Rabu", "Khamis", "Jumaat", "Sabtu"],
            dayNamesShort: ["Aha", "Isn", "Sel", "Rab", "kha", "Jum", "Sab"],
            dayNamesMin: ["Ah", "Is", "Se", "Ra", "Kh", "Ju", "Sa"],
            weekHeader: "Mg",
            dateFormat: "dd/mm/yy",
            firstDay: 0,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ""
        };

        $.datepicker.setDefaults($.datepicker.regional['ms-MY']);

    }
    else {

        $.datepicker.regional['en-GB'] = {
            closeText: "Done",
            prevText: "Prev",
            nextText: "Next",
            currentText: "Today",
            monthNames: ["January", "February", "March", "April", "May", "June",
                "July", "August", "September", "October", "November", "December"],
            monthNamesShort: ["Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
            dayNames: ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"],
            dayNamesShort: ["Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat"],
            dayNamesMin: ["Su", "Mo", "Tu", "We", "Th", "Fr", "Sa"],
            weekHeader: "Wk",
            dateFormat: "dd/mm/yy",
            firstDay: 1,
            isRTL: false,
            showMonthAfterYear: false,
            yearSuffix: ""
        };

        $.datepicker.setDefaults($.datepicker.regional['en-GB']);
    };

    return false;
}




