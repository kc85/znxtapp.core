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
           $scope.user = undefined;
           $scope.name = "";
           $scope.gallery = [];
           function active() {
               $scope.pagesize = 50
               fetchImage(function () {
                   $scope.pagesize = 20;
               });
               getUserInfo();
           }
           function getUserInfo() {
               var url = "../api/myphotos/userinfo";
               $http.get(url).then(function (response) {
                   console.log(response)
                   if (response.data.code == 1) {
                       $scope.user = response.data.data;
                   }
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
           $scope.redirectToLogin = function () {
               window.location = "../signup/login.z?rurl=" + window.location.href.replace("/index.z", "/clearcache.z");
           };
           function fetchImage(callback) {
               var url = "../api/myphotos/gallery?pagesize=" + $scope.pagesize + "&currentpage=" + $scope.currentpage;
               $http.get(url).then(function (response) {
                   response.data.data.forEach(function (d) {
                       if (d.thumbnail_image.changeset_no == undefined) {
                           d.thumbnail_image.changeset_no = 0;
                       }
                       $scope.gallery.push(d);
                   });
                   if (callback != undefined) {
                       callback();
                   }
                   setTimeout(function () {
                       response.data.data.forEach(function (d) {
                           var cacheUrl = "./gallery.z?galleryid=" + d.id;
                           $http.get(cacheUrl)
                          
                       });
                   }, 1000 * 10);

               });
           }
           active();

       }]);

})();