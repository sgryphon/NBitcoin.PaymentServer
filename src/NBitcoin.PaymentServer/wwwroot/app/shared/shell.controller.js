//(function () {
//    'use strict';

    angular
        .module('app')
        .controller('ShellController', ShellController);

    ShellController.$inject = ['$route', '$routeParams', '$location', 'aboutService'];

    function ShellController($route, $routeParams, $location, aboutService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'Shell';
        vm.informationalVersion = null;

        vm.$route = $route;
        vm.$location = $location;
        vm.$routeParams = $routeParams;

        activate();

        function activate() {
            aboutService.getAbout()
                .then(getAboutComplete);

            function getAboutComplete(resultData) {
                vm.informationalVersion = 'v' + resultData.informationalVersion;
            }
        }
    }
//})();
