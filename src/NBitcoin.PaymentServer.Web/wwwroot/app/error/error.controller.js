(function () {
    'use strict';

    angular
        .module('app')
        .controller('ErrorController', ErrorController);

    HomeController.$inject = ['$location'];

    function ErrorController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'Error';
        vm.message = 'Unrecognised URL';

        activate();

        function activate() { }
    }
})();
