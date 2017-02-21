(function () {
    'use strict';

    angular.module('app').config(['$locationProvider', '$routeProvider',
        function config($locationProvider, $routeProvider) {
            $locationProvider.hashPrefix('!');

            $routeProvider
              .when('/gateway/:gatewayId/payment', {
                  templateUrl: 'app/gateway/payment.html',
                  resolve: {
                      item: ['$route', 'paymentService', function ($route, paymentService) {
                          var item = paymentService.getGateway($route.current.params.gatewayId);
                          return item;
                      }]
                  },
                  controller: 'PaymentController',
                  controllerAs: 'vm'
              })
              .when('/gateway/:gatewayId/payment/:paymentId', {
                  templateUrl: 'app/gateway/receivePayment.html',
                  resolve: {
                      item: ['$route', '$routeParams', 'paymentService', function ($route, $routeParams, paymentService) {
                          var item = paymentService.getPayment($route.current.params.gatewayId, $route.current.params.paymentId);
                          return item;
                      }]
                  },
                  controller: 'ReceivePaymentController',
                  controllerAs: 'vm'
              })
              .otherwise({ redirectTo: '/error' });
        }
    ]);
})();