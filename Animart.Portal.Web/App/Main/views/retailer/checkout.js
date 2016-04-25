(function () {
    var controllerId = 'app.views.retailer.checkout';
    angular.module('app').controller(controllerId, [
        '$rootScope', '$state', 'abp.services.app.user', 'abp.services.app.shipment', 'ngCart', function ($rootScope, $state, appSession, expeditonService, ngCart) {
            var vm = this;
            $scope.po = {
                address: '',
                province: '',
                city: '',
                postalCode: '',
                expedition: '',
                grandTotal: 0,
                status: 'BOD'
            };
            expeditonService.getCities().success(function (result) {
                $scope.cities = result;
            });

            expeditonService.getShipmentCosts().success(function (result) {
                $scope.expeditions = result;
            });
        }
    ]);
})();