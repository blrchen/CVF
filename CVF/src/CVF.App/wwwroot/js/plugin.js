var $enums = $enums || {};
(function (enums) {
    enums.items = [];

    enums.find = function (name, value) {
        var item = enums.items.find(function (i) {
            return i.name == name;
        });

        if (typeof (value) == 'undefined') {
            return item;
        }

        if (!item) {
            return value;
        }

        let result = item.items.find(function (ele) {
            return ele.value === value || ele.name === value;
        });

        return result;
    };
})($enums || ($enums = {}));

$groupCache = {};
// var $plugins = null;
var $pluginApp = angular.module("pluginApp", ['ui.bootstrap', 'chart.js', 'ngRoute', 'gc.bootstrap.modal'])
    .filter('enums', function () {
        return function (input, enumName) {
            var item = $enums.find(enumName, input);
            return item ? item.displayName : input;
        }
    }).filter('group', function () {
        return function (input, length) {
            if (!input) {
                return null;
            }

            var key = [];
            angular.forEach(input, function (v) {
                key.push(v.name);
            });
            key = key.join('-') + "-" + length;
            if ($groupCache[key]) {
                return $groupCache[key];
            }

            var rows = [];
            var row = [];
            if (input.length < length) {
                rows = [input];
            } else {
                for (var i = 0; i < input.length; i++) {
                    if (i % length == 0) {
                        row = [];
                        rows.push(row);
                    }

                    row.push(input[i]);
                }
            }

            $groupCache[key] = rows;
            return rows;
        };
    }).service('pluginService', ['$http', '$location', function ($http, $location) {
        var find = function (items, name) {
            for (var i = items.length - 1; i >= 0; i--) {
                if (items[i].name == name) {
                    return items[i];
                }
            }

            return items[0];
        }

        var observerCallbacks = [];

        this.plugin = $plugin;
        //register an observer
        this.registerObserverCallback = function (callback) {
            observerCallbacks.push(callback);
        };

        //call this when you know 'foo' has been changed
        var notifyObservers = function () {
            angular.forEach(observerCallbacks, function (callback) {
                callback();
            });
        };

        this.setCurrent = function (route) {
            var regex = /\/category\/([^\/]*)(\/item\/([^\/]*))?/;
            var match = regex.exec(route);
            if (match != null) {
                var categoryName = match[1];
                var itemName = match[3];

                this.category = find(this.plugin.categories, categoryName);
                if (this.category && itemName) {
                    this.item = find(this.category.items, itemName);
                } else {
                    this.item = null;
                }
            } else {
                this.category = null;
                this.item = null;
            }
        };

        this.init = function (route) {
            this.setCurrent(route);
            notifyObservers();
        };
    }])
    .controller('baseController', ['$scope', '$http', '$location', '$templateCache', 'pluginService', function ($scope, $http, $location, $templateCache, pluginService) {
        $scope.$enums = $enums;

        function updateScope() {
            $scope.plugins = pluginService.plugins;
            $scope.plugin = pluginService.plugin;
            $scope.category = pluginService.category;
            $scope.item = pluginService.item;
        }

        updateScope();

        pluginService.registerObserverCallback(updateScope);
    }]).controller('navController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });
        $scope.plugins = pluginService.plugins;
        $scope.plugin = pluginService.plugin;
        $scope.setCategory = function (c) {
            pluginService.category = c;
        };

        $scope.setItem = function (item) {
            pluginService.item = item;
        };

    }]).controller('pluginController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });
    }]).controller('categoryController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });
    }]).controller('apiController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('baseController', { $scope: $scope });

        var loading = {
            'fetch': '正在加载',
            'update': '正在更新',
            'delete': '正在删除',
            'done': null,
        };

        $scope.params = {};
        $scope.beforeFetch = function () { };

        var post = function (method, callback) {
            $scope.code = null;
            $scope.response = null;
            var params = [];
            for (var n in $scope.params) {
                if ($scope.params[n]) {
                    params.push(n + "=" + $scope.params[n]);
                }
            }

            $scope.error = null;

            $http.post(
                '/api/plugins',
                {
                    pluginName: $scope.plugin.name,
                    categoryName: $scope.category ? $scope.category.name : null,
                    itemName: $scope.item.name,
                    method: method,
                    content: $scope.params
                },
                {
                    responseType: 'json',
                    // cache: $templateCache,
                    timeout: 10000
                }).
                then(function (response) {
                    $scope.loadingText = loading.done;
                    $scope.status = response.status;
                    $scope.data = response.data;
                    console.log(response);
                    if (callback) {
                        callback(true, response);
                    }
                }, function (response) {
                    $scope.loadingText = loading.done;
                    console.log(response);
                    $scope.error = {
                        status: response.status,
                        message: response.data || (response.status == "-1" ? "请求超时，服务器没有在有效时间内做出相应" : "发生未知错误")
                    };

                    if (callback) {
                        callback(false, response);
                    }
                });
        }

        $scope._hideError = function () {
            $scope.error = null;
        };

        $scope._fetch = function (callback) {
            $scope.loadingText = loading.fetch;
            post('Read', callback);
        };

        $scope._update = function (callback) {
            $scope.loadingText = loading.update;
            post('Update', callback);
        };

        $scope._delete = function (callback) {
            $scope.loadingText = loading.delete;
            post('Delete', callback);
        };

        $scope._sync = function (callback) {
            if (!$scope.item) {
                return;
            }

            var interval = $scope.item.refreshInterval || $scope.plugin.refreshInterval;
            if (interval > 0) {
                var _callback = function (isSuccess, response) {
                    if (callback) {
                        callback(isSuccess, response);
                    }

                    setTimeout(function () {
                        $scope._fetch(_callback);
                    },
                    interval);
                }

                $scope._fetch(_callback);
            } else {
                $scope._fetch(callback);
            }
        };
    }]).controller('pluginsController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });
    }]).controller('tableController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });
        $scope.maxShowPages = 7;
        $scope.params.pageSize = 10;
        $scope.pageSizeList = [5, 10, 20, 50];
        $scope.beforeFetch = function () { };

        $scope.onPageChange = function () {
            $scope._fetch();
        }

        $scope.onPageSizeChange = function (size) {
            $scope.params.pageSize = size;
            $scope._fetch();
        }

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

        $scope._sync();
    }])
    .controller('formController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });

        $scope.updateData = function () {
            $scope.params.data = JSON.stringify($scope.data);
            $scope._update();
        }

        // auto sync should only on table & report type plugin.
        $scope._fetch();
    }])
    .controller('chartController', ['$scope', '$http', '$templateCache', '$controller', 'pluginService', function ($scope, $http, $templateCache, $controller, pluginService) {
        $controller('apiController', { $scope: $scope });
        $scope._sync();
    }])
    .config(['$routeProvider', function ($routeProvider) {
        if ($plugin) {
            if ($plugin.categories) {
                $plugin.categories.forEach(function (c) {
                    $routeProvider.when('/category/' + c.name, {
                        templateUrl: '/plugins/_templates/views/category.html',
                        controller: 'categoryController'
                    });

                    if (c.items) {
                        c.items.forEach(function (i) {
                            $routeProvider.when('/category/' + c.name + '/item/' + i.name, {
                                templateUrl: '/plugins/_templates/views/' + i.type + '.html',
                                controller: i.controller
                            });
                        });
                    }
                });
            }
        }

        $routeProvider.otherwise({
            templateUrl: '/plugins/_templates/views/plugin.html',
            controller: 'pluginController'
        });
    }])
    .run(['$rootScope', '$location', '$http', 'pluginService', function ($rootScope, $location, $http, pluginService) {
        $rootScope.$on('$routeChangeStart', function (next, current) {
            pluginService.init($location.path());
        });

        pluginService.init($location.path());
    }]);
