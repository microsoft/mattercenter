'use strict';

angular.module('matterMain')
  .factory('matterDashBoardResource', ['$resource', 'auth',
      function matterDashBoardResource($resource, auth) {
          return $resource(null, null,
              {                  
                 'get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/get',
                      isArray: true
                  }),
                  'getPinnedMatters': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getpinned',
                      isArray: true
                  }),
                  'getMyMatters': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getpinned',
                      isArray: true
                  }),
                  'getTaxonomyDetails': auth.attachCSRF({  
                      method: 'POST',  
                      url: '/api/v1/taxonomy/gettaxonomy'  
                  }),
                  'UnpinMatter': auth.attachCSRF({
				      method: 'POST',
				      url: '/api/v1/matter/unpin'
				  }),
                  'PinMatter':auth.attachCSRF({
				      method: 'POST',
				      url: '/api/v1/matter/pin'
                  }),
                  'getFolderHierarchy': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getfolderhierarchy'
                     
                  }),
                  'getMatterCounts': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getmattercounts'
                     
                  }),
              });
      }]);
