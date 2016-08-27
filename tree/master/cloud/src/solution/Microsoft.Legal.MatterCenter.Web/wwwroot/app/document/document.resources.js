'use strict';

angular.module('matterMain')
  .factory('documentResource', ['$resource', 'auth',
      function documentResource($resource, auth) {

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
                  'unPinDocument': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/unpindocument'
                  }),
                  'pinDocument': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/pindocument'
                  }),
                  'getassets': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getassets'
                  })
              });
      }]);


//https://mattercenterservicewebapi.azurewebsites.net/api
//http://localhost:58775/api/v1/matter/getpinned