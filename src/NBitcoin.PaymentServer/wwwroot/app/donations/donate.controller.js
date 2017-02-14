(function () {
    'use strict';

    angular
        .module('app')
        .controller('DonateController', DonateController);

    DonateController.$inject = ['$location', 'donationService'];

    function DonateController($location, donationService) {
        /* jshint validthis:true */
        var vm = this;
        vm.amountMBtc = null;
        vm.email = null;
        vm.name = null;
        vm.title = 'Donate';
        //vm.message = '';
        vm.submit = submit;

        activate();

        function activate() { }

        function submit() {
            donationService.submitDonation(vm.name, vm.email, vm.amountMBtc)
                .then(submitComplete);

            function submitComplete(resultData) {
                var paymentPagePath = '/donations/payment/' + resultData.orderId;
                $location.path(paymentPagePath);
            }

        }
    }
})();
