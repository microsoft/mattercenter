'use strict';

angular.module('matterMain')
  .factory('dashBoardResource', ['$resource', 'auth',
      function dashBoardResource($resource, auth) {
          return $resource(null, null,
              {                  
                 'get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getdocuments',
                      isArray: true
                  }),
                  'getPinDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getpinneddocuments'
                  }),
                  'getMyDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getdocuments'
                  }),
              });
      }]);
