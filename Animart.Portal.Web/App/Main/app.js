﻿(function () {
    'use strict';

    var app = angular.module('app', [
         'ngAnimate',
         'ngSanitize',
         'ui.grid',
         'ui.grid.edit',
         'ui.grid.rowEdit',
         'ui.grid.cellNav',
         'ui.grid.selection',
         'ngMaterial',
         'chart.js',
         'ui.router',
         'ui.bootstrap',
         'ui.jq',
         'abp',
         'ngCart',
         'ngRoute'
    ]);

    //Configuration for Angular UI routing.
    app.config([
        '$stateProvider', '$urlRouterProvider',
        function ($stateProvider, $urlRouterProvider) {
            $urlRouterProvider.otherwise('/');
            $stateProvider
                .state('home', {
                    url: '/',
                    templateUrl: '/App/Main/views/home/home.cshtml',
                    menu: 'Home' //Matches to name of 'Home' menu in PortalNavigationProvider
                })
                .state('item', {
                    url: '/item/:id',
                    templateUrl: '/App/Main/views/home/item.cshtml'
                })
                .state('checkout', {
                     url: '/checkout',
                     templateUrl: '/App/Main/views/retailer/checkout.cshtml'
                })
                .state('supply', {
                    url: '/supply',
                    templateUrl: '/App/Main/views/supply/supply.cshtml',
                    menu: 'Supply' //Matches to name of 'Supply' menu in PortalNavigationProvider
                }).state('users', {
                    url: '/users',
                    templateUrl: '/App/Main/views/admin/userManagement.cshtml',
                    menu: 'Users' //Matches to name of 'User' menu in PortalNavigationProvider
                }).state('expedition', {
                    url: '/expedition',
                    templateUrl: '/App/Main/views/admin/expeditionManagement.cshtml',
                    menu: 'Expedition' //Matches to name of 'Expedition' menu in PortalNavigationProvider
                }).state('cities', {
                    url: '/cities',
                    templateUrl: '/App/Main/views/admin/cityManagement.cshtml',
                    menu: 'Cities' //Matches to name of 'City' menu in PortalNavigationProvider
                }).state('retailerDashboard', {
                    url: '/retailerDashboard',
                    templateUrl: '/App/Main/views/retailer/retailer-dashboard.cshtml',
                    menu: 'RetailerDashboard' //Matches to name of 'City' menu in PortalNavigationProvider
                }).state('marketingDashboard', {
                    url: '/marketingDashboard',
                    templateUrl: '/App/Main/views/marketing/marketing-dashboard.cshtml',
                    menu: 'marketingDashboard' //Matches to name of 'City' menu in PortalNavigationProvider
                }).state('logisticDashboard', {
                    url: '/logisticDashboard',
                    templateUrl: '/App/Main/views/logistic/logistic-dashboard.cshtml',
                    menu: 'LogisticDashboard' //Matches to name of 'City' menu in PortalNavigationProvider
                }).state('accountingDashboard', {
                    url: '/accountingDashboard',
                    templateUrl: '/App/Main/views/accounting/accounting-dashboard.cshtml',
                    menu: 'AccountingDashboard' //Matches to name of 'City' menu in PortalNavigationProvider
                });
        }
    ]);
})();