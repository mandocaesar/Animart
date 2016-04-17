(function () {
    var controllerId = 'app.views.item';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.supply', 'ngCart', '$stateParams', function ($scope, supplyService, ngCart, stateParams) {

            ngCart.setTaxRate(7.5);
            ngCart.setShipping(2.99);

            var itemID = stateParams.id;

            var vm = this;
            $scope.supplies = [];
            supplyService.supply(itemID).success(function (result) {
                $scope.supply = result;
                $scope.supply.image = '../SupplyImage/' + result.id + ".jpg";
            });
        }
    ]);
})();