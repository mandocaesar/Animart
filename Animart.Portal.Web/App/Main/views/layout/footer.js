(function () {
    var controllerId = 'app.views.layout.footer';
    angular.module('app').controller(controllerId, ['$scope',
        '$rootScope', '$state', 'abp.services.app.user', 'ngCart', '$uibModal', function ($scope, $rootScope, $state, appSession, ngCart, $uibModal) {
            var foot = this;
            foot.menu = abp.nav.menus.MainMenu;
            foot.currentMenuName = $state.current.menu;

            $rootScope.$on('$stateChangeSuccess', function (event, toState, toParams, fromState, fromParams) {
                foot.currentMenuName = toState.menu;
            });
        }
    ]);
})();