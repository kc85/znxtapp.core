var __myPhotoAppName = 'MyPhotoGalleryApp';

(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, ['infinite-scroll']);

    MyPhotoApp.controller(__myPhotoAppName + '.Main', ['$scope', '$location', '$rootScope', '$http', '$timeout','$window',
       function ($scope, $location, $rootScope, $http, $timeout, $window) {

           $scope.user = undefined;
           $scope.isShowBookmark = undefined;
           function active() {
               $scope.screenHeight = $window.innerHeight + 30;
               getUserInfo();
           }
           function getUserInfo() {
               var url = "../api/user/me";
               $http.get(url).then(function (response) {
                   console.log(response)
                   if (response.data.code == 1) {
                       $scope.user = response.data.data;
                   }
                   else {
                       $scope.user = undefined;
                   }
               });
           }
           active();

       }]);

})();