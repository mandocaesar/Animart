(function () {
    var controllerId = 'app.views.supply';
    angular.module('app').controller(controllerId, [
        '$rootScope', '$scope', 'abp.services.app.supply', function ($rootScope, $scope, supplyService) {
            var vm = this;
            $scope.supplies = [];
            supplyService.getSupplies().success(function(result) {
                for (var i = 0; i < result.length; i++) {
                    $scope.supplies.push(result[i]);
                }
            });
        }
    ]);
})();