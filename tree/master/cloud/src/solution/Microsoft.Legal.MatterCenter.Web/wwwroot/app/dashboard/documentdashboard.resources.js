'use strict';

angular.module('matterMain')
  .factory('documentDashBoardResource', ['$resource', 'auth',
      function documentDashBoardResource($resource, auth) {
          return $resource(null, null,
              {                  
                 'get': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getdocuments',
                      isArray: true
                  }),
                 'getPinnedDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getpinneddocuments',
                      isArray: true
                  }),
                  'getMyDocuments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getdocuments',
                      isArray: true
                  }),
                  'getTaxonomyDetails': auth.attachCSRF({  
                      method: 'POST',  
                      url: '/api/v1/taxonomy/gettaxonomy'  
                  }),
                  'unPinDocument': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/unpindocument'
                  }),
                  'pinDocument': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/pindocument'
                  }),
                  'downloadattachmentsasstream': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/email/downloadattachmentsasstream'
                  }),
                  'downloadAttachments': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/email/downloadattachments'
                  }),
                  'getDocumentCounts': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getdocumentcounts'
                     
                  }),
                  'getassets': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/getassets'
                  })
              });
      }]);
