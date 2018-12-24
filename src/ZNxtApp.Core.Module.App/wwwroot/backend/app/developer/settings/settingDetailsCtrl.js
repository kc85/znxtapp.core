(function () {
    var ZApp = angular.module(__ZNxtAppName);

    ZApp.controller(__ZNxtAppName + '.settingDetailsCtrl', ['$scope', '$controller', '$location', '$rootScope', '$window', 'dataService', 'userData',
    function ($scope, $controller, $location, $rootScope, $window, dataService, userData) {
        $scope.setting = {};
        var scrollX = 0;
        var scrollY;
        $scope.closeDetails = function () {
            $scope.$emit("onHideSettingViewDetails", $scope.setting);
            $window.scrollTo(scrollX, scrollY);
        }
        $scope.$on("onShowSettingViewDetails", function (e, setting) {
            $scope.setting = angular.copy(setting);
            if ($scope.setting.is_enabled == undefined) {
                $scope.setting.is_enabled = !$scope.setting.is_override;
            }
            scrollY = $window.scrollY;
            scrollX = $window.scrollX;
            $window.scrollTo(0, 0);
        });
        $scope.save = function () {
            dataService.post("./api/admin/setting/update", $scope.setting).then(function (response) {
                if (response.data.code == 1) {
                    logger.success("Successfully saved setting");
                    $scope.$emit("onSettingUpdate", $scope.setting);
                }
            });
        };

        $scope.getDataType = function (setting) {
            return typeof setting.data;
        };
        function isBoolean(arg) {
            return typeof arg === 'boolean';
        }

        function isNumber(arg) {
            return typeof arg === 'number';
        }

        function isString(arg) {
            return typeof arg === 'string';
        }

        function isFunction(arg) {
            return typeof arg === 'function';
        }
        function isObject(arg) {
            return arg !== null && typeof arg === 'object';
        }
    }]);
})();