'use strict';

angular.module('matterMain')
  .factory('matterResource', ['$resource', 'auth',
      function matterResource($resource, auth) {

          return $resource(null, null,
              {
                  'get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/get'
                  }),

                  'getMatters': auth.attachCSRF({
                      method: 'GET',
                      url: '/api/v1/taxonomy/getcurrentsitetitle'
                  }),
				  'getPinnedMatters': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getpinned'
                  }),
              });
      }]);


//https://mattercenterservicewebapi.azurewebsites.net/api
//http://localhost:58775/api/v1/matter/getpinned