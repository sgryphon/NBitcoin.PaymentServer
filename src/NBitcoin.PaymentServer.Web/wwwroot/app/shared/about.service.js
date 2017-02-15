(function () {
    'use strict';

    angular
        .module('app')
        .factory('aboutService', aboutService);

    aboutService.$inject = ['$log', '$http'];

    function aboutService($log, $http) {
        var service = {
            getAbout: getAbout
        };

        return service;

        function getAbout() {
            return $http.get('/api/about')
                .then(getAboutComplete)
                .catch(getAboutFailed);

            function getAboutComplete(response) {
                return response.data;
            }

            function getAboutFailed(error) {
                $log.error('XHR Failed for checkStatus.' + error.data);
            }
        }
    }
})();