(function () {
    var controllerId = 'app.views.retailer.checkout';
    angular.module('app').controller(controllerId, [
        '$rootScope', '$scope', '$state', 'abp.services.app.user', 'abp.services.app.order', 'abp.services.app.supply', 'abp.services.app.shipment', 'ngCart',
        function ($rootScope, $scope, $state, appSession, orderService,supplyService, expeditonService, ngCart) {
            var vm = this;

            var user = null;
            $scope.loggedUser = {};
            $scope.isCombined = false;
            $scope.isPO = false;
            $scope.isAvailable = true;
          
            appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
                user = result.user;
                $scope.po.address = user.address;
                $scope.po.phoneNumber = user.phoneNumber;
               
            }).error(
                 function (result) {
                     console.log(result);
                 }
             );
            if($scope.po==null)
                $scope.po = {
                    address: '',
                    phoneNumber:'',
                    province: '',
                    city: '',
                    isPreOrder: false,
                    postalCode: '',
                    expedition: '',
                    expeditionAdjustment:'',
                    grandTotal: 0,
                    totalWeight: 0,
                    totalGram:0,
                    status: 'MARKETING',
                    shipping: 0,
                    firstKilo: 0,
                    kiloQuantity:1,
                    showExpedition:false
                };

            $scope.orderItems = [];

            expeditonService.getCities().success(function (result) {
                $scope.cities = result;
                $scope.calculateShip();
            });

            expeditonService.getShipmentCosts().success(function (result) {
                //$scope.expeditions = result;
                //console.log(result);
                //$scope.calculateShip();

                //ngCart.setShipping(0);
            });
            $scope.updateExpedition = function () {
                if ($scope.po.city !== ''){
                    expeditonService.getShipmentCostFilterByCity($scope.po.city).success(function (rs) {
                        //console.log(rs);
                        //alert(rs[0].nextKilo);

                        if (rs == null || rs.length===0) {
                            alert("Sorry your city is not available for shipment at the moment. Please contact marketing@animart.co.id for inquries.");
                            $scope.po.showExpedition = false;
                        } else {
                            $scope.expeditions = rs;
                            //console.log(rs);
                            ngCart.setShipping(0);
                            $scope.po.showExpedition = true;
                        }
                    });
                }
            };
            $scope.updateShippingPrice = function () {
                //console.log(ngCart);
                if ($scope.po.expedition !== '' && $scope.po.city !== '') {
                    var name = $scope.po.expedition.split('-')[0];
                    var type = $scope.po.expedition.split('-')[1];
                    //alert(name);
                    //alert(type);
                    expeditonService.getShipmentCostFilterByExpeditionAndCity(name, $scope.po.city,type).success(function(rs) {
                        //console.log(rs);
                        //alert(rs[0].nextKilo);
                        $scope.po.shipping = rs[0].nextKilo;
                        $scope.po.firstKilo = rs[0].firstKilo;
                        $scope.po.kiloQuantity = rs[0].kiloQuantity;
                        ngCart.setShipping((Math.max($scope.po.totalWeight - rs[0].kiloQuantity, 0) * rs[0].nextKilo) + rs[0].firstKilo);
                    });
                }
            };

            //$rootScope.updateItem = function(id, qty) {
            //    var items = ngCart.getItems();
            //    for (var i = 0; i < items.length; i++) {
            //        if (items[i].getId() === id) {
            //            items[i].setQuantity(qty, true);
            //        }
            //    }
            //    $scope.calculateShip();
            //    $scope.updateShippingPrice();

            //}

            $scope.$on('ngCart:change', function (event, args) {
                $scope.checkItemQuantity();
                $scope.calculateShip();
                $scope.updateShippingPrice();
            });

            $scope.checkItemQuantity = function() {
                var items = ngCart.getItems();
                var data = null;
                for (var i = 0; i < items.length; i++) {
                    data = items[i].getData();
                    //if (!data.ispo) {
                    //    if (items[i].getQuantity() < 1)
                    //        items[i]._quantity = 1;
                    //    else if (items[i].getQuantity() > data.inStock)
                    //        items[i]._quantity = data.inStock;
                    //} else {
                        if (items[i].getQuantity() < 1)
                            items[i]._quantity = 1;
                       else if (items[i].getQuantity() >500)
                            items[i]._quantity = 500;
                    //}

                }
            };

            $scope.validateAvailability = function() {
                var items = ngCart.getItems();
                for (var i = 0; i < items.length; i++) {
                    supplyService.isAvailable(items[i].getData().id, i).success(function(result) {
                        ngCart.getItems()[result.idx].getData().available = result.isAvailable;
                        $scope.isAvailable = $scope.isAvailable && result.isAvailable;
                        //console.log(result);
                    }).error(function(rs) {
                        console.log(rs);
                    });
                }
            };
            $scope.calculateShip = function () {
                var items = ngCart.getItems();
                var totalWeight = 0;
                var totalGram = 0;
                var subTotal = 0;
                //console.log(items);
               
                $scope.isCombined = false;
                for (var i = 0; i < items.length; i++) {
                    totalGram += items[i].getData().weight * items[i].getQuantity();
                    subTotal += items[i].getPrice() * items[i].getQuantity();
                    
                    if (i === 0)
                        $scope.isPO = items[i].getData().ispo;
                    else {
                        if ($scope.isPO !== items[i].getData().ispo)
                            $scope.isCombined = true;
                    }
                    //alert(subTotal);
                    //alert(items[i].getData());
                }
                $scope.validateAvailability();
                $scope.po.subTotal = subTotal;
                totalWeight = convertToKg(totalGram);
                $scope.po.totalGram = totalGram;
                $scope.po.totalWeight = totalWeight;
                //console.log($scope.po);
                //alert($scope.po.expedition.nextKilo);

                ngCart.setShipping( Math.max(totalWeight-$scope.po.expedition.kiloQuantity,0) * $scope.po.expedition.nextKilo +$scope.po.expedition.firstKilo);
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
                //$scope.calculateShip();

            };

            function validate() {
                if ($scope.isAvailable === false) {
                    abp.message.error('One of more items you want to buy is not available anymore, please remove it from cart.');
                    return false;
                }
                if ($scope.po.address === '' || $scope.po.phoneNumber === '' || $scope.po.province === '' || $scope.po.city === '' || $scope.po.postalCode === '' || $scope.po.expedition === '' || ngCart.getTotalItems() === 0) {
                    abp.message.error('One of required fields empty!');
                    return false;
                }
                return true;
            }

            function convertToKg(data) {
                return parseInt(((data+999)/1000));
            }

            $scope.placeOrder = function () {
                $scope.validateAvailability();
                if (validate() === false) {
                    return;
                }
                $scope.translateCart();
                //console.log($scope.orderItems);
                $scope.po.hideOrderBtn = true;
                $scope.po.isPreOrder = $scope.isPO;
                $scope.po.grandTotal = ngCart.totalCost();
                //alert($scope.po.grandTotal);

                orderService.create($scope.po).success(function (result) {
                    orderService.addOrderItem(result, $scope.orderItems).success(function (rs) {
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