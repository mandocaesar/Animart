(function () {
    var controllerId = 'app.views.category';
    angular.module('app').controller(controllerId, ['$q', '$rootScope', '$scope', 'abp.services.app.category', '$uibModal', '$http',
        function ($q, $rootScope, $scope, categoryService, $uibModal, $http) {
            if (!(abp.auth.isGranted('CanAccessMarketing') || abp.auth.isGranted('CanAccessAdministrator')))
                window.location.href = "#";
            else {
                var vm = this;
                $scope.newFile = {};
                $scope.gridOptions = {
                    enableRowSelection: true,
                    enableSelectAll: false,
                    multiselect: false,
                    selectionRowHeaderWidth: 35,
                    rowHeight: 35,
                    showGridFooter: true
                };
                $scope.title = "Add New";
                $scope.animationsEnabled = true;

                $scope.refresh = function() {
                    $scope.gridOptions.data = null;
                    categoryService.getCategories().success(function(result) {

                        //console.log(result);

                        $scope.gridOptions.data = result;
                    });
                };


                $scope.gridOptions.columnDefs = [
                    { name: 'id', enableCellEdit: false },
                    { name: 'parentName', displayName: 'Parent Category', enableCellEdit: false },
                    { name: 'name', displayName: 'Name', enableCellEdit: false }, {
                        name: 'view',
                        displayName: 'Edit',
                        cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.showMe(row.entity.id)"><i class="fa fa-pencil"></i> Edit</button>'
                    }
                ];

                $scope.saveRow = function(rowEntity) {
                    var promise = $q.defer();
                    categoryService.update(rowEntity)
                        .success(function(result) {
                            promise.resolve();
                            $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
                            $scope.refresh();
                            abp.notify.info('Updated');
                        })
                        .error(function(result) {
                            promise.resolve();
                            $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
                            $scope.refresh();
                            abp.notify.error('Error Occured');
                        });
                };

                $scope.gridOptions.onRegisterApi = function(gridApi) {
                    $rootScope.gridApi = gridApi;
                    gridApi.rowEdit.on.saveRow($scope, $scope.saveRow);
                };

                $scope.open = function() {
                    var modalInstance = $uibModal.open({
                        animation: $scope.animationsEnabled,
                        templateUrl: 'addNewCategory.html',
                        controller: 'categoryModalCtrl',
                        size: 'm'
                    });

                    modalInstance.result.then(function(result) {
                        $scope.refresh();
                    });
                };

                $scope.showMe = function(id) {
                    var modalInstance = $uibModal.open({
                        animation: $scope.animationsEnabled,
                        templateUrl: 'addNewCategory.html',
                        controller: 'categoryCtrl',
                        resolve: {
                            param: function() {
                                return { 'data': id };
                            }
                        },
                        size: 'm'
                    });

                    modalInstance.result.then(function(result) {
                        $scope.refresh();
                    });
                };

                $scope.delete = function() {
                    var rows = $scope.gridApi.selection.getSelectedRows();
                    angular.forEach(rows, function(value, key) {
                        categoryService.delete(value.id).success(function() {
                            $scope.refresh();
                        });
                    });
                };

                $scope.refresh();
            }
        }
    ]);

    angular.module('app').controller('categoryModalCtrl', [
    '$scope', 'abp.services.app.category', '$uibModalInstance',
    function ($scope, categoryService, $uibModalInstance, result) {
        $scope.IsEdit = false;
        $scope.parent = {};
        $scope.ok = function () {
            categoryService.create($scope.category)
              .success(function (rs) {
                  $scope.result = result;
                  $uibModalInstance.close($scope.result);
                  $uibModalInstance.dismiss('cancel');
                  abp.notify.info('Category Saved Successfully');
              })
                .error(function (er) {
                    abp.notify.error('Error Occured');
                    $uibModalInstance.dismiss('cancel');
                });
        };

        categoryService.getCategories().success(function (result) {
            $scope.parent = result;
        });

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
    ]);

    angular.module('app').controller('categoryCtrl', [
    '$scope', 'abp.services.app.category', '$uibModalInstance','param',
    function ($scope, categoryService, $uibModalInstance, param) {
        $scope.category = {};
        $scope.parent = {};
        $scope.IsEdit = true;
        $scope.title = "Edit";

        categoryService.category(param.data).success(function (rs) {
            $scope.category = rs;
        }).error(function (er) {
            abp.notify.error('Error Load Supply');
            $uibModalInstance.dismiss('cancel');
        });
        
        categoryService.getCategories().success(function (result) {
            $scope.parent = result;
        });

        $scope.update = function () {
            categoryService.update($scope.category)
              .success(function (result) {
                  $scope.result = result;
                  $uibModalInstance.close($scope.result);
                  $uibModalInstance.dismiss('cancel');
                  abp.notify.info('Category Saved Successfully');
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