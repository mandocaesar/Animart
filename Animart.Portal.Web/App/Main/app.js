(function () {
    'use strict';

    var app = angular.module('app', [
        'ngAnimate',
        'ngSanitize',
         'ui.grid',
         'ui.grid.edit',
         'ui.grid.rowEdit',
         'ui.grid.cellNav',
         'ui.grid.selection',
        'ui.router',
        'ui.bootstrap',
        'ui.jq',

        'abp'
    ]);

    //Configuration for Angular UI routing.
    app.config([
        '$stateProvider', '$urlRouterProvider',
        function($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/');
            $stateProvider
                .state('home', {
                    url: '/',
                    templateUrl: '/App/Main/views/home/home.cshtml',
                    menu: 'Home' //Matches to name of 'Home' menu in PortalNavigationProvider
                })
                .state('about', {
                    url: '/about',
                    templateUrl: '/App/Main/views/about/about.cshtml',
                    menu: 'About' //Matches to name of 'About' menu in PortalNavigationProvider
                }).state('supply', {
                    url: '/supply',
                    templateUrl: '/App/Main/views/supply/supply.cshtml',
                    menu: 'Supply' //Matches to name of 'Supply' menu in PortalNavigationProvider
                }).state('users', {
                    url: '/users',
                    templateUrl: '/App/Main/views/admin/userManagement.cshtml',
                    menu: 'Users' //Matches to name of 'User' menu in PortalNavigationProvider
                });
        }
    ]);
})();