﻿angular.module('app').controller('app.views.retailerDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal', '$mdSidenav', '$mdDialog',
    dashboardController
]).controller('LeftCtrl', [
    '$q', '$rootScope', '$scope', 'abp.services.app.supply', 'abp.services.app.shipment', 'abp.services.app.order', '$mdDialog',
    leftController
]);

function dashboardController($q, $rootScope, $scope, orderService, $uibModal, $mdSidenav, $mdDialog) {
    $scope.labels = ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"];
    $scope.series = ['Order Made'];
    $scope.data = [];

    $scope.LoadDashboard = function () {
        orderService.getDashboard().success(function (result) {
            $scope.dashboard = result;
        });

        orderService.updateChart().success(function (result) {
            $scope.data.push(result);
        });

    };

    $scope.$on('updateDashboard', function (event, data) { $scope.LoadDashboard(); });

    $scope.LoadDashboard();
};

function leftController($q, $rootScope, $scope, supplyService, expeditonService, orderService, $mdDialog) {

    expeditonService.getCities().success(function (result) {
        $scope.cities = result;
    });

    expeditonService.getShipmentCosts().success(function (result) {
        $scope.expeditions = result;
    });

    supplyService.getSupplies().success(function (result) {
        $scope.supplies = result;
    });

    $scope.addPO = function (ev) {
        $mdDialog.show({
            templateUrl: 'order.tmpl.html',
            controller: DialogController,
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: true,
            locals: {
                cities: $scope.cities,
                expeditions: $scope.expeditions,
                supplies: $scope.supplies,
                orderService: orderService
            }
        })
            .then(function (rs) {
                rs.po.city = rs.po.city.trim();
                rs.po.expedition = rs.po.expedition.trim();
                orderService.create(rs.po).success(function (result) {
                    for (var i = 0; i < rs.items.length; i++) {
                        orderService.addOrderItem(result, rs.items[i]);
                    }
                    $scope.$emit('updateDashboard', result);
                    abp.notify.info('Order Placed ID:' + result);
                });
            }, function () {
                $scope.status = 'You cancelled the dialog.';
            });
    };
};

function DialogController($scope, $mdDialog, cities, expeditions, supplies, orderService) {

    $scope.po = {
        address: '',
        province: '',
        city: '',
        postalCode: '',
        expedition: '',
        grandTotal: 0,
        status: 'BOD'
    };

    $scope.cities = cities;
    $scope.expeditions = expeditions;
    $scope.supplies = supplies;
    $scope.error = false;

    $scope.calculate = function (i) {
        var order = {
            id: '',
            purchaseOrder: '',
            supplyItem: $scope.supplies[i].id,
            quantity: $scope.supplies[i].quantity
        }
        orderService.checkOrderItem(order).success(function (result) {
            if (result) {
                $scope.error = false;
                $scope.po.grandTotal = 0;
                for (var i = 0; i < $scope.supplies.length; i++) {
                    var total = ($scope.supplies[i].price * $scope.supplies[0].quantity);
                    $scope.po.grandTotal += total;
                }
            } else {
                $scope.error = true;
                abp.notify.error('Quantity is exceed In Stock');
            }
        }
        );
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.place = function (answer) {
        var orders = [];
        for (var i = 0; i < $scope.supplies.length; i++) {
            var order = {
                id: '',
                purchaseOrder: '',
                supplyItem: $scope.supplies[i].id,
                quantity: $scope.supplies[i].quantity
            }
            orders.push(order);
        }

        $scope.result = { po: $scope.po, items: orders }

        $mdDialog.hide($scope.result);
    };

}