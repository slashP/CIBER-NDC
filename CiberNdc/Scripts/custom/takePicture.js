ko.bindingHandlers.takePicture = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

        $(element).click(
            function () {
                ko.bindingHandlers.takePicture.update(element, valueAccessor, allBindingsAccessor, viewModel);
            });
    },

    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        computeSize(true);
        var canvas = $('canvas').get(0);
        var video = $('video').get(0);
        //var img = $('img').get(0);

        var ctx = canvas.getContext('2d');
        if (viewModel.stream) {
            ctx.drawImage(video, 0, 0, video.width, video.height);
            viewModel.imageData(canvas.toDataURL("image/jpg"));
            //img.src = canvas.toDataURL('image/png');
        }

    }
};
