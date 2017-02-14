(function () {
    'use strict';

    angular.module('app').config(['$locationProvider', '$routeProvider',
        function config($locationProvider, $routeProvider) {
            $locationProvider.hashPrefix('!');

            $routeProvider
              .when('/home', {
                  templateUrl: 'app/home/home.html',
                  controller: 'HomeController',
                  controllerAs: 'vm'
              })
              .when('/donations/donate', {
                  templateUrl: 'app/donations/donate.html',
                  controller: 'DonateController',
                  controllerAs: 'vm'
              })
              .when('/donations/payment/:orderId', {
                  templateUrl: 'app/donations/payment.html',
                  resolve: {
                      item: ['$route', '$routeParams', 'donationService', function ($route, $routeParams, donationService) {
                          var item = donationService.getOrderDetails($route.current.params.orderId);
                          return item;
                      }]
                  },
                  controller: 'PaymentController',
                  controllerAs: 'vm'
              })
              .otherwise({ redirectTo: '/home' });
        }
    ]);
})();