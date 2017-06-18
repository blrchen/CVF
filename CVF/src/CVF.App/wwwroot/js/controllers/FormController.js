angular
    .module('manager')
    .controller('FormController', ['$scope', '$http', function ($scope, $http) {
        $http.get('/api/demo/form')
            .then(function (response) {
                $scope.data = response.data;
            }, function () {
            });

        $scope.updateData = function () {
            $http.post('/api/demo/form', JSON.stringify($scope.data))
                .then(function (response) {
                    $scope.data = response.data;
                }, function () {
                })
        }
    }]);