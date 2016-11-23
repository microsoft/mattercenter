'use strict';

angular.module('matterMain')
  .factory('settingsResource', ['$resource', 'auth',
      function settingsResource($resource, auth) {
          return $resource(null, null,
              {
                  'getTaxonomyData': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/taxonomy/gettaxonomy'
                  }),
                  'getRoles': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/user/getroles',
                      isArray: true
                  }),
                  'getPermissionLevels': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/user/getpermissionlevels',
                      isArray: true
                  }),
                  'getUsers': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/user/getusers',
                      isArray: true
                  }),
                  'getDefaultConfigurations': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getconfigurations'
                  }),
                  'saveConfigurations': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/saveconfigurations'
                  }),
                  'isLoginUserOwner': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/user/isowner'
                  }),
              });
      }]);
