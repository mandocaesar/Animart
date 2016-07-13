angular.module('app').controller('app.views.accountingDashboard', ['$http',
    '$q', '$rootScope', '$scope', 'abp.services.app.order', '$uibModal', '$mdDialog',
    accountingController
]);

function ViewAccountingOrderController($http, $scope, $mdDialog, orderService, purchaseOrderId) {

    orderService.getSinglePurchaseOrder(purchaseOrderId).success(function (result) {
        console.log(result);
        $scope.po = result;
        $scope.isPaid = (result.status === "PAID" || result.status === "DONE" || result.status === "LOGISTIC") || result.status === "PAYMENT";
        $scope.supplies = result.items;
        //console.log($scope.supplies);
        $scope.image = "";
        if ($scope.isPaid) {
            $scope.image = '../UserImage/' + $scope.po.id + ".jpg";
        }
    });

    $scope.file = {};

    $scope.update = function () {
        var hasError = false;
        for (var i = 0; i < $scope.supplies.length; i++) {
            orderService.update($scope.po.id, $scope.supplies[i]).success(function(r) {
                $scope.$emit('updateDashboard', "ok");
                abp.message.info('Update Success');
            }).error(function (r) {
                hasError = true;
                if (i === $scope.supplies.length - 1) {
                    if (hasError) {
                        abp.message.error('error occured');
                    } else {
                        $scope.$emit('updateDashboard', "ok");
                        abp.message.info('Update Success');
                    }
                }
            });

            
        }

    };

    $scope.getFile = function (e) {
        $scope.$apply(function () {
            $scope.files = e.files;
        });
    }
    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.approve = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "PAYMENT").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Verified");
        });
    };

    $scope.sendToLogistic = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "LOGISTIC").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Sent to Logistic");
        });
    };

    $scope.updateReceipt = function() {
        orderService.insertReceiptNumber(purchaseOrderId, $scope.po.receiptNumber).success(function () {
            abp.message.success("Success", "Receipt number fo Purchase Order " + purchaseOrderId + " Has Been Updated");
        });
    }

}

function accountingController($http,$q, $rootScope, $scope, orderService, $uibModal, $mdDialog) {
    orderService.getDashboardAdmin().success(function (result) {
        $scope.dashboard = result;
    });

    $scope.gridOptions = {
        enableRowSelection: true,
        enableSelectAll: false,
        multiselect: false,
        selectionRowHeaderWidth: 35,
        rowHeight: 35,
        showGridFooter: true
    };
    $scope.animationsEnabled = true;

    $scope.statusGrid = 2;
    $scope.changeTab = function(num) {
        $scope.statusGrid = num;
        $scope.refresh();
    };

    $scope.refresh = function () {
        orderService.getDashboardAdmin().success(function (result) {
            console.log(result);
            $scope.dashboard = result;
        });

        $scope.gridOptions.data = null;
        orderService.getAllPurchaseOrderForAccounting($scope.statusGrid).success(function (result) {
            console.log(result);
            $scope.gridOptions.data = result;
        });
    };

    $scope.showMe = function (id) {
        var ev = this.ev;
        $mdDialog.show({
            templateUrl: 'view-order-accounting.tmpl.html',
            controller: ViewAccountingOrderController,
            parent: angular.element(document.body),
            targetEvent: ev,
            clickOutsideToClose: true,
            locals: {
                purchaseOrderId: id,
                orderService: orderService
            }
        }).then(function (rs) {
            $scope.$emit('updateDashboard', "ok");
        }, function () {
            $scope.status = 'You cancelled the dialog.';
        }).finally(function (rs) {
            $scope.refresh();
            $scope.$emit('updateDashboard', "ok");
        });
    };

    $scope.gridOptions.columnDefs = [
        { name: 'id', enableCellEdit: false },
        { name: 'expedition', displayName: 'Expedition', enableCellEdit: false },
        { name: 'province', displayName: 'Province', enableCellEdit: false },
        { name: 'address', displayName: 'Address', enableCellEdit: false },
        { name: 'totalWeight', displayName: 'Total Weight', enableCellEdit: false },
        { name: 'grandTotal', displayName: 'Grand Total', cellFilter: 'currency:"Rp"' },
        {
            name: 'view', displayName: 'View',
            cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.showMe(row.entity.id)"><i class="fa fa-pencil"></i> View</button>'
        }
    ];
    $scope.$on('updateDashboard', function (event, data) { $scope.refresh(); });
    $scope.refresh();

}