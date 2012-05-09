﻿ko.bindingHandlers.takePicture = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {

        $(element).click(
            function () {
                ko.bindingHandlers.takePicture.update(element, valueAccessor, allBindingsAccessor, viewModel);
            });
    },

    update: function (element, valueAccessor, allBindingsAccessor, viewModel) {

        var canvas = $('canvas').get(0);
        var video = $('video').get(0);
        var img = $('img').get(0);
        
        var ctx = canvas.getContext('2d');
        if (viewModel.stream) {
            ctx.drawImage(video, 0, 0);
            viewModel.imageData(canvas.toDataURL("image/png"));
            //img.src = canvas.toDataURL('image/png');
        }

    }
};