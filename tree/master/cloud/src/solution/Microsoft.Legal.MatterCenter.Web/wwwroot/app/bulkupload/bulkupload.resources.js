'use strict';
angular.module('matterMain')
  .factory('bulkuploadResource', ['$resource', 'auth',
      function bulkuploadResource($resource, auth) {
          return $resource(null, null,
              {                                    
                  'getUserInboxEmails': auth.attachCSRF({
                        method: 'POST',
                        url: '/api/v1/email/getuserinboxemails',                  
                  }),
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
                  'UnpinMatters': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/unpin'
                  }),
                  'PinMatters': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/pin'
                  }),
                  'getFolderHierarchy': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getfolderhierarchy'

                  }),
                  'uploadEmail': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/email/UploadMail',
                      isArray: true
                  }),
                  'uploadAttachment': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/email/uploadattachmentsofemail',
                      isArray: true
                  }),
                  'uploadfiles': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/document/UploadAttachments'
                  }),
                  'userexists': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/user/userexists'
                  }),
                  'oneNoteUrlExists': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/onenoteurlexists'
                  }),
                  'getmatterprovisionextraproperties': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getmatterprovisionextraproperties'
                  }),
                  'getDefaultMatterConfigurations': auth.attachCSRF({
                      method: 'POST',
                      url: '/api/v1/matter/getconfigurations'
                  }),

              });
      }]);
