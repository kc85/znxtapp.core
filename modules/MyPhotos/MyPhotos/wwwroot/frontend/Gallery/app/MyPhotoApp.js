var __myPhotoAppName = 'MyPhotoApp';

var __userData = {};
(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, ['infinite-scroll']);
    MyPhotoApp.constant('userData', __userData);

    MyPhotoApp.controller(__myPhotoAppName + '.Main', ['$scope', '$location', '$rootScope', '$http', 'userData', '$timeout',
       function ($scope, $location, $rootScope, $http, userData, $timeout) {

           $scope.busy = false;
           $scope.pagesize = 40;
           $scope.currentpage = 0;
           $scope.galleryid = GetParameterValues("galleryid");
           if ($scope.galleryid == undefined) {
               window.location = "./gallery.z";
           }
           $scope.name = "test";
           $scope.images = [];
           $scope.gallery_files = [];
           function active() {
               //$scope.pagesize = 50
               //fetchImage(function () {
               //    $scope.pagesize = 20;
               //});
           }

           $scope.loadMore = function () {
           
               if ($scope.busy == false) {
                   $scope.busy = true;
                   fetchImage(function () {
                       $scope.busy = false;
                       $scope.currentpage++;
                   });
               }
           };

           function fetchImage(callback) {
               //var url = "../api/myphotos/fetch?pagesize=" + $scope.pagesize + "&currentpage=" + $scope.currentpage;
               //$http.get(url).then(function (response) {
               //    console.log(response);
               //    response.data.data.forEach(function (d) {
               //        $scope.images.push(d);
               //    });
               //    if (callback != undefined) {
               //        callback();
               //    }
               //});

               var url = "../api/myphotos/gallery?id=" + $scope.galleryid;
               $http.get(url).then(function (response) {
                   $scope.gallery_files = response.data.data.file_hashs;                  
                   if (callback != undefined) {
                       for (var i = ($scope.pagesize * $scope.currentpage) ; i < ($scope.pagesize * ($scope.currentpage + 1)) && i < $scope.gallery_files.length ; i++) {
                           $scope.images.push($scope.gallery_files[i]);
                       }
                       callback();
                   }
               });

           }
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