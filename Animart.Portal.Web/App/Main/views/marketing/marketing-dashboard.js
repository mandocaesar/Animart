angular.module('app').controller('app.views.marketingDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal','$mdDialog',
    marketingController
]);

function ViewMarketingOrderController($http, $scope, $mdDialog, orderService, purchaseOrderId) {

    orderService.getSinglePurchaseOrder(purchaseOrderId).success(function (result) {
        $scope.po = result;
        $scope.isBod = result.status === "MARKETING";
        $scope.isPayment = (result.status === "PAID" || result.status === "DONE" || result.status === "LOGISTIC") || result.status === "PAYMENT";
        $scope.isPaid = result.status === "PAID";
        $scope.isDone = result.status === "LOGISTIC" || result.status === "DONE";
        $scope.supplies = result.items;
        //console.log(result);
        if ($scope.isPayment) {
            $scope.image = '../UserImage/' + $scope.po.id + ".jpg";
        }
    });

    $scope.file = {};
    $scope.getFile = function(e) {
        $scope.$apply(function() {
            $scope.files = e.files;
        });
    };

    $scope.close = function () {
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
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has been sent to accounting for approval");
        });
    };

    $scope.reject = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "REJECT").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Rejected");
        });
    };

    $scope.updateReceipt = function () {
        orderService.insertReceiptNumber(purchaseOrderId, $scope.po.receiptNumber).success(function () {
            abp.message.success("Success", "Receipt number fo Purchase Order " + purchaseOrderId + " Has Been Updated");
        });
    }
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

    $scope.statusGrid = 1;
    $scope.changeTab = function (num) {
        $scope.statusGrid = num;
        $scope.refresh();
    };
    $scope.tabOrders = [
      { no: 5, name: "Done" },
      { no: 4, name: "On Delivery" },
      { no: 6, name: "Paid" },
      { no: 3, name: "Waiting For Payment" },
      { no: 1, name: "Need Review" },
      { no: 2, name: "In Accounting" },
      { no: 0, name: "Rejected" }
    ];
    $scope.refresh = function () {
        orderService.getDashboardAdmin().success(function (result) {
            console.log(result);
            $scope.dashboard = result;
        });

        $scope.gridOptions.data = null;
        orderService.getAllPurchaseOrderForMarketing($scope.statusGrid).success(function (result) {
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