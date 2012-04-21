if (!Modernizr.inputtypes.date) {
    $(function () {
        $(".datefield").datepicker({
            showAnim: 'slideDown',
        });
    });
}