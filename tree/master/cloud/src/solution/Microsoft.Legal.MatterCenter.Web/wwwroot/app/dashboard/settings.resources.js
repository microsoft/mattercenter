'use strict';

angular.module('matterMain')
  .factory('settingsResource', ['$resource', 'auth',
      function settingsResource($resource, auth) {
          return $resource(null, null,
              {
                  'getTaxonomyData': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/taxonomy/gettaxonomy'
                  })
              });
      }]);
