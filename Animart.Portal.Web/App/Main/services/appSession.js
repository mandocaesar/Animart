(function () {
    angular.module('app').service('appSession', [
            function () {

                var _session = {
                    user: null,
                    tenant: null
                };

                abp.services.app.user.getCurrentLoginInformations({ async: false }).done(function (result) {
                    _session.user = result.user;
                    _session.tenant = result.tenant;
                });

                return _session;
            }
    ]);
})();