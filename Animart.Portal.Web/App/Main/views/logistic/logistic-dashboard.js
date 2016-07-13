angular.module('app').controller('app.views.logisticDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal','$mdDialog',
    dashboardController
]);

function ViewLogisticOrderController($http, $scope, $mdDialog, orderService, purchaseOrderId) {

    orderService.getSinglePurchaseOrder(purchaseOrderId).success(function (result) {
        $scope.po = result;
        $scope.isBod = result.status === "LOGISTIC";
        $scope.isPaid = result.status === "PAID" || result.status === "DONE" || result.status === "LOGISTIC";
        $scope.supplies = result.items;
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

    $scope.cancel = function () {
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

    $scope.statusGrid = 1;
    $scope.changeTab = function (num) {
        $scope.statusGrid = num;
        $scope.refresh();
    };

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