(function () {
    var controllerId = 'app.views.users';
    angular.module('app').controller(controllerId, ['$q', '$rootScope', '$scope', 'abp.services.app.user', '$uibModal',
        function ($q, $rootScope, $scope, userService, $uibModal) {
            var vm = this;

            $scope.roleDropdown = ['Admin', 'Logistic', 'Accounting', 'Marketing', 'Retailer'];
            //$scope.selectedUser = {};
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
                { name: 'userName', displayName: 'User Name', enableCellEdit: false },
                { name: 'email', displayName: 'Email', enableCellEdit: false },
                { name: 'firstName', displayName: 'First Name', enableCellEdit: false },
                { name: 'lastName', displayName: 'Last Name', enableCellEdit: false },
                { name: 'lastLoginTime', displayName: 'Last Login', cellFilter: 'date', enableCellEdit: false },
                {
                    name: 'role', displayName: 'Role', editableCellTemplate: 'ui-grid/dropdownEditor',
                    cellFilter: 'roleFilter', editDropdownValueLabel: 'role', editDropdownOptionsArray: [
                        { id: 'Admin', role: 'Admin' },
                        { id: 'Logistic', role: 'Logistic' },
                        { id: 'Accounting', role: 'Accounting' },
                        { id: 'Marketing', role: 'Marketing' },
                        { id: 'Retailer', role: 'Retailer' }
                    ]
                },
                { name: 'isActive', displayName: 'Active', type: 'boolean', enableCellEdit: false },
                {
                    name: 'edit', displayName: 'Edit',
                    cellTemplate: '<button class="btn btn-success" ng-click="grid.appScope.open(row.entity.id)"><i class="fa fa-pencil"></i> Edit</button>'
                }
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

            $scope.open = function (id) {
                if (id !== null && id !== undefined) {
                    $scope.selectedUser = id;
                } else
                    $scope.selectedUser = null;

                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'userModal.html',
                    scope: $scope,
                    controller: 'userManagementModalCtrl',
                    size: 'm'
                });

                modalInstance.result.then(function () {
                    $scope.selectedUser = {};
                    $scope.refresh();
                });
            };

            $scope.delete = function () {
                var rows = $scope.gridApi.selection.getSelectedRows();
                console.log(rows);
                angular.forEach(rows, function (value, key) {
                    console.log(value);
                    userService.delete(value).success(function () {
                        $scope.refresh();
                    });
                });
            };

            $scope.refresh();
        }
    ]).filter('roleFilter', function () {
        var roleHash = {
            'Admin': 'Admin',
            'Logistic': 'Logistic',
            'Accounting': 'Accounting',
            'Marketing': 'Marketing',
            'Retailer': 'Retailer'
        };

        return function (input) {
            if (!input) {
                return '';
            } else {
                return roleHash[input];
            }
        };
    });

    angular.module('app').controller('userManagementModalCtrl', [
        '$scope', 'abp.services.app.user', '$uibModalInstance',
        function ($scope, userService, $uibModalInstance, result) {
            $scope.roles = ['Admin', 'Logistic', 'Accounting', 'Marketing', 'Retailer'];
            $scope.user = {
                Id:0,
                UserName: '',
                FirstName: '',
                LastName: '',
                Email: '',
                Role: '',
                IsActive:true
            };
            $scope.title = "Add New";
            $scope.init = function() {
                if ($scope.selectedUser !== undefined && $scope.selectedUser !== null) {
                    //console.log($scope.selectedUser);
                    userService.getUser($scope.selectedUser)
                       .success(function (result) {
                           //debugger;
                           $scope.title = "Edit";
                           $scope.user.Id = result.id;
                           $scope.user.UserName = result.userName;
                           $scope.user.FirstName = result.firstName;
                           $scope.user.LastName = result.lastName;
                           $scope.user.Email = result.email;
                           $scope.user.Role = result.role;
                           $scope.user.IsActive = result.isActive;
                        })
                       .error(function () { abp.notify.error('Error Occured'); });
                    
                   // $scope.$apply();
                }
            };

            $scope.ok = function () {
                if ($scope.selectedUser != undefined && $scope.selectedUser != null) {
                    $scope.updateUser();
                } else {
                    $scope.addUser();
                }
            };

            $scope.cancel = function () {
                $uibModalInstance.close();
            };

            $scope.updateUser = function () {
                userService.updateUser($scope.user)
                   .success(function (result) {
                       $scope.result = result;
                       $uibModalInstance.close($scope.result);
                       $uibModalInstance.dismiss('cancel');
                       abp.notify.info('Updated');
                   })
                   .error(function (result) { abp.notify.error('Error Occured'); });
            };

            $scope.addUser = function () {
                userService.createUser($scope.user)
                    .success(function (rs) {
                        $scope.result = result;
                        $uibModalInstance.close($scope.result);
                        $uibModalInstance.dismiss('cancel');
                        abp.notify.info('User has been created');
                    })
                    .error(function (er) {
                        abp.notify.error('Error Occured');
                        $uibModalInstance.dismiss('cancel');
                    });
            }

            $scope.init();
        }
    ]);
})();