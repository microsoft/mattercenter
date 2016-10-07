'use strict';

angular.module('matterMain')
  .factory('adminResource', ['$resource', 'auth',
      function adminResource($resource, auth) {
          return $resource(null, null,
              {
                  'Get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/config/Get',
                      isArray: true
                  }),                 
                  'getconfigsforspo': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/config/getconfigsforspo'                      
                  }),
                
              });
      }]);
