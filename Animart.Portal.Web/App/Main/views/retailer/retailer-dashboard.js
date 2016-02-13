angular.module('app').controller('app.views.retailerDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal','$mdSidenav','$mdDialog',
    dashboardController
]).controller('LeftCtrl', ['$q', '$rootScope', '$scope', 'abp.services.app.shipment', 'abp.services.app.order', '$uibModal', '$mdSidenav', '$mdDialog',
   leftController]);

function dashboardController($q, $rootScope, $scope, orderService, $uibModal, $mdSidenav,$mdDialog) {
    orderService.getDashboard().success(function (result) {
        $scope.dashboard = result;
    });
};

function leftController($q, $rootScope, $scope, expeditonService, orderService, $uibModal, $mdSidenav, $mdDialog) {
    $scope.close = function() {
        $mdSidenav('left').close()
            .then(function() {
                $log.debug("close LEFT is done");
            });
    };

    expeditonService.getCities().success(function (result) {
        console.log(result);
        $scope.cities = result;
    });

    $scope.addPO = function (ev) {
        $mdDialog.show({
            templateUrl: 'order.tmpl.html',
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: true
        })
            .then(function(answer) {
                $scope.status = 'You said the information was "' + answer + '".';
            }, function() {
                $scope.status = 'You cancelled the dialog.';
            });
    };
};