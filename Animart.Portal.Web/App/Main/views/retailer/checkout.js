(function () {
    var controllerId = 'app.views.retailer.checkout';
    angular.module('app').controller(controllerId, [
        '$rootScope', '$scope', '$state', 'abp.services.app.user', 'abp.services.app.order', 'abp.services.app.shipment', 'ngCart', 
        function ($rootScope, $scope, $state, appSession, orderService, expeditonService, ngCart) {
            var vm = this;

            var user = null;
            $scope.loggedUser = {};
            appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
                user = result.user;
                $scope.po.address = user.address;
               
            }).error(
                 function (result) {
                     console.log(result);
                 }
             );
            $scope.po = {
                address:'',
                province: '',
                city: '',
                postalCode: '',
                expedition: '',
                grandTotal: 0,
                status: 'MARKETING'
            };
          

            $scope.orderItems = [];

            expeditonService.getCities().success(function (result) {
                $scope.cities = result;
            });

            expeditonService.getShipmentCosts().success(function (result) {
                $scope.expeditions = result;
                $scope.calculateShip();
            });

            $scope.updateShippingPrice = function () {
                console.log(ngCart);
                if ($scope.po.expedition !== '' && $scope.po.city !== '') {
                    var name = $scope.po.expedition.split('-')[0];
                    var type = $scope.po.expedition.split('-')[1];
                    
                    expeditonService.getShipmentCostFilterByExpeditionAndCity(name, $scope.po.city,type).success(function(rs) {
                        console.log(rs);
                        ngCart.setShipping($scope.po.totalWeight * rs[0].nextKilo);
                    });
                }
            };

            //$rootScope.updateItem = function(id, qty) {
            //    var items = ngCart.getItems();
            //    for (var i = 0; i < items.length; i++) {
            //        if (item.getId() === id) {
            //            item.setQuantity(qty, true);
            //        }
            //    }
            //    $scope.calculateShip();
            //    $scope.updateShippingPrice();

            //}

            $scope.$on('ngCart:change', function (event, args) {
                $scope.calculateShip();
                $scope.updateShippingPrice();
            });

            $scope.calculateShip = function () {
                var items = ngCart.getItems();
                var totalWeight = 0;

                for (var i = 0; i < items.length; i++) {
                    totalWeight = items[i].getData() * items[i].getQuantity();
                    $scope.po.totalWeight = totalWeight;
                }

                ngCart.setShipping(0);
            };

            $scope.translateCart = function() {
                var carts = ngCart.getItems();

                for (var i = 0; i < carts.length; i++) {
                    $scope.orderItems.push({
                        Id: carts[i]._id,
                        SupplyItem: carts[i]._id,
                        Name: carts[i]._name,
                        Quantity: carts[i]._quantity,
                        PriceAdjusment: 0,
                        PurchaseOrder: carts[i]._id
                    });
                };
            };

            function validate() {
                if ($scope.po.address === '' || $scope.po.province === '' || $scope.po.city === '' || $scope.po.postalCode === '' || $scope.po.expedition === '' || ngCart.getTotalItems() === 0) {
                    abp.message.error('One of required fields empty!');
                    return false;
                }
                return true;
            }

            $scope.placeOrder = function () {
                if (validate() === false) {
                    return;
                }
                $scope.translateCart();
                $scope.po.grandTotal = ngCart.totalCost();

                orderService.create($scope.po).success(function (result) {
                    orderService.addOrderItem(result, $scope.orderItems).success(function(rs) {
                        abp.message.info('Order Placed ID:' + result);
                        ngCart.empty();
                    }).error(function(rs) {
                        abp.message.error(rs);
                    });
                }).error(function(result) {
                    abp.message.error('Error:' + result);
                });
            };
        }
    ]);
})();