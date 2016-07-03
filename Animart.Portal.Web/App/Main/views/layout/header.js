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

            appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
                //console.log(result);
                user = result.user;
                $rootScope.Address = user.Address;
                $scope.luser = user;
            }).error(
                function (result) {
                    //console.log(result)
                }
            );

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
                    $scope.refresh();
                });
            };

            $rootScope.checkout = function() {
                alert();
                $state.go('checkout', {});
            };
        }
    ]);

    angular.module('app').controller('userModalCtrl', [
       '$scope', 'abp.services.app.user', '$uibModalInstance',
       function ($scope, userService, $uibModalInstance, result) {
           $scope.user = {};
           $scope.user.Id = $scope.luser.id;
           $scope.user.UserName = $scope.luser.userName;
           $scope.user.Address = $scope.luser.address;
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
                   userService.updateUserProfile($scope.user)
                       .success(function(rs) {
                           $scope.result = result;
                           // $uibModalInstance.close($scope.result);
                           $uibModalInstance.dismiss('cancel');
                           abp.notify.info('Profile has been updated!');
                       })
                       .error(function(er) {
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