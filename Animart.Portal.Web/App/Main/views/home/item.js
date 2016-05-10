(function () {
    var controllerId = 'app.views.item';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.supply', 'abp.services.app.user', 'ngCart', '$stateParams', function ($scope, supplyService, appSession, ngCart, stateParams) {

            ngCart.setTaxRate(7.5);
            ngCart.setShipping(2.99);

            var itemID = stateParams.id;

            var vm = this;

            var user = null;
            appSession.user = null;

            $scope.supplies = [];
            supplyService.supply(itemID).success(function (result) {
                $scope.supply = result;
                $scope.supply.image = '../SupplyImage/' + result.id + ".jpg";
               
            });


            appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
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