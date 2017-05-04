(function () {
    var controllerId = 'app.views.layout.header';
    angular.module('app').controller(controllerId, ['$scope',
        '$rootScope', '$state', 'abp.services.app.user', 'ngCart', '$uibModal', function ($scope, $rootScope, $state, appSession, ngCart, $uibModal) {
            var vm = this;

            vm.languages = abp.localization.languages;
            vm.currentLanguage = abp.localization.currentLanguage;

            vm.menu = abp.nav.menus.MainMenu;
          //  console.log(vm.menu);
            vm.currentMenuName = $state.current.menu;

            $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                vm.currentMenuName = toState.menu;
            });

            var user = null;
            appSession.user = null;

            var refreshUserData = function() {
                appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
                    //console.log(result);
                    user = result.user;
                    $rootScope.Address = user.Address;
                    $rootScope.PhoneNumber = user.PhoneNumber;
                    $scope.luser = user;
                }).error(
                  function (result) {
                      //console.log(result)
                  }
              );
            };

            vm.getShownUserName = function () {
                return user;
            };

            $scope.editProfile = function() {
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'editProfile.html',
                    controller: 'userModalCtrl',
                    scope: $scope,
                    size: 'm'
                });

                modalInstance.result.then(function (result) {
                    //$scope.refresh();
                    refreshUserData();
                });
            };

            $scope.setCheckout = function () {
                $('.navbar-toggle').click();
                window.location.href = "#/checkout";
            };

            $scope.changePassword = function () {
                var modalInstance = $uibModal.open({
                    animation: $scope.animationsEnabled,
                    templateUrl: 'changePassword.html',
                    controller: 'changePasswordCtrl',
                    scope: $scope,
                    size: 'm'
                });

                modalInstance.result.then(function (result) {
                    //$scope.refresh();
                    refreshUserData();
                });
            };

            $scope.$on('ngCart:change', function (event, args) {
                $scope.checkItemQuantity();
            });

            $scope.checkItemQuantity = function () {
                var items = ngCart.getItems();
                var data = null;
                for (var i = 0; i < items.length; i++) {
                    data = items[i].getData();
                    if (!data.ispo) {
                        if (items[i].getQuantity() < 1)
                            items[i]._quantity = 1;
                        else if (items[i].getQuantity() > data.inStock)
                            items[i]._quantity = data.inStock;
                    } else {
                        if (items[i].getQuantity() > 100)
                            items[i]._quantity = 100;
                    }

                }
            };
            $rootScope.checkout = function() {
                alert();
                $state.go('checkout', {});
            };
            refreshUserData();
        }
    ]);

    angular.module('app').controller('userModalCtrl', [
       '$scope', 'abp.services.app.user', '$uibModalInstance',
       function ($scope, userService, $uibModalInstance, result) {
           $scope.user = {};
           $scope.user.Id = $scope.luser.id;
           $scope.user.UserName = $scope.luser.userName;
           $scope.user.Address = $scope.luser.address;
           $scope.user.PhoneNumber = $scope.luser.phoneNumber;
           $scope.user.FirstName = $scope.luser.name;
           $scope.user.LastName = $scope.luser.surname;
           $scope.user.Email = $scope.luser.emailAddress;
           //$scope.ConfirmPassword = "";

           //userService.getCurrentLoginInformations({ async: false }).success(function (result) {
           //    $scope.$apply(function () {
           //        $scope.user = result.user;
           //    });
           //}).error(function (result) { console.log(result) });

           $scope.ok = function () {
               //if ($scope.ConfirmPassword === $scope.user.NewPassword) {
                   //console.log($scope.user);
                   userService.updateUserProfile($scope.user)
                       .success(function(rs) {
                           $scope.result = rs;
                           //console.log(rs);
                            $uibModalInstance.close($scope.result);
                           //$uibModalInstance.dismiss('cancel');
                           if(rs)
                               abp.notify.info('Profile has been updated!');
                           else
                               abp.notify.info('Cannot update profile, please check your input.');
                       })
                       .error(function(er) {
                           abp.notify.error('Error Occured');
                           $uibModalInstance.dismiss('cancel');
                       });
               //} else {
               //    abp.notify.error('New Password not match');
               //}
           };

           $scope.cancel = function () {
               $uibModalInstance.dismiss('cancel');
           };
       }
    ]);

    angular.module('app').controller('changePasswordCtrl', [
       '$scope', 'abp.services.app.user', '$uibModalInstance',
       function ($scope, userService, $uibModalInstance, result) {
           //console.log($scope.luser);
           $scope.user = {};
           $scope.user.Id = $scope.luser.id;
           $scope.user.UserName = $scope.luser.userName;
           $scope.user.Address = $scope.luser.address;
           $scope.user.PhoneNumber = $scope.luser.phoneNumber;
           $scope.user.FirstName = $scope.luser.name;
           $scope.user.LastName = $scope.luser.surname;
           $scope.user.Email = $scope.luser.emailAddress;
           $scope.ConfirmPassword = "";

           //userService.getCurrentLoginInformations({ async: false }).success(function (result) {
           //    $scope.$apply(function () {
           //        $scope.user = result.user;
           //    });
           //}).error(function (result) { console.log(result) });

           $scope.ok = function () {
               if ($scope.ConfirmPassword === $scope.user.NewPassword) {
                   //console.log($scope.user);
                   userService.updatePassword($scope.user)
                       .success(function (rs) {
                           $scope.result = rs;
                           //console.log(rs);
                           $uibModalInstance.close($scope.result);
                           //$uibModalInstance.dismiss('cancel');
                           if(rs)
                               abp.notify.info('Password has been updated!');
                           else
                               abp.notify.info('Cannot update password, please check your old password.');
                       })
                       .error(function (er) {
                           abp.notify.error('Error Occured');
                           $uibModalInstance.dismiss('cancel');
                       });
               } else {
                   abp.notify.error('New Password not match');
               }
           };

           $scope.cancel = function () {
               $uibModalInstance.dismiss('cancel');
           };
       }
    ]);
})();