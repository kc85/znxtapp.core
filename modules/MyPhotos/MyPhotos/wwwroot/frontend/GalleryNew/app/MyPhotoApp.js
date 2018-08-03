var __myPhotoAppName = 'MyPhotoApp';

var __userData = {};
(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, ['infinite-scroll']);
   
    MyPhotoApp.controller(__myPhotoAppName + '.Main', ['$scope', '$location', '$rootScope','$window', '$http', '$timeout',
       function ($scope, $location, $rootScope, $window, $http, $timeout) {

           $scope.busy = false;
           $scope.pagesize = 50;
           $scope.currentpage = 0;
           $scope.user = undefined;
           $scope.rotateText = "Rotate";
           $scope.loading = false;
           $scope.selectedImageChangesetNo = 0;
           $scope.selectedImageIndex = 0;
           $scope.isbackbuttonpress = true;
           $scope.showImageTopToolbar = false;
           $scope.galleryid = GetParameterValues("galleryid");
           if ($scope.galleryid == undefined) {
               window.location = "./indexnew.z";
           }
           $scope.gallery_name = "";
           $scope.screenHeight = 0;
           $scope.images = [];
           $scope.gallery_files = [];
           function active() {
               $scope.screenHeight = $window.innerHeight+30;
               getUserInfo();
               showImageDetailsDialog(function (file) {
                   $scope.loadMore(function () {
                       if (file != null) {
                           $scope.isbackbuttonpress = false;
                       }
                       $scope.selectImage(file);
                   });
               });
           }
           function getUserInfo() {
               var url = "../api/myphotos/userinfo";
               $http.get(url).then(function (response) {
                   console.log(response)
                   if (response.data.code == 1) {
                       $scope.user = response.data.data;
                       __userData = $scope.user;
                   }
                   //else if (response.data.code == 401) {
                   //    $scope.user = response.data.data;
                   //    showLogin();
                   //}
                   else {
                       $scope.user = undefined;
                       __userData = undefined;

                   }
               });
           }
           $scope.closeImageDetail = function () {
               if (document.referrer.indexOf("znxt.app") != -1) {
                   window.history.back();
               }
               else {
                   window.location.hash = "#";
               }
           };
           $scope.rotateImage = function () {
               if ($scope.loading != true) {
                   $scope.loading = true;
                   var url = "../api/myphotos/image/rotate?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid;
                   $scope.rotateText = "...";
                   $http.post(url).then(function (response) {
                       $scope.loading = false;
                       lastFileHash = "";
                       $scope.rotateText = "Rotate";
                       $scope.selectedImage = response.data.data;
                       $scope.gallery_files.forEach(function (d) {
                           if (d.file_hash == $scope.selectedImage.file_hash) {
                               d.changeset_no = $scope.selectedImage.changeset_no;
                           }
                       });
                   });
               };
           };

           $scope.loadMore = function (callback) {
           
               if ($scope.busy == false) {
                   $scope.busy = true;
                   fetchPageImage(function () {
                       $scope.busy = false;
                       $scope.currentpage++;
                       if (callback != undefined) {
                           callback();
                       }
                   });
               }
           };
           $scope.detailImageClick = function () {
               $scope.showImageTopToolbar = !$scope.showImageTopToolbar;
           }
           $scope.imageClick = function () {
               $scope.isbackbuttonpress = false;
           }
           $scope.selectImage = function (image) {
               if (image != undefined) {
                   $scope.selectedImagedata = undefined;
                   $scope.selectedImage = image;
                   $scope.shareLink = window.location.href;
                   $scope.shareText = "Share Image";
                   $scope.selectedImageIndex = fileIndex(image.file_hash);
                   fetchImageDetails();
               }
               else {
                   $scope.selectedImagedata = undefined;
                   $scope.selectedImage = undefined;
                   $scope.shareLink = "";
                   $scope.shareText = "";
                   $scope.selectedImageIndex = 0;
               }
               $scope.isbackbuttonpress = true;
               $scope.showImageTopToolbar = false;
           }
           function fetchImageDetails(callback) {
               var url = "../api/myphotos/fetch?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid + "&pagesize=10";
               $http.get(url).then(function (response) {
                   $scope.shareLink = window.location.href;
                   $scope.shareText = "Share Image";

                   if (response.data.code == 401) {
                       $scope.user = response.data.data;
                       showLogin();
                   }
                   else {
                       $scope.selectedImagedata = response.data.data;
                       console.log($scope.selectedImagedata);
                   }
                   if (callback != undefined) {
                       callback();
                   }
               });
           };
           function fetchPageImage(callback) {
               var url = "../api/myphotos/gallery?id=" + $scope.galleryid + "&currentpage=" + $scope.currentpage + "&pagesize=" + $scope.pagesize;
               $http.get(url).then(function (response) {
                   if (response.data.code == 401) {
                       $scope.user = response.data.data;
                       showLogin();
                   }
                   else {
                       response.data.data.images.forEach(function (d) {
                           if (d.changeset_no == undefined) {
                               d.changeset_no = 0;
                           }
                           $scope.gallery_files.push(d);
                       });
                       $scope.gallery_name = response.data.data.name;
                   }
                   if (callback != undefined) {
                       callback();
                   }
               });
           }

           $scope.$on('$locationChangeSuccess', function (event, newUrl, oldUrl) {
               if ($scope.isbackbuttonpress) {
                   if (window.location.hash.length == 0) {
                       showImageDetailsDialog(function (data) {
                           $scope.selectImage(data);
                       });
                   }
                   else {
                       window.location.hash = "#";
                   }
                   $scope.isbackbuttonpress = false;
               }
               else {
                   showImageDetailsDialog(function (data) {
                       $scope.selectImage(data);
                   });
               }
               
           });

           function showImageDetailsDialog(callback) {
               lastFileHash = "";
               var keys = {};
               window.location.hash.replace("#!#", "").split("&").forEach(function (d) {
                   var keyVal = d.split("=");
                   keys[keyVal[0]] = keyVal[1];
               });

               if (keys.file_hash != undefined) {
                   if (callback != undefined) callback(keys);
                   $("#imageViewDetails").modal("show");
               }
               else {
                   $("#imageViewDetails").modal("hide");
                   if (callback != undefined) callback();
                   $scope.selectedImage = undefined;
               }
           }
           $scope.showNextImage = function () {

               if ($scope.selectedImageIndex < $scope.gallery_files.length - 1) {
                   $scope.isbackbuttonpress = false;
                   window.location = window.location.href.replace("file_hash=" + $scope.selectedImage.file_hash, "file_hash=" + $scope.gallery_files[$scope.selectedImageIndex+1].file_hash);
               }
           };
           $scope.showPreviousImage = function () {
               if ($scope.selectedImageIndex > 0) {
                   $scope.isbackbuttonpress = false;
                   window.location = window.location.href.replace("file_hash=" + $scope.selectedImage.file_hash, "file_hash=" + $scope.gallery_files[$scope.selectedImageIndex - 1].file_hash);
               }
           };
           function fileIndex(file_hash) {
               var selectedImage = $scope.gallery_files.filter(function (d) { return d.file_hash == file_hash; })[0];
               return $scope.gallery_files.indexOf(selectedImage);
           }

           active();

       }]);

})();
