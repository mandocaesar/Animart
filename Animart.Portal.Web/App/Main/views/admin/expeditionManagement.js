﻿
angular.module('app').controller('expeditionController', ['$q', '$rootScope', '$scope', 'abp.services.app.shipment', '$uibModal',
expeditionController]).filter('cityFilter', function ($rootScope) {
    return function (input) {
        if (!input) {
            return '';
        } else {
            return $rootScope.cityOptions[input];
        }
    }
});

function expeditionController($q, $rootScope, $scope, expeditionService, $uibModal) {
    if (!(abp.auth.isGranted('CanAccessLogistic') || abp.auth.isGranted('CanAccessAdministrator')))
        window.location.href = "#";
    else {
        var vm = this;


        $scope.refresh = function() {
            $scope.gridOptions.data = null;
            expeditionService.getShipmentCosts().success(function(result) {
                //console.log(result);
                $scope.gridOptions.data = result;
            });
        };

        $scope.gridOptions = {
            enableRowSelection: true,
            enableSelectAll: false,
            multiselect: false,
            selectionRowHeaderWidth: 35,
            rowHeight: 35,
            showGridFooter: true
        };

        $scope.animationsEnabled = true;

        expeditionService.getCities().success(function(result) {
            $rootScope.cityOptions = result;
        });

        $scope.gridOptions.columnDefs = [
            { name: 'id', enableCellEdit: false },
            { name: 'expedition', displayName: 'Expedition Agent', enableCellEdit: false },
            { name: 'type', displayName: 'Type', enableCellEdit: false },
            {
                name: 'city',
                displayName: 'City Name',
                editableCellTemplate: 'ui-grid/dropdownEditor',
                editDropdownIdLabel: 'id',
                editDropDownValueLable: 'name',
                editDropdownRowEntityOptionsArrayPath: 'cityOptions'
            },
            { name: 'kiloQuantity', displayName: 'First Kilo Quantity', enableCellEdit: false },
            { name: 'firstKilo', displayName: 'First Kilo', enableCellEdit: false },
            { name: 'nextKilo', displayName: 'Next Kilo', enableCellEdit: false },
            {
                name: 'edit',
                displayName: 'Edit',
                cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.open(row.entity.id)"><i class="fa fa-pencil"></i> Edit</button>'
            }
        ];

        $scope.saveRow = function(rowEntity) {
            var promise = $q.defer();
            expeditionService.update(rowEntity)
                .success(function(result) { abp.notify.info('Expedition Updated!') })
                .error(function(result) { abp.notify.error('Error Occurred') });
            $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
            promise.resolve();
        };

        $scope.gridOptions.onRegisterApi = function(gridApi) {
            $rootScope.gridApi = gridApi;
            gridApi.rowEdit.on.saveRow($scope, $scope.saveRow);
        };

        $scope.delete = function() {
            var rows = $scope.gridApi.selection.getSelectedRows();
            angular.forEach(rows, function(value, key) {
                expeditionService.delete(value.id).success(function() {
                    $scope.refresh();
                });
            });
        };

        $scope.open = function(id) {

            if (id !== null && id !== undefined) {
                $scope.selectedItem = id;
            } else
                $scope.selectedItem = null;


            var modalInstance = $uibModal.open({
                animation: $scope.animationEnabled,
                templateUrl: 'addNewExpedition.html',
                scope: $scope,
                controller: 'expeditionModalCtrl',
                size: 'm'
            });

            modalInstance.result.then(function() {
                $scope.selectedItem = {};
                $scope.refresh();
            });
        };
        $scope.refresh();
    }
};

angular.module('app').controller('expeditionModalCtrl', ['$scope', 'abp.services.app.shipment', '$uibModalInstance',
function ($scope, expeditonService, $uibModalInstance, result) {

    $scope.shipmentItem = {};
    expeditonService.getCities().success(function (result) {
        //console.log(result);
        $scope.cities = result;
    });
    $scope.title = "Add New";
    $scope.init = function () {
        if ($scope.selectedItem !== null && $scope.selectedItem !== undefined) {
            $scope.title = "Edit";
            expeditonService.getShipment($scope.selectedItem)
                .success(function (result) {
                    //console.log(result);
                    $scope.shipmentItem.Id = result.id;
                    $scope.shipmentItem.Expedition = result.expedition;
                    $scope.shipmentItem.Type = result.type;
                    $scope.shipmentItem.FirstKilo = result.firstKilo;
                    $scope.shipmentItem.NextKilo = result.nextKilo;
                    $scope.shipmentItem.KiloQuantity = result.kiloQuantity;
                    $scope.shipmentItem.City = result.city;
                })
                .error(function (result) {
                    abp.notify.error('Error Occured while loading expedition data');
                }
            );
        }
    };

    $scope.ok = function () {
        //console.log($scope.shipmentItem);
        if ($scope.selectedItem !== null && $scope.selectedItem !== undefined) {
            $scope.update();
        } else {
            $scope.create();
        }
    };

    $scope.cancel = function () {
        $uibModalInstance.close();
    }

    $scope.create = function () {
        expeditonService.create($scope.shipmentItem)
            .success(function (result) {
                $scope.result = result;
                $uibModalInstance.close($scope.result);
                $uibModalInstance.dismiss('cancel');
                abp.notify.info('Expedition has been created');
            }).error(function (rs) {
                abp.notify.error('Error Occured while create expedition data');
            });
    };

    $scope.update = function () {
        expeditonService.update($scope.shipmentItem)
          .success(function (result) {
              $scope.result = result;
              $uibModalInstance.close($scope.result);
              $uibModalInstance.dismiss('cancel');
              abp.notify.info('Expedition Updated!');
          })
          .error(function (result) { abp.notify.error('Error Occurred') });
    };

    $scope.init();
}
]);

