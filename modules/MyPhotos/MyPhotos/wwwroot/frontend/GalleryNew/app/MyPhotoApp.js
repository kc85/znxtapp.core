var __myPhotoAppName = 'MyPhotoApp';

var __userData = {};
(function () {
    var MyPhotoApp = angular.module(__myPhotoAppName, ['infinite-scroll']);
   
    MyPhotoApp.controller(__myPhotoAppName + '.Main', ['$scope', '$location', '$rootScope','$window', '$http', '$timeout','fileUploadService',
       function ($scope, $location, $rootScope, $window, $http, $timeout, fileUploadService) {
           $scope.busy = false;
           $scope.pagesize = 50;
           $scope.currentpage = 0;
           $scope.user = undefined;
           $scope.userLoginRequired = false;
           $scope.loading = false;
           $scope.selectedImageChangesetNo = 0;
           $scope.selectedImageIndex = 0;
           $scope.isbackbuttonpress = true;
           $scope.showImageTopToolbar = false;
           $scope.galleryid = GetParameterValues("galleryid");
           $scope.isShowBookmark = GetParameterValues("bookmark");
           if ($scope.galleryid == undefined && $scope.isShowBookmark == undefined) {
               window.location = "./indexnew.z";
           }
           $scope.gallery = undefined
           $scope.gallery_name = "";
           $scope.screenHeight = 0;
           $scope.images = [];
           $scope.gallery_files = [];
           function active() {
               $scope.screenHeight = $window.innerHeight + 30;
               $scope.isbackbuttonpress = false;
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
               var url = "../api/user/me";
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
               if ($scope.busy != true) {
                   $scope.busy = true;
                   var url = "../api/myphotos/image/rotate?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid;
                   $scope.rotateText = "...";
                   $http.post(url).then(function (response) {
                       $scope.busy = false;
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
                   $scope.galleryid = image.galleryid;
                   $scope.selectedImage = image;
                   setShareLink();
                   $scope.selectedImageIndex = fileIndex(image.file_hash);
                   $scope.selectedImagedata = $scope.gallery_files[$scope.selectedImageIndex];
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

           function setShareLink() {
               if ($scope.gallery != undefined &&  $scope.gallery.thumbnail_image!=undefined && $scope.gallery.thumbnail_image!=undefined) {
                   var galleryshareurl = "https://znxt.app/gallerynew/share.z?thumbnail_image=" + $scope.gallery.thumbnail_image.file_hash + "&galleryid=" + $scope.galleryid;
                   $scope.galleryShareLink = galleryshareurl;
                   $scope.shareText = $scope.gallery.display_name;
               }
               if ($scope.selectedImage != undefined) {
                   var url = "https://znxt.app/gallerynew/share.z?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid;
                   $scope.shareLink = url;
               }
           }
           function fetchImageDetails(callback) {
               var url = "../api/myphotos/fetch?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid + "&pagesize=10";
               $http.get(url).then(function (response) {
                   setShareLink();
                   
                   if (response.data.code == 401) {
                       $scope.user = undefined;
                       window.location.hash = "#";

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
               $scope.gallery = {};
               var url = "../api/myphotos/gallery?id=" + $scope.galleryid + "&currentpage=" + $scope.currentpage + "&pagesize=" + $scope.pagesize;
               if ($scope.isShowBookmark != undefined) {
                   if ($scope.gallery_files.length != 0) {
                       return;
                   }
                   url = "../api/myphotos/user/bookmark";

               }
               $http.get(url).then(function (response) {

                   if (response.data.code == 401) {
                       $scope.user = undefined;
                       $scope.userLoginRequired = true;
                       $scope.busy = false;
                   }
                   else {
                       $scope.gallery = response.data.data;
                       if ($scope.isShowBookmark == undefined && $scope.gallery.thumbnail_image!=undefined &&  $scope.gallery.thumbnail_image.changeset_no == undefined) {
                           $scope.gallery.thumbnail_image.changeset_no = 0;
                       }
                       response.data.data.images.forEach(function (d) {
                           if (d.changeset_no == undefined) {
                               d.changeset_no = 0;
                           }
                           if (d.galleryid == undefined) {
                               d.galleryid = $scope.galleryid;
                           }
                           $scope.gallery_files.push(d);
                       });
                       $scope.gallery.images = undefined;
                       setShareLink();
                   }
                   if (callback != undefined) {
                       callback();
                   }
               });
           }

           $scope.isOwner = function () {
               if ($scope.user != undefined && $scope.gallery!=undefined) {
                   if ($scope.user.user_id == $scope.gallery.owner) {
                       return true;
                   }
                   else if ($scope.user.groups.indexOf($scope.gallery.owner) != -1) {
                       return true;
                   }

               }
               return false;
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
               var keys = getUrlHashKeys();
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
                   window.location.replace(window.location.href.replace("file_hash=" + $scope.selectedImage.file_hash, "file_hash=" + $scope.gallery_files[$scope.selectedImageIndex+1].file_hash));
               }
           };
           $scope.showPreviousImage = function () {
               if ($scope.selectedImageIndex > 0) {
                   $scope.isbackbuttonpress = false;
                   window.location.replace(window.location.href.replace("file_hash=" + $scope.selectedImage.file_hash, "file_hash=" + $scope.gallery_files[$scope.selectedImageIndex - 1].file_hash));
               }
           };
           function fileIndex(file_hash) {
               var selectedImage = $scope.gallery_files.filter(function (d) { return d.file_hash == file_hash; })[0];
               return $scope.gallery_files.indexOf(selectedImage);
           }
           $scope.likeImage = function () {

               if ($scope.loading != true) {
                   $scope.loading = true;
                   var url = "../api/myphotos/image/like?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid;
                   $http.post(url).then(function (response) {                       
                       $scope.loading = false;
                       if (response.data.data.count > 0) {
                           $scope.selectedImagedata.likes_count++;
                           $scope.selectedImagedata.is_liked = true;
                       }
                       else {
                           $scope.selectedImagedata.is_liked = false;
                           $scope.selectedImagedata.likes_count--;
                       }
                       
                   });
               };

           };

           $scope.bookmarkImage = function () {

               if ($scope.loading != true) {
                   $scope.loading = true;
                   var url = "../api/myphotos/image/bookmark?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid;
                   $http.post(url).then(function (response) {
                       removeCacheKey("/api/myphotos/user/bookmark");
                       $scope.loading = false;
                       if (response.data.data.count > 0) {                           
                           $scope.selectedImagedata.is_bookmarked = true;
                       }
                       else {
                           $scope.selectedImagedata.is_bookmarked = false;
                       }
                   });
               };
           };

           $scope.$watch('selectedUploadImage', function () {
               ImageUploader();
           });
           function ImageUploader() {

               if ($scope.selectedUploadImage != undefined) {
                   if ($scope.busy != true) {
                       $scope.busy = true;
                       $scope.uploadimagetext = "Uploading image";
                       var uploadImageUrl = "./api/myphotos/gallery/addimage?galleryid=" + $scope.galleryid;
                       if ($scope.selectedUploadImage.type.indexOf("image") != -1) {

                           fileUploadService.uploadFileToUrl($scope.uploadFiles, uploadImageUrl, undefined, function (response) {
                               $scope.uploadFiles = [];
                               $scope.busy = false;
                               $scope.uploadimagetext = "";
                               if (response.data.code == 1) {
                                   $scope.gallery_files = [];
                                   removeCacheKey("/api/myphotos/gallery", function () {
                                       $scope.currentpage = 0;
                                       $scope.loadMore();
                                   });
                                   
                               }
                           }, function () {
                               $scope.busy = false;
                               $scope.uploadFiles = [];
                               $scope.uploadimagetext = "";
                           });
                       }
                       else {
                           alert("Invalid file selection. Please select image file");
                       }
                   }
               }
           }

           $scope.deleteImage = function () {
               if (confirm("Are you sure to delete image ?")) {
                   if ($scope.loading != true) {
                       $scope.loading = true;
                       var url = "../api/myphotos/gallery/deleteimage?file_hash=" + $scope.selectedImage.file_hash + "&galleryid=" + $scope.galleryid;
                       $http.post(url).then(function (response) {
                           $scope.loading = false;
                           if (response.data.code == 1) {
                              $scope.closeImageDetail();
                               var index = fileIndex($scope.selectedImage.file_hash);
                               $scope.gallery_files.splice(index, 1);
                           }
                           else {
                               alert("Error");
                           }
                       });
                   };
               }
           }

           $scope.setGalleryThumb = function () {
               if (confirm("Are you sure to set this image as gallery home ?")) {
                   var data = {};
                   data.thumbnail = $scope.selectedImage.file_hash;
                   var url = "../api/myphotos/gallery/update?galleryid=" + $scope.galleryid;
                   $http.post(url, data).then(function (response) {
                       if (response.data.code == 1) {
                           console.log(response.data)
                           alert("Applied");
                       }
                   });
               }
           };

           active();

       }]);

})();
