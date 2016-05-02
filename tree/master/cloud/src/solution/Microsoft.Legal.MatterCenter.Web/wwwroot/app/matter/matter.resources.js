'use strict';

angular.module('matterMain')
  .factory('matterResource', ['$resource', 'auth',
      function matterResource($resource, auth) {

          return $resource(null, null,
              {
                  'get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/get',
                      isArray: true
                  }),
				  'getPinnedMatters': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getpinned'
				  }),
				  'UnpinMatters': auth.attachCSRF({
				      method: 'POST',
				      url: '/api/v1/matter/unpin'
				  }),
                  'PinMatters':auth.attachCSRF({
				      method: 'POST',
				      url: '/api/v1/matter/pin'
                  }),
                  'getTaxonomyDetails': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/taxonomy/gettaxonomy'
                  }),
              });
      }]);


//https://mattercenterservicewebapi.azurewebsites.net/api
//http://localhost:58775/api/v1/matter/getpinned