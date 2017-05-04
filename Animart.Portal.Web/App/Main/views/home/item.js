(function () {
    var controllerId = 'app.views.item';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.supply', 'abp.services.app.user', 'ngCart', '$stateParams', '$sce', '$state',
        function ($scope, supplyService, appSession, ngCart, stateParams, $sce, $state) {
            ngCart.setTaxRate(0);
            ngCart.setShipping(0);

            var itemID = stateParams.id;

            $scope.catName = stateParams.name;
            $scope.catId = stateParams.catId;
            $scope.currentPOPage = stateParams.currentPOPage;
            $scope.currentPage = stateParams.currentPage;
            $scope.isLatestPO = stateParams.isLatestPO;
            $scope.search = stateParams.search;
            $scope.searchPO = stateParams.searchPO;
            $scope.showCart = true;

            var vm = this;

            var user = null;
            appSession.user = null;

            $scope.supplies = [];
            supplyService.supply(itemID).success(function (result) {
                $scope.supply = result;
                $scope.supply.availableUntil = new Date($scope.supply.availableUntil);
                console.log(result);
                //if ($scope.ispo)
                //    $scope.showCart = $scope.supply.availableUntil > new Date();
                $scope.showCart = $scope.showCart && result.available;
                $scope.supply.image = '../SupplyImage/' + result.id + ".jpg";
            });

            $scope.renderHtml = function (htmlCode) {
                return $sce.trustAsHtml(htmlCode);
            };

            appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
                user = result.user;
                $scope.luser = user;
            }).error(
               function (result) {
                   //console.log(result);
               }
           );

            $scope.backToProduct = function () {
                $state.go("product", {
                    catId: $scope.catId,
                    type: $scope.itemType, name: $scope.catName,
                    currentPage: $scope.currentPage,
                    currentPOPage: $scope.currentPOPage,
                    search: $scope.search,
                    searchPO: $scope.searchPO
                });
            };

            vm.getShownUserName = function () {
                return user;
            };
        }
    ]);
})();