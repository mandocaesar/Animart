(function () {
    var controllerId = 'app.views.product';
    angular.module('app').controller(controllerId, [
        '$scope', '$timeout', '$mdSidenav', 'abp.services.app.supply', 'abp.services.app.category', 'abp.services.app.user', 'ngCart', '$stateParams', '$state',
        function ($scope, $timeout, $mdSidenav, supplyService, categoryService, appSession, ngCart, stateParams, $state) {

            $scope.toggleLeft =buildDelayedToggler('left');
            function debounce(func, wait, context) {
                var timer;

                return function debounced() {
                    var context = $scope,
                        args = Array.prototype.slice.call(arguments);
                    $timeout.cancel(timer);
                    timer = $timeout(function () {
                        timer = undefined;
                        func.apply(context, args);
                    }, wait || 10);
                };
            }

            /**
             * Build handler to open/close a SideNav; when animation finishes
             * report completion in console
             */
            function buildDelayedToggler(navID) {
                return debounce(function () {
                    // Component lookup should always be available since we are not using `ng-if`
                    $mdSidenav(navID)
                      .toggle()
                      .then(function () {
                          //$log.debug("toggle " + navID + " is done");
                      });
                }, 200);
            }

            //function buildToggler(navID) {
            //    return function () {
            //        // Component lookup should always be available since we are not using `ng-if`
            //        $mdSidenav(navID)
            //          .toggle()
            //          .then(function () {
            //              //$log.debug("toggle " + navID + " is done");
            //          });
            //    }
            //}
            ngCart.setTaxRate(0);
            ngCart.setShipping(0);

            var catId = stateParams.catId;
            $scope.catName = stateParams.name;
            $scope.catId = catId;

            var vm = this;
            
            $scope.pageSize = 5;
            $scope.pagePOSize = 5;
            $scope.isLatestPO = stateParams.isLatestPO;
            $scope.currentPOPage = stateParams.currentPOPage;
            $scope.currentPage = stateParams.currentPage;
            $scope.search = stateParams.search;
            $scope.searchPO = stateParams.searchPO;

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
                    supplyService.getSuppliesRetailer($scope.isLatestPO).success(function(result) {
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
                    supplyService.getSuppliesRetailerByCategoryId(catId, $scope.isLatestPO).success(function (result) {
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

            $scope.changeCategory = function (id, name, type) {
                $scope.toggleLeft();
                $state.go("product", {
                    catId: id, name: name, type: type,
                    currentPage: 1,
                    currentPOPage: 1,
                    isLatestPO: $scope.isLatestPO,
                    search: "",
                    searchPO:""
                });
            };
            $scope.changeItemType = function (no) {
                $scope.toggleLeft();
                $scope.itemType = no;
            };
            $scope.view = function (id) {
                $state.go("item", {
                    id: id, catId: $scope.catId,
                    type: $scope.itemType, name: $scope.catName,
                    currentPage: $scope.currentPage,
                    currentPOPage: $scope.currentPOPage,
                    isLatestPO: $scope.isLatestPO,
                    search: $scope.search,
                    searchPO: $scope.searchPO
                });
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