var VIDEO_WIDTH, VIDEO_HEIGHT,
computeSize = function (supportsObjectFit) {
	// user agents that don't support object-fit 
	// will display the video with a different 
	// aspect ratio. 

	var width = $(".purple-square").width();
	var height = $(".purple-square").height();
	var aspect = width / height;
	console.log("aspect: " + aspect + "(" + width + "/" + height +")");
	
	if (aspect < 1) {
		video.height = $(".purple-square").height();
	} else {
		video.width = $(".purple-square").width();
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
			alert("Din nettleser støtter ikke bruk av kameraet, prøv å bruke siste versjon av Opera Mobile!");
		}
	}
};

