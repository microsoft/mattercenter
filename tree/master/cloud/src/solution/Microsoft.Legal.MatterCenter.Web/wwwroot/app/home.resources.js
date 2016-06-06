'use strict';

angular.module('matterMain')
  .factory('homeResource', ['$resource', 'auth',
      function homeResource($resource, auth) {
          return $resource(null, null,
              {
                  'getHelp': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/shared/help',
                      isArray: true
                  })
              });
      }]);