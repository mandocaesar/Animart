(function () {
    var controllerId = 'app.views.supply';
    angular.module('app').controller(controllerId, ['$q', '$rootScope', '$scope', 'abp.services.app.supply', '$uibModal', '$http',
        function ($q, $rootScope, $scope, supplyService, $uibModal, $http) {
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
            $scope.animationsEnabled = true;

            $scope.refresh = function () {
                $scope.gridOptions.data = null;
                supplyService.getSupplies().success(function (result) {
                    console.log(result);
                    $scope.gridOptions.data = result;
                });
            };

            $scope.storeFile = function (gridRow, gridCol, files) {
                console.log(gridRow.entity);

                var id = gridRow.entity.id;
                var data = new FormData();
                data.append("id", id);
                data.append("type", "supply");
                data.append("uploadedFile", files[0]);

                $http.post("/api/fileupload/", data, {
                    transformRequest: angular.identity,
                    headers: { 'Content-Type': undefined }
                }).success(function (data) {
                    if (data) {
                        gridRow.entity.hasImage = true;
                        supplyService.update(gridRow.entity)
                         .success(function (result) {
                             abp.message.success("Success", "Files uploaded successfully.");
                         })
                        .error(function (result) {
                            abp.message.error('Error Occured');
                        });

                    } else {
                        abp.message.error("Error", "Files uploaded unsuccess");
                    }
                })
                .error(function (data, status) {
                    abp.message.error("Error", "Files uploaded unsuccess");
                });
            };

            $scope.gridOptions.columnDefs = [
                { name: 'id', enableCellEdit: false },
                { name: 'code', displayName: 'Code' },
                { name: 'name', displayName: 'Name' },
                { name: 'price', displayName: 'Price', cellFilter: 'currency:"Rp."' },
                { name: 'weight', displayName: 'Weight', type: 'number' },
                { name: 'inStock', displayName: 'Stock', type: 'number' },
                { name: 'available', displayName: 'Active', type: 'boolean' },
                { name: 'description', displayName: 'Description' },
                { name: 'ispo', displayName: "Is PO ?", type: 'boolean' },
                { name: 'availableUntil', displayName: "Available Until", type: 'date' },
                { name: 'hasImage', displayName: "Image", type: 'boolean', enableCellEdit: false },
                { name: 'filename', displayName: 'File', width: '20%', editableCellTemplate: 'ui-grid/fileChooserEditor', editFileChooserCallback: $scope.storeFile }
            ];

            $scope.saveRow = function (rowEntity) {
                var promise = $q.defer();
                supplyService.update(rowEntity)
                    .success(function (result) {
                        promise.resolve();
                        $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
                        $scope.refresh();
                        abp.notify.info('Updated');
                    })
                    .error(function (result) {
                        promise.resolve();
                        $scope.gridApi.rowEdit.setSavePromise(rowEntity, promise.promise);
                        $scope.refresh();
                        abp.notify.error('Error Occured');
                    });
            };

            $scope.gridOptions.onRegisterApi = function (gridApi) {
                $rootScope.gridApi = gridApi;
                gridApi.rowEdit.on.saveRow($scope, $scope.saveRow);
            };

            $scope.open = function () {
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'addNewSupply.html',
                    controller: 'supplyModalCtrl',
                    size: 'm'
                });

                modalInstance.result.then(function (result) {
                    $scope.refresh();
                });
            };

            $scope.delete = function () {
                var rows = $scope.gridApi.selection.getSelectedRows();
                angular.forEach(rows, function (value, key) {
                    supplyService.delete(value.id).success(function () {
                        $scope.refresh();
                    });
                });
            };

            $scope.refresh();
        }
    ]);

    angular.module('app').controller('supplyModalCtrl', [
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