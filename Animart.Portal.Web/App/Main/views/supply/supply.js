(function () {
    var controllerId = 'app.views.supply';
    angular.module('app').controller(controllerId, ['$q', '$rootScope', '$scope', 'abp.services.app.supply', 'abp.services.app.category', '$uibModal', '$http',
        function ($q, $rootScope, $scope, supplyService, categoryService, $uibModal, $http) {
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
                $scope.category = {};
                $scope.categoryId = "";
                $scope.title = "Add New";
                $scope.animationsEnabled = true;

                $scope.refresh = function() {
                    $scope.gridOptions.data = null;
                    var catId = $scope.categoryId + "";

                    if (catId === "00000000-0000-0000-0000-000000000000" || catId === "")
                        supplyService.getSupplies().success(function(result) {
                            //console.log(result);
                            $scope.gridOptions.data = result;
                        });
                    else
                        supplyService.getSuppliesByCategoryId($scope.categoryId).success(function(result) {
                            $scope.gridOptions.data = result;
                        });
                };

                categoryService.getCategoriesForFilter().success(function(result) {
                    $scope.category = result;
                });

                //$scope.$watch('mydateOfBirth', function (newValue) {
                //    $scope.workerDetail.dateOfBirth = $filter('date')(newValue, 'yyyy/MM/dd');
                //});

                //$scope.$watch('workerDetail.dateOfBirth', function (newValue) {
                //    $scope.mydateOfBirth = $filter('date')(newValue, 'yyyy/MM/dd');
                //});
                $scope.changeCategory = function() {
                    $scope.refresh();
                };

                $scope.storeFile = function(gridRow, gridCol, files) {
                    console.log(gridRow.entity);

                    var id = gridRow.entity.id;
                    var data = new FormData();
                    data.append("id", id);
                    data.append("type", "supply");
                    data.append("uploadedFile", files[0]);

                    $http.post("/api/fileupload/", data, {
                            transformRequest: angular.identity,
                            headers: { 'Content-Type': undefined }
                        }).success(function(data) {
                            if (data) {
                                gridRow.entity.hasImage = true;
                                supplyService.update(gridRow.entity)
                                    .success(function(result) {
                                        abp.message.success("Success", "Files uploaded successfully.");
                                    })
                                    .error(function(result) {
                                        abp.message.error('Error Occured');
                                    });

                            } else {
                                abp.message.error("Error", "Files uploaded unsuccess");
                            }
                        })
                        .error(function(data, status) {
                            abp.message.error("Error", "Files uploaded unsuccess");
                        });
                };

                $scope.gridOptions.columnDefs = [
                    { name: 'id', enableCellEdit: false },
                    { name: 'category', displayName: 'Category' },
                    { name: 'code', displayName: 'Code' },
                    { name: 'name', displayName: 'Name' },
                    { name: 'price', displayName: 'Price', cellFilter: 'currency:"Rp."' },
                    { name: 'weight', displayName: 'Weight (gr)', type: 'number' },
                    { name: 'available', displayName: 'Active', type: 'boolean' },
                    { name: 'ispo', displayName: "Is PO ?", type: 'boolean' },
                    { name: 'availableUntil', displayName: "Available Until", cellFilter: 'date: "dd-MMMM-yyyy, HH:mma"' },
                    { name: 'hasImage', displayName: "Image", type: 'boolean', enableCellEdit: false },
                    { name: 'filename', displayName: 'File', width: '20%', editableCellTemplate: 'ui-grid/fileChooserEditor', editFileChooserCallback: $scope.storeFile }, {
                        name: 'view',
                        displayName: 'Edit',
                        cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.showMe(row.entity.id)"><i class="fa fa-pencil"></i> Edit</button>'
                    }
                ];

                $scope.saveRow = function(rowEntity) {
                    var promise = $q.defer();
                    supplyService.update(rowEntity)
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
                        templateUrl: 'addNewSupply.html',
                        controller: 'supplyModalCtrl',
                        size: 'm'
                    });

                    modalInstance.result.then(function(result) {
                        $scope.refresh();
                    });
                };

                $scope.showMe = function(id) {
                    var modalInstance = $uibModal.open({
                        animation: $scope.animationsEnabled,
                        templateUrl: 'addNewSupply.html',
                        controller: 'editSupplyCtrl',
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
                        supplyService.delete(value.id).success(function() {
                            $scope.refresh();
                        });
                    });
                };

                $scope.refresh();
            }
        }
    ]);

    angular.module('app').controller('supplyModalCtrl', [
    '$scope', 'abp.services.app.supply', 'abp.services.app.category', '$uibModalInstance',
    function ($scope, supplyService,categoryService, $uibModalInstance, result) {
        $scope.IsEdit = false;
        $scope.category = {};
        $scope.ok = function () {
            supplyService.create($scope.supply)
              .success(function (rs) {
                    //console.log($scope.supply);
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

        categoryService.getCategoriesForFilter().success(function (result) {
            result.shift();
            $scope.category = result;
        });

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
    ]);

    angular.module('app').controller('editSupplyCtrl', [
    '$scope', 'abp.services.app.supply', 'abp.services.app.category', '$uibModalInstance', 'param',
    function ($scope, supplyService,categoryService, $uibModalInstance, param) {
        $scope.supply = {};
        $scope.category = {};
        $scope.IsEdit = true;
        $scope.title = "Edit";

       

        $scope.update = function () {
           supplyService.update($scope.supply)
              .success(function (result) {
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

        categoryService.getCategoriesForFilter().success(function (result) {
            result.shift();
            $scope.category = result;
            supplyService.supply(param.data).success(function (rs) {
                //console.log(rs);
                //console.log(rs.availableUntil);

                rs.availableUntil = new Date(rs.availableUntil);
                //console.log(rs.availableUntil);
                $scope.supply = rs;
            }).error(function (er) {
                abp.notify.error('Error Load Supply');
                $uibModalInstance.dismiss('cancel');
            });
        });

        $scope.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };
    }
    ]);

})();