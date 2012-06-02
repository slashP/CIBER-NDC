ko.bindingHandlers.takePicture = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

        $(element).click(
            function () {
                var canvas = $('canvas').get(0);
                video.height = Math.floor((video.videoHeight / video.videoWidth) * $(".purple-square").width());
                canvas.height = (video.height);
                canvas.width = (video.width);
                $("#myCanvas").height(video.height);
                $("#myCanvas").width(video.width);
                console.log(canvas.width + "/" + canvas.height);
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
            ctx.drawImage(video, 0, 0, canvas.width, canvas.height);
            console.log(canvas.width + "/" + canvas.height);
            viewModel.imageData(canvas.toDataURL("image/jpg"));
            //img.src = canvas.toDataURL('image/png');
        }

    }
};
