'use strict';

angular.module('matterMain')
  .factory('navigationResource', ['$resource', 'auth',
      function navigationResource($resource, auth) {

          return $resource(null, null,
              {
                  'canCreateMatter': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/cancreate'                     
                  }),				  
              });
      }]);


//https://mattercenterservicewebapi.azurewebsites.net/api
//http://localhost:58775/api/v1/matter/getpinned