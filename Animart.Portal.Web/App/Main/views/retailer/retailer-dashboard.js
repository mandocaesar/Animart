angular.module('app').controller('app.views.retailerDashboard', [
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
        $scope.gridOptions.data = null;
        orderService.getAllPurchaseOrderByUserId().success(function (result) {
            console.log(result);
            $scope.gridOptions.data = result;
        });
    };

    $scope.showMe = function (id) {
        var ev = this.ev;
            $mdDialog.show({
                templateUrl: 'view-order.tmpl.html',
                controller: ViewOrderController,
                parent: angular.element(document.body),
                targetEvent: ev,
                clickOutsideToClose: true,
                locals: {
                    purchaseOrderId:id,
                    orderService: orderService
                }
            }).then(function (rs) {
                    $scope.$emit('updateDashboard', "ok");
            }, function () {
                $scope.status = 'You cancelled the dialog.';
            });
    };

    $scope.gridOptions.columnDefs = [
        { name: 'id', enableCellEdit: false },
        { name: 'expedition', displayName: 'Expedition' },
        { name: 'province', displayName: 'Province' },
        { name: 'address', displayName: 'Address' },
        { name: 'totalWeight', displayName: 'Total Weight' },
        { name: 'grandTotal', displayName: 'Grand Total' , cellFilter: 'currency:"Rp"'},
        {
            name: 'view',displayName:'View',
            cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.showMe(row.entity.id)"><i class="fa fa-pencil"></i> View</button>'
        }
    ];

    $scope.LoadDashboard = function () {

        orderService.getDashboard().success(function (result) {
            $scope.dashboard = result;
        });
        orderService.updateChart().success(function (result) {
            $scope.data.push(result);
        });

    };

    $scope.$on('updateDashboard', function (event, data) { $scope.LoadDashboard(); });
    $scope.refresh();
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

function ViewOrderController($http, $scope, $mdDialog, orderService, purchaseOrderId) {

    orderService.getSinglePurchaseOrder(purchaseOrderId).success(function(result) {
        $scope.po = result;
       
        $scope.isApproved = false;
        $scope.isNotBOD = false;

        if(result.status == "APPROVED" || result.status == "ACCOUNTING")
        {
            $scope.isApproved = true;
            if (result.status === "APPROVED") {
                $scope.isNotBOD = true;
            }
        }
        if ($scope.po.status === "ACCOUNTING") {

            $scope.image = '../UserImage/' + $scope.po.id + ".jpg";
            console.log($scope.image);
        }
        console.log($scope.isApproved);
        $scope.supplies = result.items;
    });

    $scope.file = {};
    $scope.getFile = function (e) {
        $scope.$apply(function () {
            $scope.files = e.files;
        });
    }
    $scope.cancel = function () {
        $mdDialog.cancel();
    };

    $scope.upload = function() {
        var data = new FormData();
        data.append("id", $scope.po.id);
        data.append("type", "payment");
        data.append("uploadedFile", $scope.files[0]);
       
        $http.post("/api/fileupload/", data, {
            transformRequest: angular.identity,
            headers: { 'Content-Type': undefined }
        }).success(function (data) {
            if (data) {
                abp.message.success("Success", "Files uploaded successfully.");
                orderService.updatePurchaseOrderStatus($scope.po.id, "ACCOUNTING");
            } else {
                    abp.message.error("Error", "Files uploaded unsuccess");
                }
            })
        .error(function (data, status) {
            abp.message.error("Error", "Files uploaded unsuccess");
        });
    };
}

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