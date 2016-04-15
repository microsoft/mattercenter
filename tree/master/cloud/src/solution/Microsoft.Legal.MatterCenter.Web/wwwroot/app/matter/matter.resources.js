'use strict';

angular.module('matterMain')
  .factory('matterResource', ['$resource', 'auth',
      function matterResource($resource, auth) {

          return $resource(null, null,
              {
                  'getMatters': auth.attachCSRF({
                      method: 'GET',
                      url: 'https://mattercenterservicenew.azurewebsites.net/api/v1/taxonomy/getcurrentsitetitle'
                  }),
				  'getPinnedMatters': auth.attachCSRF({
                      method: 'POST',
                      url: 'http://localhost:58775/api/v1/matter/getpinned'
                  }),
              });
      }]);


	  //https://mattercenterservicewebapi.azurewebsites.net/api