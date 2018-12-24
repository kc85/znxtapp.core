(function () {
    var ZApp = angular.module(__ZNxtAppName);
    ZApp.controller(__ZNxtAppName + '.gridBaseCtrl', ['$scope', '$location', '$rootScope', 'dataService', 'userData',
    function ($scope, $location, $rootScope, dataService, userData) {
        $scope.filter = "";
        $scope.pageSize = 10;
        $scope.currentPage = 1;
        $scope.isError = false;
        $scope.errorMessage = "";
        $scope.filterCoumns = [];
        $scope.filterIncludeColumns = ["type", "message", "transaction_id"];
        $scope.loadingData = false;
        var removeFiltyerColumn = [];

        $scope.filterChanged = function () {
            $scope.currentPage = 1;
            removeFiltyerColumn = [];
            $scope.active();
        }
        $scope.getFilter = function () {
            var filterText = "{}";
            $scope.filterCoumns = [];
            if ($scope.filter.length != 0) {
                var filters = [];
                $scope.filterIncludeColumns.forEach(function (d) {
                    if (removeFiltyerColumn.filter(function (f) { return f.column == d }).length == 0) {
                        filters.push("{" + d + ":{$regex : '.*" + $scope.filter + "*.','$options' : 'i'}}");
                        $scope.filterCoumns.push({ "column": d, "value": $scope.filter });
                    }
                });
                if (filters.length != 0) {
                    filterText = "{$or : [ " + filters.join(",") + "]}";
                }
                else {
                    $scope.filter = "";
                }
            }
            return filterText;
        }
        $scope.next = function () {
            if ($scope.currentPage < $scope.pageData.TotalPages) {
                $scope.currentPage++;
                $scope.active();
            }
        }
        $scope.gotoPage = function (pageNo) {
            if (pageNo > 0 && pageNo < $scope.pageData.TotalPages + 1) {
                $scope.currentPage = pageNo;
                $scope.active();
            }
            else {
                alert("Invalid page number");
            }
        }
        $scope.previous = function () {
            if ($scope.currentPage > 1) {
                $scope.currentPage--;
                $scope.active();
            }
        }
        $scope.removeFilter = function (filterColumn) {
            removeFiltyerColumn.push(filterColumn);
            $scope.active();
        }
        $scope.clearFilter = function () {
            removeFiltyerColumn = [];
            $scope.filter = "";
            $scope.active();
        }
        $scope.range = function (min, max, step) {
            step = step || 1;
            var input = [];
            for (var i = min; i <= max; i += step) {
                input.push(i);
            }
            return input;
        };
        $scope.getMaxPageCount = function (uicount) {
            return Math.min(uicount, $scope.pageData.TotalPages);
        };
        $scope.pageSizeChanged = function () {
            $scope.active();
        }
    }]);
})();