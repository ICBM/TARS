/*******************************************************************************************
**  Modal Popup that sizes to any display data passed via modal.open({ content: data })   **
*******************************************************************************************/
$function() {
    var method = {}, $overlay, $modal, $modalContent, $modalClose;

    // Center the modal in the viewport
    method.center = function () {
        var top, left;
        top = Math.max($(window).height() - $modal.outerHeight(), 0) / 2;
        left = Math.max($(window).width() - $modal.outerWidth(), 0) / 2;
        $modal.css({
            top: top + $(window).scrollTop(),
            left: left + $(window).scrollLeft()
        });
    };

    // Open the modal
    method.open = function (settings) {
        $modalContent.append(settings.content);
        $modal.css({
            width: settings.width || 'auto',
            height: settings.height || 'auto'
        })
        method.center();
        $(window).bind('resize.modal', method.center);
        $modal.show();
        $overlay.show();
    };

    // Close the modal
    method.close = function () {
        $modal.hide();
        $overlay.hide();
        $modalContent.empty();
        $(window).unbind('resize.modal');
    };

    $overlay = $('<div id="overlay"></div>');
    $modal = $('<div id="modal"></div>');
    $modalContent = $('<div id="modalContent"></div>');
    $modalClose = $('<a id="modalClose" href="#">modalClose</a>');
    $modal.hide();
    $overlay.hide();
    $modal.append($modalContent, $modalClose);

    $(document).ready(function () {
        $('body').append($overlay, $modal);
    });

    $modalClose.click(function (e) {
        e.preventDefault();
        method.close();
    });

    return method;
} ());

// Wait until the DOM has loaded before querying the document
$(document).ready(function () {
    $.get('ajax.html', function (data) {
        modal.open({ modalContent: data });
    });
});