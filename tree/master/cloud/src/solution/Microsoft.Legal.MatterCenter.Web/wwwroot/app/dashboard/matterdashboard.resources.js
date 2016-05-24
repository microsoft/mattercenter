'use strict';

angular.module('matterMain')
  .factory('matterDashBoardResource', ['$resource', 'auth',
      function matterDashBoardResource($resource, auth) {
          return $resource(null, null,
              {                  
                 'get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/get',
                      isArray: true
                  }),
                  'getPinDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getpinned',
                      isArray: true
                  }),
                  'getMyDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getpinned',
                      isArray: true
                  }),
              });
      }]);
