(function () {
    'use strict';

    angular
        .module('app')
        .controller('PaymentController', PaymentController);

    PaymentController.$inject = ['$location', '$route', 'item', 'paymentService'];

    function PaymentController($location, $route, item, paymentService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'Payment';
        vm.amount = $route.current.params.amount;
        vm.currency = $route.current.params.currency;
        vm.reference = $route.current.params.reference;
        vm.memo = $route.current.params.memo;
        vm.gateway = item;
        vm.confirm = confirm;

        activate();

        function activate() {
        }

        function confirm() {
            paymentService.submitPaymentRequest(vm.gateway.id, vm.amount, vm.currency, vm.reference, vm.memo)
                .then(submitComplete);

            function submitComplete(resultData) {
                var paymentPagePath = '/gateways/' + resultData.gatewayId
                    + '/payments/' + resultData.paymentId;
                $location.path(paymentPagePath);
            }
        }
    }
})();
