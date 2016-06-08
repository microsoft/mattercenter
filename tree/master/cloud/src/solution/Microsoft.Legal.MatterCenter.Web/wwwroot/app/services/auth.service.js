'use strict';

/**
 * Service that can be used by the repository modules to attach CSRF tokens to the request object.
 * 
 * @module {Service} auth
 */
angular.module('matterMain')
  .factory('auth', function auth() {

      function retrieveCSRF() {
          return angular.element('input[name="__RequestVerificationToken"]').val();
      }

      function getCurrentDate() {
          var date = new Date();
          return (("0" + (date.getMonth() + 1)).slice(-2) + '/' + ("0" + date.getDate()).slice(-2) + '/' + date.getFullYear()).toString();
      }

      return {

          /**
           * Attaches CSRF Token to the {request_object} and returns it.
           * 
           * @method attachCSRF
           * @param request_object {Object} request object to attach CSRF token info.
           * @returns {Object} request object with CSRF token attached.
           */
          attachCSRF: function (request_object) {

              return angular.extend(request_object, {
                  headers: {
                      'Content-Type': 'application/json',
                      'X-XSRF-Token': retrieveCSRF(),
                 //     'X-Requested-With': 'XMLHttpRequest',
				//	  'Authorization': 'Bearer ' + configs.token,
                //      'UserDate': getCurrentDate()
          }
              });
          },

          /**
           * Returns the CSRF token
           * 
           * @method getCSRF
           * @return {String} request verification token
           */
          getCSRF: function () {
              return retrieveCSRF();
          }
      }
  });