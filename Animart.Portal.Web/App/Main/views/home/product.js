(function () {
    var controllerId = 'app.views.product';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.supply', 'abp.services.app.category', 'abp.services.app.user', 'ngCart', '$stateParams', '$state',
        function ($scope, supplyService,categoryService, appSession, ngCart, stateParams, $state) {

            ngCart.setTaxRate(0);
            ngCart.setShipping(0);

            var catId = stateParams.id;
            $scope.catName = stateParams.name;

            var vm = this;
            
            $scope.pageSize = 5;
            $scope.pagePOSize = 5;

            $scope.currentPOPage = 1;
            $scope.currentPage = 1;

            $scope.categories = [];
            $scope.supplies = [];
            $scope.poSupplies = [];
            $scope.itemType = stateParams.type;
            $scope.itemTypes =
                [{
                    no: 0,
                    name:"- All Product Types -"
                }, {
                    no: 1,
                    name:"Pre-Order Products"
                }, {
                    no: 2,
                    name: "In Stock Products"
                }];

            
            $scope.refresh = function () {
                //console.log(stateParams);
                if (catId == null || catId === "00000000-0000-0000-0000-000000000000" || catId === "") {
                    $scope.catName = "- All Categories -";
                    supplyService.getSuppliesRetailer().success(function(result) {
                        $scope.supplies = result.supply;

                        for (var i = 0; i < $scope.supplies.length; i++) {
                            if ($scope.supplies[i].hasImage) {
                                $scope.supplies[i].image = '../SupplyImage/' + $scope.supplies[i].id + ".jpg";
                            } else {
                                $scope.supplies[i].image = "";
                            }
                            $scope.supplies[i].availableUntil = new Date($scope.supplies[i].availableUntil);

                        }

                        $scope.poSupplies = result.poSupply;
                        for (var i = 0; i < $scope.poSupplies.length; i++) {
                            if ($scope.poSupplies[i].hasImage) {
                                $scope.poSupplies[i].image = '../SupplyImage/' + $scope.poSupplies[i].id + ".jpg";
                            } else {
                                $scope.poSupplies[i].image = "";
                            }
                            $scope.poSupplies[i].availableUntil = new Date($scope.poSupplies[i].availableUntil);
                        }
                    });
                } else {
                    supplyService.getSuppliesRetailerByCategoryId(catId).success(function (result) {
                        $scope.supplies = result.supply;

                        for (var i = 0; i < $scope.supplies.length; i++) {
                            if ($scope.supplies[i].hasImage) {
                                $scope.supplies[i].image = '../SupplyImage/' + $scope.supplies[i].id + ".jpg";
                            } else {
                                $scope.supplies[i].image = "";
                            }
                            $scope.supplies[i].availableUntil = new Date($scope.supplies[i].availableUntil);
                        }

                        $scope.poSupplies = result.poSupply;
                        for (var i = 0; i < $scope.poSupplies.length; i++) {
                            if ($scope.poSupplies[i].hasImage) {
                                $scope.poSupplies[i].image = '../SupplyImage/' + $scope.poSupplies[i].id + ".jpg";
                            } else {
                                $scope.poSupplies[i].image = "";
                            }
                            $scope.poSupplies[i].availableUntil = new Date($scope.poSupplies[i].availableUntil);
                        }
                    });
                }
            };
            categoryService.getCategoriesForFilter().success(function (result) {
                $scope.categories = result;
            });
            $scope.refresh();

            $scope.changeCategory = function (id, name,type) {
                $state.go("product", { id: id, name: name,type:type});
            };
            $scope.changeItemType = function (no) {
                $scope.itemType = no;
            };
            $scope.view = function (id) {
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