(function () {
    'use strict';

    angular
        .module('app')
        .factory('paymentService', paymentService);

    paymentService.$inject = ['$log', '$http'];

    function paymentService($log, $http) {
        var service = {
            checkPaymentStatus: checkStatus,
            getGateway: getGateway,
            getPaymentStatus: getPaymentStatus,
            submitPaymentRequest: submitPaymentRequest
        };

        return service;

        function checkPaymentStatus(gatewayId, paymentId) {
            return $http.get('/api/gateway/' + gatewayId + '/payment/' + paymentId + '/status')
                .then(checkStatusComplete)
                .catch(checkStatusFailed);

            function checkStatusComplete(response) {
                return response.data;
            }

            function checkStatusFailed(error) {
                $log.error('XHR Failed for checkStatus.' + error.data);
            }
        }

        function getGateway(gatewayId) {
            return $http.get('/api/gateway/' + gatewayId)
                .then(getGatewayComplete)
                .catch(getGatewayFailed);

            function getGatewayComplete(response) {
                return response.data;
            }

            function getGatewayFailed(error) {
                $log.error('XHR Failed for getGateway.' + error.data);
            }
        }

        function getPayment(gatewayId, paymentId) {
            return $http.get('/api/gateway/' + gatewayId + '/payment/' + paymentId)
                .then(getPaymentComplete)
                .catch(getPaymentFailed);

            function getPaymentComplete(response) {
                return response.data;
            }

            function getPaymentFailed(error) {
                $log.error('XHR Failed for getPayment.' + error.data);
            }
        }

        function submitPaymentRequest(gatewayId, amount, currency, reference, memo) {
            var dto = {
                gatewayId: gatewayId,
                amount: amount,
                currency: currency,
                reference: reference,
                memo: memo
            };
            return $http.post('/api/payment', dto)
                .then(submitPaymentRequestComplete)
                .catch(submitPaymentRequestFailed);

            function submitPaymentRequestComplete(response) {
                return response.data;
            }

            function submitPaymentRequestFailed(error) {
                $log.error('XHR Failed for submitPaymentRequest.' + error.data);
            }
        }
    }
})();