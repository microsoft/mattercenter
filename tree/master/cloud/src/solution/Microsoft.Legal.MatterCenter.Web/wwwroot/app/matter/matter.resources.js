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
                  'checkMatterExists': auth.attachCSRF({
                      method: 'POST',  
                      url: '/api/v1/matter/checkmatterexists'
                  }),
                  'getDefaultMatterConfigurations':auth.attachCSRF({  
                      method: 'POST',  
                      url: '/api/v1/matter/getconfigurations'  
                  }),
                  'getUsers': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/user/getusers',
                      isArray: true
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
                   'checkSecurityGroupExists': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/checksecuritygroupexists'
                     
                  }),
                  'getFolderHierarchy': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getfolderhierarchy'
                     
                  }),
                  'createMatter': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/create'

                  }),
                  'assignUserPermissions': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/assignuserpermissions'

                  }),
                   'assignContentType': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/assigncontenttype'

                  }),
                   'createLandingPage': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/createlandingpage'
                  }),
                   'updateMatterMetadata': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/UpdateMetadata'
                  }),
                  'getStampedProperties':auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getstampedproperties'
                  })
                  

              });
      }]);


//https://mattercenterservicewebapi.azurewebsites.net/api
//http://localhost:58775/api/v1/matter/getpinned