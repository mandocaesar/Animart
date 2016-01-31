(function () {
    var controllerId = 'app.views.supply';
    angular.module('app').controller(controllerId, ['$rootScope', '$scope', 'abp.services.app.supply',
        function ($rootScope, $scope, supplyService) {
            var vm = this;
            $scope.gridOptions = [];
            supplyService.getSupplies().success(function(result) {
                $scope.gridOptions.data = result;
            });

            $scope.gridOptions.columnDefs = [
                { name: 'id', enableCellEdit: false },
                { name: 'code', displayName: 'Code' },
                { name: 'name', displayName: 'Name (editable)' },
                { name: 'price', displayName: 'Price', cellFilter: 'currency' },
                { name: 'inStock', displayName: 'Stock', type: 'number' },
                { name: 'available', displayName: 'Active', type: 'boolean' }
            ];

            $scope.saveRow = function (rowEntity) {
                supplyService.update(rowEntity);
            };

            $scope.gridOptions.onRegisterApi = function (gridApi) {
                //set gridApi on scope
                $scope.gridApi = gridApi;
                gridApi.rowEdit.on.saveRow($scope, $scope.saveRow);
            };

        }
    ]);
})();