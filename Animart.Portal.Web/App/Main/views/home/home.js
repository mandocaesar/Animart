(function() {
    var controllerId = 'app.views.home';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.supply', 'ngCart', '$state', function ($scope, supplyService, ngCart, $state) {

            ngCart.setTaxRate(10);
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

            $scope.view = function(id) {
                $state.go("item", { id: id });
            };
        }
    ]);
})();