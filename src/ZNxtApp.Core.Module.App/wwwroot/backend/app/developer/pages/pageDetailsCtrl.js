(function () {

    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.pageDetailsCtrl', ['$scope', '$controller', '$location', '$rootScope','$window', 'dataService', 'userData','loggerService',
    function ($scope, $controller, $location, $rootScope, $window, dataService, userData,logger) {
        
        $scope.page = {};
        var scrollX = 0;
        var scrollY;
        $scope.filterIncludeColumns = ["id", "file_path","module_name"];

        $scope.closeDetails = function () {
            $scope.$emit("onHidePageViewDetails", $scope.page);
            $window.scrollTo(scrollX, scrollY);
        }
        $scope.$on("onShowPageViewDetails", function (e, page) {
            $scope.page = page;
            scrollY = $window.scrollY;
            scrollX = $window.scrollX;
            $window.scrollTo(0, 0);
            $scope.getPageContent()
        });

        $scope.getPageContent = function () {
            var filePath = $scope.page.file_path;
            $scope.pageContent = "";
            if ($scope.page.file_path.indexOf("/backend/") == 0) {
                filePath = $scope.page.file_path.replace("/backend/", "./");
            }
            if ($scope.page.file_path.indexOf("/frontend/") == 0) {
                filePath = $scope.page.file_path.replace("/frontend/", "../");
            }
            
            if ($scope.getContentType() == "text") {

                dataService.get(filePath).then(function (response) {
                    console.log(response);
                    $scope.pageContent = response.data;
                    console.log($scope.pageContent);
                });
            }
            else if ($scope.getContentType() == "image") {
                $scope.file_url = filePath;
            }
        };

        $scope.getContentType = function () {
            
            if ($scope.page != undefined && $scope.page.content_type != undefined) {
                if ($scope.page.content_type.indexOf("text") == 0 || $scope.page.content_type.indexOf("javascript") != -1) {
                    return "text";
                }
                if ($scope.page.content_type.indexOf("image") == 0) {
                    return "image";
                }
                return "binary"
            }
            else {
                return "unknown";
            }
        }
        $scope.save = function () {

            var data = angular.copy($scope.page);
            data.data = $scope.pageContent;
            dataService.post("./api/content/update", data).then(function (response) {
                if (response.data.code == 1) {
                    logger.success("Successfully saved page");
                }
            });
        };
    }]);
})();