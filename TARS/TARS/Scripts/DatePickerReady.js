if (!Modernizr.inputtypes.date) {
    $(function () {
        $(".datefield").datepicker({
            changeMonth: true,
            changeYear: true,
            yearRange: 'c-10:c+20'
        });
    });
}