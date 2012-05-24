ko.bindingHandlers.videoInit = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        if (navigator.getUserMedia) {
            navigator.getUserMedia({'video':true, 'audio':false}, function (raw) {

                viewModel.stream = raw;
                element.src = raw;
            });
        }
    }
};
