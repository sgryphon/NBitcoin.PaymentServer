(function () {
    'use strict';

    angular
        .module('app')
        .controller('ReceivePaymentController', ReceivePaymentController);

    ReceivePaymentController.$inject = ['$location', '$interval', '$route', 'item', 'paymentService'];

    function ReceivePaymentController($location, $interval, $route, item, paymentService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'Payment';
        vm.payment = item.paymentDetail;
        vm.status = null;
        vm.checked = null;

        var checkStatusPromise = null;
        var stopConfirmations = 10;

        activate();

        function activate() {
            checkStatusPromise = $interval(checkStatus, 30000);
            checkStatus();
        }

        function checkStatus() {
            var gatewayId = $route.current.params.gatewayId;
            var paymentId = $route.current.params.paymentId;
            if (gatewayId && paymentId) {
                paymentService.checkPaymentStatus(gatewayId, paymentId)
                    .then(checkPaymentStatusComplete);
            }

            function checkPaymentStatusComplete(resultData) {
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
