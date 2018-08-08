var __myPhotoAppName = 'MyPhotoAppUser';

(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, []);
    MyPhotoApp.controller(__myPhotoAppName + '.main', ['$scope', '$http',
       function ($scope, $http) {

           $scope.user_pic = "";
           function active() {
               getUserInfo();
           }
           function getUserInfo() {
               var url = "../api/user/me";
               $http.get(url).then(function (response) {
                   if (response.data.code == 1) {
                       $scope.user = response.data.data;
                       var userInfoUrl = "../api/user/userinfo?user_id=" + $scope.user.user_id;
                       $http.get(userInfoUrl).then(function (response) {
                           console.log(response);

                           $scope.user_pic = response.data.data.user_info[0].user_pic;
                       });
                   }
                   else {
                       $scope.user = undefined;
                   }
               });
           }
           active();

       }]);

})();
