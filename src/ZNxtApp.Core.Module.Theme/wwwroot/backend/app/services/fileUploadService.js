(function () {
    'use strict';
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.directive('fileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var model = $parse(attrs.fileModel);
                var modelSetter = model.assign;

                element.bind('change', function () {
                    scope.$apply(function () {
                        modelSetter(scope, element[0].files[0]);
                    });
                });
            }
        };
    }]);

    ZApp.service('fileUploadService', ['$http', function ($http) {
        this.uploadFileToUrl = function (fileData, uploadUrl, headres, successCallBack, errorCallback) {
            var fd = new FormData();
            fd.append('file', fileData);
            if (headres == undefined) {
                headres = {};
            };
            headres["Content-Type"] = undefined;
            $http({
                url: uploadUrl,
                method: "POST",
                data: fd,
                headers: headres,
            }).then(function (response) {
                successCallBack(response);
            }, function () {
                errorCallback();
            });;
        }
    }]);
})();