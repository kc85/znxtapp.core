var __myPhotoAppName = 'MyPhotoGalleryApp';

var __userData = {};
(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, ['infinite-scroll']);
    MyPhotoApp.constant('userData', __userData);

    MyPhotoApp.controller(__myPhotoAppName + '.Main', ['$scope', '$location', '$rootScope', '$http', 'userData', '$timeout',
       function ($scope, $location, $rootScope, $http, userData, $timeout) {

           $scope.busy = false;
           $scope.pagesize = 20;
           $scope.currentpage = 1;

           $scope.name = "test";
           $scope.gallery = [];
           function active() {
               $scope.pagesize = 50
               fetchImage(function () {
                   $scope.pagesize = 20;
               });
           }

           $scope.loadMore = function () {
           
               if ($scope.busy == false) {
                   $scope.busy = true;
                   $scope.currentpage++;

                   fetchImage(function () {
                       $scope.busy = false;
                   });

               }
           };

           function fetchImage(callback) {
               var url = "../api/myphotos/gallery?pagesize=" + $scope.pagesize + "&currentpage=" + $scope.currentpage;
               $http.get(url).then(function (response) {
                   console.log(response);
                   response.data.data.forEach(function (d) {
                       $scope.gallery.push(d);
                   });
                   if (callback != undefined) {
                       callback();
                   }
               });
           }
           active();

       }]);

})();