﻿angular.module('app').controller('app.views.bodDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal',
    dashboardController
]);

function dashboardController($q, $rootScope, $scope, orderService, $uibModal) {
    orderService.GetDashboard().success(function(result) {
        $scope.dashboard = result;
    });

}