﻿angular.module('app').controller('app.views.marketingDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal','$mdDialog',
    marketingController
]);

function ViewMarketingOrderController($http, $scope, $mdDialog, orderService, purchaseOrderId) {

    orderService.getSinglePurchaseOrder(purchaseOrderId).success(function (result) {
        $scope.po = result;
        $scope.isBod = result.status==="MARKETING";
        $scope.supplies = result.items;
        console.log(result);
    });

    $scope.file = {};
    $scope.getFile = function(e) {
        $scope.$apply(function() {
            $scope.files = e.files;
        });
    };

    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.update = function () {
        var hasError = false;
        for (var i = 0; i < $scope.supplies.length; i++) {
            orderService.update($scope.po.id,$scope.supplies[i]).error(function(r) {
                hasError = true;
            });

            if (i === $scope.supplies.length - 1) {
                if (hasError) {
                    abp.message.error('error occured');
                } else {
                    $scope.$emit('updateDashboard', "ok");
                    abp.message.info('Update Success');
                }
            }
        }
        
    };

    $scope.approve = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "ACCOUNTING").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Approved");
        });
    };

    $scope.reject = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "REJECT").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Rejected");
        });
    };
}


function marketingController($q, $rootScope, $scope, orderService, $uibModal, $mdDialog) {
    
    $scope.gridOptions = {
        enableRowSelection: true,
        enableSelectAll: false,
        multiselect: false,
        selectionRowHeaderWidth: 35,
        rowHeight: 35,
        showGridFooter: true
    };
    $scope.animationsEnabled = true;

    $scope.refresh = function () {
        orderService.getDashboardAdmin().success(function (result) {
            console.log(result);
            $scope.dashboard = result;
        });

        $scope.gridOptions.data = null;
        orderService.getAllPurchaseOrderForMarketing().success(function (result) {
            console.log(result);
            $scope.gridOptions.data = result;
        });
    };

    $scope.showMe = function (id) {
        var ev = this.ev;
        $mdDialog.show({
            templateUrl: 'view-order-bod.tmpl.html',
            controller: ViewMarketingOrderController,
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: true,
            locals: {
                purchaseOrderId: id,
                orderService: orderService
            }
        }).then(function (rs) {
            $scope.refresh();
            $scope.$emit('updateDashboard', "ok");
        }, function () {
            $scope.status = 'You cancelled the dialog.';
        }).finally(function () {
            $scope.refresh();
        });
    };

    $scope.gridOptions.columnDefs = [
        { name: 'id', enableCellEdit: false },
        { name: 'expedition', displayName: 'Expedition' },
        { name: 'province', displayName: 'Province' },
        { name: 'address', displayName: 'Address' },
        { name: 'totalWeight', displayName: 'Total Weight' },
        { name: 'grandTotal', displayName: 'Grand Total', cellFilter: 'currency:"Rp"' },
        {
            name: 'view', displayName: 'View',
            cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.showMe(row.entity.id)"><i class="fa fa-pencil"></i> View</button>'
        }
    ];
    $scope.$on('updateDashboard', function (event, data) { $scope.refresh(); });
    $scope.refresh();
}