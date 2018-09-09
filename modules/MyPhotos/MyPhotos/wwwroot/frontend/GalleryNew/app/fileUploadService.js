(function () {
    'use strict';
    var __myPhotoAppName = 'MyPhotoApp';
    var ZApp = angular.module(__myPhotoAppName);

    
    ZApp.directive('fileModel', ['$parse', function ($parse) {
        return {
            restrict: 'A',
            link: function (scope, element, attrs) {
                var model = $parse(attrs.fileModel);
                var modelSetter = model.assign;
                scope.uploadFiles  = [];

                element.bind('change', function (e) {
                    for (var i = 0; i < element[0].files.length; i++) {
                        scope.uploadFiles.push(element[0].files[i])
                    }

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
            for (var i in fileData) {
                fd.append("file", fileData[i]);
            }

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