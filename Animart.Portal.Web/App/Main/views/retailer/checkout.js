(function () {
    var controllerId = 'app.views.retailer.checkout';
    angular.module('app').controller(controllerId, [
        '$rootScope', '$scope', '$state', 'abp.services.app.user', 'abp.services.app.order', 'abp.services.app.shipment', 'ngCart',
        function ($rootScope, $scope, $state, appSession, orderService, expeditonService, ngCart) {
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
                console.log(result);
                $scope.cities = result;
            });

            expeditonService.getShipmentCosts().success(function (result) {
                console.log(result);
                $scope.expeditions = result;

                $scope.calculateShip();
                
            });

            $scope.calculateShip = function() {
                var items = ngCart.getItems();
                var totalWeight = 0;
                for (var i = 0; i < items.length; i++) {
                    totalWeight = items[i].getData();
                }
                ngCart.setShipping(totalWeight);
            };

            $scope.placeOrder = function () {
               // console.log(ngCart.getItems()[0].getData());
                //orderService.create($scope.po).success(function (result) {
                //    //for (var i = 0; i < $scope.rs.items.length; i++) {
                //    //    orderService.addOrderItem(result, $scope.rs.items[i]);
                //    //}
                //    $scope.$emit('updateDashboard', result);
                //    abp.notify.info('Order Placed ID:' + result);
                //});
            };
        }
    ]);
})();