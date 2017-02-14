(function () {
    'use strict';

    angular
        .module('app')
        .controller('PaymentController', PaymentController);

    PaymentController.$inject = ['$location', '$interval', '$route', 'item', 'donationService'];

    function PaymentController($location, $interval, $route, item, donationService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'Payment';
        vm.order = item.order;
        vm.payment = item.payment;
        vm.status = null;
        vm.checked = null;

        var checkStatusPromise = null;
        var stopConfirmations = 10;

        activate();

        function activate() {
            checkStatusPromise = $interval(checkStatus, 2000);
            checkStatus();
        }

        function checkStatus() {
            var orderId = $route.current.params.orderId;
            if (orderId) {
                donationService.checkStatus($route.current.params.orderId)
                    .then(checkStatusComplete);
            }

            function checkStatusComplete(resultData) {
                vm.status = resultData.status;
                vm.checked = new Date();
                if (vm.status.confirmationLevel > stopConfirmations) {
                    $interval.cancel(checkStatusPromise);
                    checkStatusPromise = null;
                }
            }
        }
    }
})();
