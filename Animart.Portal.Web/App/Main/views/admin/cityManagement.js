
angular.module('app').controller('app.views.cities', ['$q', '$rootScope', '$scope', 'abp.services.app.shipment', '$uibModal',
   cityController]);

function cityController($q, $rootScope, $scope, cityService, $uibModal) {
    var vm = this;

    $scope.refresh = function () {
        $scope.gridOptions.data = null;
        cityService.getCities().success(function (result) {
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

    $scope.gridOptions.columnDefs = [
        { name: 'id', enableCellEdit: false },
        { name: 'name', displayName: 'City Name' }
    ];

    $scope.saveRow = function (rowEntity) {
        var promise = $q.defer();
        cityService.updateCity(rowEntity)
            .success(function (result) { abp.notify.info('City Updated!') })
            .error(function (result) { abp.notify.error('Error Ocurred') });
        $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
        promise.resolve();
    };

    $scope.gridOptions.onRegisterApi = function (gridApi) {
        $rootScope.gridApi = gridApi;
        gridApi.rowEdit.on.saveRow($scope, $scope.saveRow);
    };

    $scope.delete = function () {
        var rows = $scope.gridApi.selection.getSelectedRows();
        angular.forEach(rows, function (value, key) {
            cityService.deleteCity(value.id).success(function (rs) {
                if (rs) {
                    abp.notify.info(value.name + " has been deleted");
                    $scope.refresh();
                }
            });
        });
        
    };

    $scope.open = function () {
        var modalInstance = $uibModal.open({
            animation: $scope.animationEnabled,
            templateUrl: 'addNewCity.html',
            controller: 'cityModalCtrl',
            size: 'm'
        });

        modalInstance.result.then(function (result) {
            $scope.refresh();
        });
    };
   
    $scope.refresh();
}

angular.module('app').controller('cityModalCtrl', ['$scope', 'abp.services.app.shipment', '$uibModalInstance',
    function ($scope, cityService, $uibModalInstance, result) {
        $scope.ok = function () {
            cityService.createCity($scope.city.name)
                .success(function (rs) {
                    $scope.result = result;
                    $uibModalInstance.close($scope.result);
                    $uibModalInstance.dismiss('cancel');
                    abp.notify.info('City has been created');
                }).error(function (rs) {
                    abp.notify.error('Error Occured while create city');
                });
        };
        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
]);