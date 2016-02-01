(function () {
    var controllerId = 'app.views.users';
    angular.module('app').controller(controllerId, ['$q', '$rootScope', '$scope', 'abp.services.app.user', '$uibModal',
        function ($q, $rootScope, $scope, userService, $uibModal) {
            var vm = this;

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
                userService.getUsers().success(function (result) {
                    $scope.gridOptions.data = result;
                });
            };

            $scope.gridOptions.columnDefs = [
                { name: 'id', enableCellEdit: false },
                { name: 'firstName', displayName: 'First Name' },
                { name: 'lastName', displayName: 'Last Name' },
                { name: 'lastLoginTime', displayName: 'Last Login', cellFilter: 'date' },
                { name: 'role', displayName: 'Role' },
                { name: 'isActive', displayName: 'Active', type: 'boolean' }
            ];

            $scope.saveRow = function (rowEntity) {
                var promise = $q.defer();
                userService.updateUser(rowEntity)
                    .success(function (result) { abp.notify.info('Updated'); })
                    .error(function (result) { abp.notify.error('Error Occured'); });
                $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
                promise.resolve();
            };

            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $rootScope.gridApi = gridApi;
                gridApi.rowEdit.on.saveRow($scope, $scope.saveRow);
            };

            $scope.open = function () {
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'addNewUser.html',
                    controller: 'userModalCtrl',
                    size: 'm'
                });

                modalInstance.result.then(function (result) {
                    $scope.refresh();
                });
            };

            $scope.delete = function () {
                var rows = $scope.gridApi.selection.getSelectedRows();
                console.log(rows);
                angular.forEach(rows, function (value, key) {
                    console.log(value);
                    userService.delete(value.id).success(function () {
                        $scope.refresh();
                    });
                });
            };

            $scope.refresh();
        }
    ]);

    angular.module('app').controller('userModalCtrl', [
    '$scope', 'abp.services.app.supply', '$uibModalInstance',
    function ($scope, supplyService, $uibModalInstance, result) {
        $scope.ok = function () {
            supplyService.create($scope.supply)
              .success(function (rs) {
                  $scope.result = result;
                  $uibModalInstance.close($scope.result);
                  $uibModalInstance.dismiss('cancel');
                  abp.notify.info('Supply Saved Successfully');
              })
                .error(function (er) {
                    abp.notify.error('Error Occured');
                    $uibModalInstance.dismiss('cancel');
                });
        };

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
    ]);
})();