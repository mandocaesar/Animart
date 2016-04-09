(function() {
    var controllerId = 'app.views.home';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.supply', 'ngCart', function ($scope, supplyService, ngCart) {

            ngCart.setTaxRate(7.5);
            ngCart.setShipping(2.99);

            var vm = this;
            $scope.supplies = [];
            supplyService.getSupplies().success(function(result) {
                $scope.supplies = result;
                for (var i = 0; i < $scope.supplies.length; i++) {
                    if ($scope.supplies[i].hasImage) {
                        $scope.supplies[i].image = '../SupplyImage/' + $scope.supplies[i].id + ".jpg";
                    } else {
                        $scope.supplies[i].image = "";
                    }
                }
                console.log($scope.supplies);
            });
        }
    ]);
})();