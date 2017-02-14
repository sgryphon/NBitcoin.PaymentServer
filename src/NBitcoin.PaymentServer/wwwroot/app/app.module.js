(function () {
    'use strict';

    angular.module('app', [
        // Angular modules 
        'ngRoute'

        // Custom modules 

        // 3rd Party Modules
        
    ]);

    // Work around to get ng-view working inside an ng-include
    // See: https://github.com/angular/angular.js/issues/1213
    angular.module('app').run(['$route', angular.noop]);
})();