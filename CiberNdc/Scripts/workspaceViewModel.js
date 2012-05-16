var workspaceViewModel = function () {

    var myself = this;

    this.stream = null;
    this.imageData = ko.observable();
    this.url = '/Home/Test/';

    this.success = ko.observable(false);
    this.failure = ko.observable(false);
    this.loading = ko.observable(false);
    var json = new Object();
    this.name = ko.observable('');
    this.codeword = ko.observable('codeword');

    this.imageData.subscribe(function (value) {

        json.image = value;
        json.codeword = myself.codeword();
        myself.failure(false);
        myself.success(false);
        $.ajax({
            url: myself.url,
            type: 'post',
            beforeSend: function () { myself.loading(true); },
            contentType: 'application/json; charset=utf-8',
            data: JSON.stringify(json),
            success: myself.successCallback,
            error: myself.errorCallback
        });
    });


    this.successCallback = function (data) {

        myself.name('');

        myself.loading(false);

        if (data.success) {
            myself.name(data.name);

            myself.success(true);
            myself.failure(false);
        }
        else {
            myself.failure(true);
            myself.success(false);

        }
    };

    this.errorCallback = function (data) {
        myself.loading(false);
        alert('Something went wrong');
    };


    this.showMsg = function () {
        myself.loading(false);
        return myself.success() || myself.failure();

    };
}
