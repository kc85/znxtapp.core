var __myPhotoAppName = 'MyPhotoAppUser';

(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, []);
    MyPhotoApp.controller(__myPhotoAppName + '.main', ['$scope', '$http',
       function ($scope, $http) {


           function active() {
               getUserInfo();
           }
           function getUserInfo() {
               var url = "../api/myphotos/userinfo";
               $http.get(url).then(function (response) {
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
