angular
    .module('manager')
    .controller('TableController', ['$scope', '$http', function ($scope, $http) {

        $scope.maxShowPages = 7;
        //$scope.params.pageSize = 10;
        $scope.pageSizeList = [5, 10, 20, 50];
        $scope.beforeFetch = function () { };

        $scope.onPageChange = function () {
            $scope._fetch();
        };

        $scope.onPageSizeChange = function (size) {
            $scope.params.pageSize = size;
            $scope._fetch();
        };

        $scope.updateModel = function (method, url) {
            $scope.method = method;
            $scope.url = url;
        };

        $scope.open = function (raw) {
            $scope.raw = raw;
            $scope.showModal = true;
        };

        $scope.save = function () {
            $scope.params.raw = JSON.stringify($scope.raw);
            $scope._update();
            $scope.showModal = false;
        };

        $scope.delete = function (raw) {
            $scope.params.raw = JSON.stringify(raw);
            $scope._delete();
            $scope.showModel = false;
        };

        $scope.cancel = function () {
            $scope.showModal = false;
        };

        $scope._fetch = function () {
            $http.post('/api/demo/table',
                { "Method": 0, "Parameters": [{ "Name": "pageSize", "Value": "50" }] }
            ).then(function (response) {
                $scope.data = response.data;
            }, function () {
            });
        };

        $scope._fetch();
    }]);