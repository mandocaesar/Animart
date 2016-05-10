(function() {
    var controllerId = 'app.views.home';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.supply', 'abp.services.app.user', 'ngCart', '$state', function ($scope, supplyService,appSession, ngCart, $state) {

            ngCart.setTaxRate(0);
            ngCart.setShipping(0);

            var vm = this;
            $scope.supplies = [];
            $scope.poSupplies = [];

            supplyService.getSuppliesRetailer().success(function (result) {
                console.log(result);
                $scope.supplies = result.supply;
                for (var i = 0; i < $scope.supplies.length; i++) {
                    if ($scope.supplies[i].hasImage) {
                        $scope.supplies[i].image = '../SupplyImage/' + $scope.supplies[i].id + ".jpg";
                    } else {
                        $scope.supplies[i].image = "";
                    }
                }

                $scope.poSupplies = result.poSupply;
                for (var i = 0; i < $scope.poSupplies.length; i++) {
                    if ($scope.poSupplies[i].hasImage) {
                        $scope.poSupplies[i].image = '../SupplyImage/' + $scope.poSupplies[i].id + ".jpg";
                    } else {
                        $scope.poSupplies[i].image = "";
                    }
                }

                //console.log($scope.supplies);
            });

            $scope.view = function(id) {
                $state.go("item", { id: id });
            };

            var user = null;
            appSession.user = null;
            appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
                //console.log(result);
                user = result.user;
                $scope.luser = user;
            }).error(
              function (result) {
                 //console.log(result);
              }
          );
            vm.getShownUserName = function () {
                return user;
            };
        }
    ]);
})();