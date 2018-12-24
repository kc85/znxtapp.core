var __myPhotoDetailsAppName = 'MyPhotoAppDetails';

var __userData = {};
(function () {
    var MyPhotoApp = angular.module(__myPhotoDetailsAppName, ['infinite-scroll']);
    MyPhotoApp.constant('userData', __userData);

    MyPhotoApp.controller(__myPhotoDetailsAppName + '.Main', ['$scope', '$location', '$rootScope', '$http', 'userData', '$timeout',
       function ($scope, $location, $rootScope, $http, userData, $timeout) {

           $scope.imagedata = {}
          
           function active() {
               $scope.file_hash = GetParameterValues("file_hash");
               $scope.galleryid = GetParameterValues("galleryid");
               fetchImage();
             
           }
          
           function fetchImage(callback) {
               var url = "../api/myphotos/fetch?file_hash=" + $scope.file_hash + "&galleryid=" + $scope.galleryid;
               $http.get(url).then(function (response) {                   
                   $scope.imagedata = response.data.data;
                   if (callback != undefined) {
                       callback();
                   }
               });
           };
           active();

       }]);

})();

function GetParameterValues(param) {
    var url = window.location.href.slice(window.location.href.indexOf('?') + 1).split('&');
    for (var i = 0; i < url.length; i++) {
        var urlparam = url[i].split('=');
        if (urlparam[0] == param) {
            return urlparam[1];
        }
    }
}