angular
    .module('manager')
    .controller('ChartController', ['$scope', '$http', function ($scope, $http) {
        $http.post('/api/demo/chart',
            { "Method": 0, "Parameters": [{  }] }
        ).then(function (response) {
            $scope.data = response.data;
        }, function () {
        });
    }]);