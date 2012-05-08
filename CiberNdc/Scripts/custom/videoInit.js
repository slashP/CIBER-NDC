ko.bindingHandlers.videoInit = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        if (navigator.getUserMedia) {
            navigator.getUserMedia({'video':true, 'hints': {'video': {'orientation': 'front'}}}, function (stream) {

                viewModel.stream = stream;
                element.src = stream;
            });
        }
    }
};
