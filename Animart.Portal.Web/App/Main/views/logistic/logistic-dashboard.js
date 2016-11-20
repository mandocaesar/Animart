angular.module('app').controller('app.views.logisticDashboard', [
    '$q', '$rootScope', '$scope', 'abp.services.app.order', 'abp.services.app.shipment', '$uibModal', '$mdDialog',
    dashboardController
]);

function ViewLogisticOrderController($http, $scope, $mdDialog, orderService, expeditionService, purchaseOrderId) {
    $scope.po = {};
    $scope.supplies = [];
    orderService.getSinglePurchaseOrder(purchaseOrderId).success(function (result) {
        $scope.po = result;
        $scope.isBod = result.status === "LOGISTIC";
        $scope.isPaid = result.status === "PAID" || result.status === "DONE" || result.status === "LOGISTIC";
        $scope.isDone = result.status === "DONE";
        $scope.supplies = result.items;
        $scope.isLogistic = result.status === "LOGISTIC";
        $scope.status = (result.isPreOrder) ? "Pre-Order" : "Ready Stock";
        if ($scope.isPaid) {
            $scope.image = '../UserImage/' + $scope.po.id + ".jpg";
        }
        $scope.updateExpedition();
    });
    $scope.allChecked = false;

    $scope.file = {};
    $scope.getFile = function (e) {
        $scope.$apply(function () {
            $scope.files = e.files;
        });
    };

    $scope.updateShippingPrice = function () {
        $scope.po.expeditionAdjustment = $scope.po.expeditionAdjustment.trim();
        if ($scope.po.expeditionAdjustment !== '' && $scope.po.city !== '') {
            var name = $scope.po.expeditionAdjustment.split('-')[0];
            var type = $scope.po.expeditionAdjustment.split('-')[1];
            expeditionService.getShipmentCostFilterByExpeditionAndCity(name, $scope.po.city, type).success(function (rs) {
                $scope.po.kiloAdjustmentQuantity = rs[0].kiloQuantity;
                $scope.po.shipmentAdjustmentCostFirstKilo = rs[0].firstKilo;
                $scope.po.shipmentAdjustmentCost = rs[0].nextKilo;
                $scope.po.totalAdjustmentShipmentCost = Math.max($scope.po.totalWeight - rs[0].kiloQuantity, 0) * rs[0].nextKilo + rs[0].firstKilo;
            });
        }
    };
    $scope.updateExpedition = function () {
        if ($scope.po.city !== '') {
            expeditionService.getShipmentCostFilterByCity($scope.po.city).success(function (rs) {
                if (rs == null || rs.length === 0) {
                    alert("Sorry your city is not available for shipment at the moment. Please contact marketing@animart.co.id for inquries.");
                    $scope.po.showExpedition = false;
                } else {
                    $scope.expeditions = rs;
                    $scope.po.showExpedition = true;
                }
            });
        }
    };
    $scope.receive = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "DONE").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Verified");
        });
        $mdDialog.cancel();

    };
    $scope.close = function () {
        $mdDialog.cancel();
    };

    $scope.print = function () {
        window.location.href = "#/orderDetail/" + purchaseOrderId;
        $mdDialog.cancel();
    };

    $scope.showInvoice = function (id) {
        window.location.href = "#/invoice/" + id;
        $mdDialog.cancel();
    };


    $scope.updateReceipt = function () {
        orderService.insertReceiptNumber(purchaseOrderId, $scope.po.receiptNumber).success(function () {
            abp.message.success("Success", "Receipt number for Purchase Order " + purchaseOrderId + " has been Updated");
        });
        $mdDialog.cancel();

    }
    $scope.insertExpeditionAdjustment = function () {
        if ($scope.po.expeditionAdjustment !== '' ) {
            orderService.insertExpeditionAdjustment(purchaseOrderId, $scope.po.expeditionAdjustment).success(function () {
                abp.message.success("Success", "Expedition Adjustment for Purchase Order " + purchaseOrderId + " has been Updated");
            });
        }
        $mdDialog.cancel();
    }

    $scope.reject = function () {
        orderService.updatePurchaseOrderStatus(purchaseOrderId, "REJECT").success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Rejected");
        });
        $mdDialog.cancel();
    };
    $scope.toggleInvoice = function (res) {
        $scope.showExpedition = false;
        if (!res)
            $scope.allChecked = false;
        for (var i = 0; i < $scope.supplies.length; i++) {
            if ($scope.supplies[i].checked) {
                $scope.showExpedition = true;
                i = $scope.supplies.length;
            }
        }
    };
    $scope.toggleAllInvoice = function () {
        $scope.showExpedition = $scope.allChecked;
        for (var i = 0; i < $scope.supplies.length; i++) {
            $scope.supplies[i].checked = $scope.showExpedition;
        }
    };
}

function dashboardController($q, $rootScope, $scope, orderService,expeditionService, $uibModal,$mdDialog) {
    if (!(abp.auth.isGranted('CanAccessLogistic') || abp.auth.isGranted('CanAccessAdministrator')))
        window.location.href = "#";
    else {
    $scope.gridOptions = {
        enableRowSelection: true,
        enableSelectAll: false,
        multiselect: false,
        selectionRowHeaderWidth: 35,
        rowHeight: 35,
        showGridFooter: true
    };
    $scope.animationsEnabled = true;

    $scope.statusType = 0;
    $scope.changeType = function (num) {
        $scope.statusType = num;
        $scope.refresh();
    };
    $scope.orderType = [
      { no: 1, name: "Pre-Order" },
      { no: 0, name: "Ready Stock" }
    ];

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
            $scope.dashboard = result;
        });

        $scope.gridOptions.data = null;
        orderService.getAllPurchaseOrderForLogistic($scope.statusType,$scope.statusGrid).success(function (result) {
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
                orderService: orderService,
                expeditionService: expeditionService
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
        { name: 'code', enableCellEdit: false },
        { name: 'creationTime', displayName: 'Date', cellFilter: 'date: "dd-MMMM-yyyy, HH:mma"', enableCellEdit: false },
         { name: 'creatorUser.name', displayName: 'Name', enableCellEdit: false },
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
}