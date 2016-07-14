﻿angular.module('app').controller('app.views.logisticDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal','$mdDialog',
    dashboardController
]);

function ViewLogisticOrderController($http, $scope, $mdDialog, orderService, purchaseOrderId) {

    orderService.getSinglePurchaseOrder(purchaseOrderId).success(function (result) {
        $scope.po = result;
        $scope.isBod = result.status === "LOGISTIC";
        $scope.isPaid = result.status === "PAID" || result.status === "DONE" || result.status === "LOGISTIC";
        $scope.isDone = result.status === "DONE";
        $scope.supplies = result.items;
        //console.log(result.items);
        $scope.isLogistic = result.status === "LOGISTIC";
        //console.log(result);
        if ($scope.isPaid) {
            $scope.image = '../UserImage/' + $scope.po.id + ".jpg";
        }
    });

    $scope.file = {};
    $scope.getFile = function (e) {
        $scope.$apply(function () {
            $scope.files = e.files;
        });
    };

    $scope.receive = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "DONE").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Verified");
        });
    };
    $scope.close = function () {
        $mdDialog.cancel();
    };

    $scope.updateReceipt = function () {
        orderService.insertReceiptNumber(purchaseOrderId, $scope.po.receiptNumber).success(function () {
            abp.message.success("Success", "Receipt number fo Purchase Order " + purchaseOrderId + " Has Been Updated");
        });
    }
}

function dashboardController($q, $rootScope, $scope, orderService, $uibModal,$mdDialog) {

    $scope.gridOptions = {
        enableRowSelection: true,
        enableSelectAll: false,
        multiselect: false,
        selectionRowHeaderWidth: 35,
        rowHeight: 35,
        showGridFooter: true
    };
    $scope.animationsEnabled = true;

    $scope.statusGrid = 4;
    $scope.changeTab = function (num) {
        $scope.statusGrid = num;
        $scope.refresh();
    };
    $scope.tabOrders = [
      { no: 5, name: "Done" },
      { no: 4, name: "On Delivery" }
    ];

    $scope.refresh = function () {
        orderService.getDashboardAdmin().success(function (result) {
            console.log(result);
            $scope.dashboard = result;
        });

        $scope.gridOptions.data = null;
        orderService.getAllPurchaseOrderForLogistic($scope.statusGrid).success(function (result) {
            console.log(result);
            $scope.gridOptions.data = result;
        });
    };

    $scope.showMe = function (id) {
        var ev = this.ev;
        $mdDialog.show({
            templateUrl: 'view-order-logistic.tmpl.html',
            controller: ViewLogisticOrderController,
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
        { name: 'expedition', displayName: 'Expedition', enableCellEdit: false },
        { name: 'province', displayName: 'Province', enableCellEdit: false },
        { name: 'address', displayName: 'Address', enableCellEdit: false },
        { name: 'status', displayName: 'Status', enableCellEdit: false },
        { name: 'totalWeight', displayName: 'Total Weight', enableCellEdit: false },
        { name: 'grandTotal', displayName: 'Sub Total', cellFilter: 'currency:"Rp"', enableCellEdit: false },
        {
            name: 'view', displayName: 'View',
            cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.showMe(row.entity.id)"><i class="fa fa-pencil"></i> View</button>'
        }
    ];
    $scope.$on('updateDashboard', function (event, data) { $scope.refresh(); });
    $scope.refresh();
}