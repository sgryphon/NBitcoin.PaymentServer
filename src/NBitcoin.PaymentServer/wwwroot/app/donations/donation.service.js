(function () {
    'use strict';

    angular
        .module('app')
        .factory('donationService', donationService);

    donationService.$inject = ['$log', '$http'];

    function donationService($log, $http) {
        var service = {
            checkStatus: checkStatus,
            getOrderDetails: getOrderDetails,
            submitDonation: submitDonation
        };

        return service;

        function checkStatus(orderId) {
            return $http.get('/api/donation/' + orderId + '/status')
                .then(checkStatusComplete)
                .catch(checkStatusFailed);

            function checkStatusComplete(response) {
                return response.data;
            }

            function checkStatusFailed(error) {
                $log.error('XHR Failed for checkStatus.' + error.data);
            }
        }

        function getOrderDetails(orderId) {
            return $http.get('/api/donation/' + orderId)
                .then(getOrderDetailsComplete)
                .catch(getOrderDetailsFailed);

            function getOrderDetailsComplete(response) {
                return response.data;
            }

            function getOrderDetailsFailed(error) {
                $log.error('XHR Failed for getOrderDetails.' + error.data);
            }
        }

        function submitDonation(paymentName, email, amountMBtc) {
            var dto = {
                paymentName: paymentName,
                email: email,
                amountMBtc: amountMBtc
            };
            return $http.post('/api/donation', dto)
                .then(submitDonationComplete)
                .catch(submitDonationFailed);

            function submitDonationComplete(response) {
                return response.data;
            }

            function submitDonationFailed(error) {
                $log.error('XHR Failed for submitDonation.' + error.data);
            }
        }
    }
})();