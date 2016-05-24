'use strict';

angular.module('matterMain')
  .factory('documentDashBoardResource', ['$resource', 'auth',
      function documentDashBoardResource($resource, auth) {
          return $resource(null, null,
              {                  
                 'get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getdocuments',
                      isArray: true
                  }),
                 'getPinnedDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getpinneddocuments',
                      isArray: true
                  }),
                  'getMyDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getdocuments',
                      isArray: true
                  }),
              });
      }]);
