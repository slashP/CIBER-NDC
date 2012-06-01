ko.bindingHandlers.videoInit = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        if (navigator.getUserMedia) {
            navigator.getUserMedia({ 'video': true, 'audio': false }, function (raw) {

                viewModel.stream = raw;
                element.src = raw;
            });
        } else {
            alert("Your browser does not support access to your camera, try using the latest version of Opera Mobile!");
        }
    }
};
