angular.module('app').controller('app.views.accountingDashboard', ['$http',
    '$q', '$rootScope', '$scope', 'abp.services.app.order', 'abp.services.app.shipment', '$uibModal', '$mdDialog',
    accountingController
]);

function ViewAccountingOrderController($http, $scope, $mdDialog, orderService,
    expeditionService, purchaseOrderId,statusType) {

    $scope.showExpedition = false;
    $scope.supplies = [];
    $scope.po = {
        address: '',
        province: '',
        city: '',
        isPreOrder: false,
        postalCode: '',
        expedition: '',
        expeditionAdjustment: '',
        isAdjustment: false,
        grandTotal: 0,
        totalWeight: 0,
        totalGram: 0,
        status: 'MARKETING',
        shipping: 0,
        showExpedition: false
    };

    $scope.showInvoice = false;
    $scope.allChecked = false;


    orderService.getSinglePurchaseOrder(purchaseOrderId, statusType).success(function (result) {
        $scope.po = result;
        //console.log(result);
        if ($scope.po.expedition !== $scope.po.expeditionAdjustment)
            $scope.po.isAdjustment = true;
        $scope.isPayment = (result.status === "PAID" || result.status === "DONE" || result.status === "LOGISTIC") || result.status === "PAYMENT";
        $scope.isPaid = result.status === "PAID";
        $scope.isNeedPayment = result.status === "PAYMENT";
        $scope.isDone = result.status === "LOGISTIC" || result.status === "DONE";
        $scope.supplies = result.items;
        $scope.isBod = result.status === "ACCOUNTING";
        $scope.isAdmin = abp.auth.isGranted('CanAccessAdministrator');
        $scope.status = (result.isPreOrder) ? "Pre-Order" : "Ready Stock";
        $scope.image = "";
        if ($scope.isPayment) {
            $scope.image = '../UserImage/' + $scope.po.id + ".jpg";
        }
        $scope.updateExpedition();
    });

    $scope.file = {};

    $scope.update = function () {
        var hasError = false;
        //for (var i = 0; i < $scope.supplies.length; i++) {
        //    console.log(i);
        //    console.log($scope.po.id);
        //    console.log($scope.supplies[i]);
            orderService.updatePO($scope.po.id, $scope.supplies).error(function (r) {
                hasError = true;
            });
        //}
        if (hasError) {
            abp.message.error('error occured');
        } else {
            $scope.$emit('updateDashboard', "ok");
            abp.message.info('Update Success');
        }
        $mdDialog.cancel();

    };
    $scope.getSubTotal = function () {
        var total = 0;
        if ($scope.supplies !== null)
            for (var i = 0; i < $scope.supplies.length; i++) {
                var product = $scope.supplies[i];
                total += (product.priceAdjustment * product.quantityAdjustment);
            }
        $scope.po.grandTotal = total;
        return total;
    }
    $scope.getFile = function (e) {
        $scope.$apply(function () {
            $scope.files = e.files;
        });
    }
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
    $scope.changeToPaid = function () {
        orderService.updateOrderItemStatus(purchaseOrderId, "PAID",$scope.supplies).success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Verified");
        });
        $mdDialog.cancel();

    };
    $scope.approve = function () {
        orderService.updateOrderItemStatus(purchaseOrderId, "PAYMENT", $scope.supplies).success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Verified");
        });
        $mdDialog.cancel();

    };
    $scope.reject = function () {
        orderService.updateOrderItemStatus(purchaseOrderId, "REJECT", $scope.supplies).success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Rejected");
        });
        $mdDialog.cancel();
    };
    $scope.sendToLogistic = function () {
        orderService.updateOrderItemStatus(purchaseOrderId, "LOGISTIC", $scope.supplies).success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Sent to Logistic");
        });
        $mdDialog.cancel();
    };
    $scope.sendToReview = function () {
        orderService.updateOrderItemStatus(purchaseOrderId, "ACCOUNTING", $scope.supplies).success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Sent to Accounting Review");
        });
        $mdDialog.cancel();
    };
    $scope.sendToMarketingReview = function () {
        orderService.updateOrderItemStatus(purchaseOrderId, "MARKETING", $scope.supplies).success(function () {
            abp.message.success("Success", "Purchase Order " + purchaseOrderId + " Has Been Sent to Marketing Review");
        });
        $mdDialog.cancel();
    };
    $scope.updateReceipt = function() {
        orderService.insertReceiptNumber(purchaseOrderId, $scope.po.receiptNumber).success(function () {
            abp.message.success("Success", "Receipt number fo Purchase Order " + purchaseOrderId + " Has Been Updated");
        });
        $mdDialog.cancel();

    }
    //$scope.insertExpeditionAdjustment = function () {
    //    if ($scope.po.expeditionAdjustment !== '') {
    //        orderService.insertExpeditionAdjustment(purchaseOrderId, $scope.po.expeditionAdjustment).success(function () {
    //            abp.message.success("Success", "Expedition Adjustment for Purchase Order " + purchaseOrderId + " has been Updated");
    //        });
    //    }
    //    $mdDialog.cancel();
    //}
    $scope.insertExpeditionAdjustment = function () {
        if ($scope.po.expeditionAdjustment !== '') {
            orderService.updateExpeditionAdjustment(purchaseOrderId, $scope.po.expeditionAdjustment, $scope.supplies).success(function () {
                abp.message.success("Success", "Expedition Adjustment for Purchase Order " + purchaseOrderId + " has been Updated");
            });
        }
        $mdDialog.cancel();
    }
    $scope.updateExpedition = function () {
        if ($scope.po.city !== '') {
            expeditionService.getShipmentCostFilterByCity($scope.po.city).success(function (rs) {
                if (rs === null || rs.length === 0) {
                    alert("Sorry your city is not available for shipment at the moment. Please contact marketing@animart.co.id for inquries.");
                    $scope.po.showExpedition = false;
                } else {
                    $scope.expeditions = rs;
                    $scope.po.showExpedition = true;
                }
            });
        }
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

    $scope.createInvoice = function() {
        orderService.createInvoice($scope.po).success(function (result) {
            orderService.addOrderItemToInvoice(result, $scope.supplies).success(function (rs) {
                abp.message.info('Invoice ID:' + result);
                $mdDialog.cancel();
            }).error(function (rs) {
                abp.message.error(rs);
            });
        }).error(function (result) {
            abp.message.error('Error:' + result);
        });
    };

    $scope.toggleInvoice = function (res) {
        $scope.showExpedition = false;
        if (!res)
            $scope.allChecked = false;
        for (var i = 0; i < $scope.supplies.length; i++) {
            if ($scope.supplies[i].checked){
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

function accountingController($http,$q, $rootScope, $scope, orderService,expeditionService, $uibModal, $mdDialog) {

    if (!(abp.auth.isGranted('CanAccessAccounting') || abp.auth.isGranted('CanAccessAdministrator')))
        window.location.href = "#";
    else {
        orderService.getDashboardAdmin().success(function(result) {
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


        $scope.statusType = 0;
        $scope.changeType = function(num) {
            $scope.statusType = num;
            $scope.refresh();
        };
        $scope.orderType = [
            { no: 1, name: "Pre-Order" },
            { no: 0, name: "Ready Stock" }
        ];

        $scope.statusGrid = 2;
        $scope.changeTab = function(num) {
            $scope.statusGrid = num;
            $scope.refresh();
        };

        $scope.tabOrders = [
            { no: 5, name: "Done" },
            { no: 4, name: "On Delivery" },
            { no: 6, name: "Paid" },
            { no: 3, name: "Waiting For Payment" },
            { no: 2, name: "Need Review" },
            { no: 1, name: "In Marketing" },
            { no: 0, name: "Rejected" }
        ];


        $scope.refresh = function() {
            orderService.getDashboardAdmin().success(function(result) {
                $scope.dashboard = result;
            });

            $scope.gridOptions.data = null;
            orderService.getAllPurchaseOrderForAccounting($scope.statusType, $scope.statusGrid).success(function(result) {
                $scope.gridOptions.data = result;
            });
        };

        $scope.showMe = function(id) {
            var ev = this.ev;
            $mdDialog.show({
                templateUrl: 'view-order-accounting.tmpl.html',
                controller: ViewAccountingOrderController,
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                locals: {
                    statusType:$scope.statusGrid,
                    purchaseOrderId: id,
                    orderService: orderService,
                    expeditionService:expeditionService
                }
            }).then(function(rs) {
                $scope.$emit('updateDashboard', "ok");
            }, function() {
                $scope.status = 'You cancelled the dialog.';
            }).finally(function(rs) {
                $scope.refresh();
                $scope.$emit('updateDashboard', "ok");
            });
        };

        $scope.gridOptions.columnDefs = [
            { name: 'code', enableCellEdit: false },
            { name: 'creationTime', displayName: 'Date', cellFilter: 'date: "dd-MMMM-yyyy, HH:mma"', enableCellEdit: false },
            { name: 'creatorUser.name', displayName: 'Name', enableCellEdit: false },
            { name: 'expedition', displayName: 'Expedition', enableCellEdit: false },
            { name: 'province', displayName: 'Province', enableCellEdit: false },
            { name: 'status', displayName: 'Status', enabledCellEdit: false },
            { name: 'grandTotal', displayName: 'Sub Total', cellFilter: 'currency:"Rp"', enableCellEdit: false },
            {
                name: 'view',
                displayName: 'View',
                cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.showMe(row.entity.id)"><i class="fa fa-pencil"></i> View</button>'
            }
        ];

        $scope.$on('updateDashboard', function(event, data) { $scope.refresh(); });
        $scope.refresh();
    }
}