ko.bindingHandlers.message = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

    },

    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {

        if (valueAccessor()()) {
            setTimeout(function () {
                viewModel.success(false);
                viewModel.failure(false);
            },2000);
        }
    }

};
