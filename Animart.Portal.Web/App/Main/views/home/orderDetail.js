(function () {
    Number.prototype.padLeft = function(base, chr) {
        var len = (String(base || 10).length - String(this).length) + 1;
        return len > 0 ? new Array(len).join(chr || '0') + this : this;
    };
    var controllerId = 'app.views.orderDetail';
    angular.module('app').controller(controllerId, [
        '$scope', 'abp.services.app.order', 'abp.services.app.user', '$stateParams', '$sce',
        function($scope, orderService, appSession, stateParams, $sce) {
            if (!(abp.auth.isGranted('CanAccessLogistic') || abp.auth.isGranted('CanAccessAdministrator')
                || abp.auth.isGranted('CanAccessAccounting') || abp.auth.isGranted('CanAccessMarketing')
                || abp.auth.isGranted('CanAccessRetailer')))
                window.location.href = "#";
            else {
                var itemID = stateParams.id;
                var d = new Date();
                $scope.printedDate = [
                    d.getDate().padLeft(),
                    (d.getMonth() + 1).padLeft(),
                    d.getFullYear()
                ].join('-') + ',' +
                [
                    d.getHours().padLeft(),
                    d.getMinutes().padLeft(),
                    d.getSeconds().padLeft()
                ].join(':');
                $scope.itemID = itemID;

                var vm = this;

                var user = null;
                appSession.user = null;

                $scope.po = {
                    address: '',
                    province: '',
                    city: '',
                    isPreOrder: false,
                    postalCode: '',
                    expedition: '',
                    expeditionAdjustment: '',
                    isAdjustment: false,
                    grandTotal: 0,
                    totalWeight: 0,
                    totalGram: 0,
                    status: 'MARKETING',
                    shipping: 0,
                    showExpedition: false
                };

                orderService.getSinglePurchaseOrder(itemID).success(function(result) {
                    //console.log(result);
                    $scope.po = result;
                    if ($scope.po.expedition != $scope.po.expeditionAdjustment)
                        $scope.po.isAdjustment = true;
                    $scope.isPayment = (result.status === "PAID" || result.status === "DONE" || result.status === "LOGISTIC") || result.status === "PAYMENT";
                    $scope.isPaid = result.status === "PAID";
                    $scope.isNeedPayment = result.status === "PAYMENT";
                    $scope.isDone = result.status === "LOGISTIC" || result.status === "DONE";
                    $scope.supplies = result.items;
                    $scope.isBod = result.status === "ACCOUNTING";
                    $scope.status = (result.isPreOrder) ? "Pre-Order" : "Ready Stock";
                    //console.log($scope.supplies);
                    $scope.image = "";
                    if ($scope.isPayment) {
                        $scope.image = '../UserImage/' + $scope.po.id + ".jpg";
                    }
                });

                $scope.file = {};

                $scope.getSubTotal = function() {
                    var total = 0;
                    if ($scope.supplies != null)
                        for (var i = 0; i < $scope.supplies.length; i++) {
                            var product = $scope.supplies[i];
                            total += (product.priceAdjustment * product.quantityAdjustment);
                        }
                    $scope.po.grandTotal = total;
                    return total;
                }

                $scope.getFile = function(e) {
                    $scope.$apply(function() {
                        $scope.files = e.files;
                    });
                }

                $scope.print = function() {
                    $(".printDialog").hide();
                    var pdf = new jsPDF('p', 'pt', 'letter');
                    pdf.addHTML($('#printContent')[0], function() {

                        pdf.save('Test.pdf');
                    });
                    $(".printDialog").show();

                };

                $scope.renderHtml = function(htmlCode) {
                    return $sce.trustAsHtml(htmlCode);
                };

                appSession.getCurrentLoginInformations({ async: false }).success(function(result) {
                    user = result.user;
                    $scope.luser = user;
                }).error(
                    function(result) {
                        //console.log(result);
                    }
                );
                vm.getShownUserName = function() {
                    return user;
                };
            }
        }
    ]);
})();