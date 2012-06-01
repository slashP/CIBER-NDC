var VIDEO_WIDTH, VIDEO_HEIGHT,
computeSize = function(supportsObjectFit) {
	// user agents that don't support object-fit 
	// will display the video with a different 
	// aspect ratio. 
	if (supportsObjectFit === true) {
		VIDEO_WIDTH = 640;
		VIDEO_HEIGHT = 480;
	} else {
		VIDEO_WIDTH = video.videoWidth;
		VIDEO_HEIGHT = video.videoHeight;
	}
},

successCallback = function (stream) {
	video.src = stream;
	video.play();
	computeSize(true);
},
errorCallback = function (error) {
	console.error('An error occurred: [CODE ' + error.code + ']');
	computeSize(true);
};

ko.bindingHandlers.videoInit = {
    init: function (element, valueAccessor, allBindingsAccessor, viewModel) {
        if (navigator.getUserMedia) {
            navigator.getUserMedia({ 'video': true, 'audio': false }, function (raw) {
                viewModel.stream = raw;
                element.src = raw;
                computeSize(true);
            }, errorCallback);
        } else if (navigator.webkitGetUserMedia) {

            navigator.webkitGetUserMedia("video", function (stream) {
                viewModel.stream = stream;
                element.src = window.webkitURL.createObjectURL(stream);
                computeSize(false);
            });

        } else {
            alert("Your browser does not support access to your camera, try using the latest version of Opera Mobile, or activate it in Chrome by setting the MediaStream-flag in chrome://flags/");
        }
    }
};

