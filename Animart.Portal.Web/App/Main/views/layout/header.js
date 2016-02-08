(function () {
    var controllerId = 'app.views.layout.header';
    angular.module('app').controller(controllerId, [
        '$rootScope', '$state', 'abp.services.app.user', function ($rootScope, $state, appSession) {
            var vm = this;

            vm.languages = abp.localization.languages;
            vm.currentLanguage = abp.localization.currentLanguage;

            vm.menu = abp.nav.menus.MainMenu;
            console.log(vm.menu);
            vm.currentMenuName = $state.current.menu;

            $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                vm.currentMenuName = toState.menu;
            });

            var user = null;
            appSession.user = null;

            appSession.getCurrentLoginInformations({ async: false }).success(function (result) {
                user = result.user;
            }).error(function(result) {console.log(result)});

            vm.getShownUserName = function () {
                return user;
            };
        }
    ]);
})();