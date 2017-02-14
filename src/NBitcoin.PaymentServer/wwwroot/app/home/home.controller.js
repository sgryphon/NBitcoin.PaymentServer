(function () {
    'use strict';

    angular
        .module('app')
        .controller('HomeController', HomeController);

    HomeController.$inject = ['$location'];

    function HomeController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'Home';

        activate();

        function activate() { }
    }
})();
