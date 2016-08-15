(function(window, angular, undefined) {'use strict';

/**
 * @ngdoc overview
 * @name angulartics.google.analytics
 * Enables analytics support for Google Analytics (http://google.com/analytics)
 */
angular.module('angulartics.google.analytics', ['angulartics'])
.config(['$analyticsProvider', function ($analyticsProvider) {

  // GA already supports buffered invocations so we don't need
  // to wrap these inside angulartics.waitForVendorApi

  $analyticsProvider.settings.pageTracking.trackRelativePath = true;

  // Set the default settings for this module
  $analyticsProvider.settings.ga = {
    additionalAccountNames: undefined,
    disableEventTracking: null,
    disablePageTracking: null,
    userId: null
  };

  function dimensionsAndMetrics(properties) {
    if (window.ga) {
      // add custom dimensions and metrics
      var customData = {};
      for(var idx = 1; idx<=200;idx++) {
        if (typeof properties['dimension' + idx] !== 'undefined') {
          customData['dimension' + idx] = properties['dimension' + idx];
        }
        if (typeof properties['metric' +idx] !== 'undefined') {
          customData['metric' + idx] = properties['metric' + idx];
        }
      }
      return customData;
    }
  }

  $analyticsProvider.registerPageTrack(function (path) {
    
    // Do nothing if page tracking is disabled
    if ($analyticsProvider.settings.ga.disablePageTracking) return;

    if (window._gaq) {
      _gaq.push(['_trackPageview', path]);
      angular.forEach($analyticsProvider.settings.ga.additionalAccountNames, function (accountName){
        _gaq.push([accountName + '._trackPageview', path]);
      });
    }
    if (window.ga) {
      if ($analyticsProvider.settings.ga.userId) {
        ga('set', 'userId', $analyticsProvider.settings.ga.userId);
      }
      ga('send', 'pageview', path);
      angular.forEach($analyticsProvider.settings.ga.additionalAccountNames, function (accountName){
        ga(accountName +'.send', 'pageview', path);
      });
    }
  });

  /**
   * Track Event in GA
   * @name eventTrack
   *
   * @param {string} action Required 'action' (string) associated with the event
   * @param {object} properties Comprised of the mandatory field 'category' (string) and optional  fields 'label' (string), 'value' (integer) and 'nonInteraction' (boolean)
   *
   * @link https://developers.google.com/analytics/devguides/collection/gajs/eventTrackerGuide#SettingUpEventTracking
   *
   * @link https://developers.google.com/analytics/devguides/collection/analyticsjs/events
   */
  $analyticsProvider.registerEventTrack(eventTrack);

  function eventTrack (action, properties) {

    // Do nothing if event tracking is disabled
    if ($analyticsProvider.settings.ga.disableEventTracking) return;

    // Google Analytics requires an Event Category
    if (!properties || !properties.category) {
      properties = properties || {};
      properties.category = 'Event';
    }
    // GA requires that eventValue be an integer, see:
    // https://developers.google.com/analytics/devguides/collection/analyticsjs/field-reference#eventValue
    // https://github.com/luisfarzati/angulartics/issues/81
    if (properties.value) {
      var parsed = parseInt(properties.value, 10);
      properties.value = isNaN(parsed) ? 0 : parsed;
    }

    // GA requires that hitCallback be an function, see:
    // https://developers.google.com/analytics/devguides/collection/analyticsjs/sending-hits#hitcallback
    if (properties.hitCallback && (typeof properties.hitCallback !== 'function')) {
      properties.hitCallback = null;
    }

    // Making nonInteraction parameter more intuitive, includes backwards compatibilty
    // https://github.com/angulartics/angulartics-google-analytics/issues/49
    if (!properties.hasOwnProperty('nonInteraction')) {
      properties.nonInteraction = properties.noninteraction;
    }

    if (window.ga) {

      var eventOptions = {
        eventCategory: properties.category,
        eventAction: action,
        eventLabel: properties.label,
        eventValue: properties.value,
        nonInteraction: properties.nonInteraction,
        page: properties.page || window.location.hash.substring(1) || window.location.pathname,
        userId: $analyticsProvider.settings.ga.userId,
        hitCallback: properties.hitCallback
      };

      // Round up any dimensions and metrics for this hit
      var dimsAndMets = dimensionsAndMetrics(properties);
      angular.extend(eventOptions, dimsAndMets);

      // Add transport settings
      if($analyticsProvider.settings.ga.transport) {
        angular.extend(eventOptions, $analyticsProvider.settings.ga.transport);
      }

      ga('send', 'event', eventOptions);

      angular.forEach($analyticsProvider.settings.ga.additionalAccountNames, function (accountName){
        ga(accountName +'.send', 'event', eventOptions);
      });

    } else if (window._gaq) {
      _gaq.push(['_trackEvent', properties.category, action, properties.label, properties.value, properties.nonInteraction]);
    }

  }

  /**
   * Exception Track Event in GA
   * @name exceptionTrack
   * Sugar on top of the eventTrack method for easily handling errors
   *
   * @param {object} error An Error object to track: error.toString() used for event 'action', error.stack used for event 'label'.
   * @param {object} cause The cause of the error given from $exceptionHandler, not used.
   *
   * @link https://developers.google.com/analytics/devguides/collection/analyticsjs/events
   */
  $analyticsProvider.registerExceptionTrack(function (error, cause) {
    eventTrack(error.toString(), {
      category: 'Exceptions',
      label: error.stack,
      nonInteraction: true
    });
  });

  /**
   * Set Username
   * @name setUsername
   *
   * @param {string} userId Registers User ID of user for use with other hits
   *
   * @link https://developers.google.com/analytics/devguides/collection/analyticsjs/cookies-user-id#user_id
   */
  $analyticsProvider.registerSetUsername(function (userId) {
    $analyticsProvider.settings.ga.userId = userId;
  });

  /**
   * Set User Properties
   * @name setUserProperties
   *
   * @param {object} properties Sets all properties with dimensionN or metricN to their respective values
   *
   * @link https://developers.google.com/analytics/devguides/collection/analyticsjs/field-reference#customs
   */
  $analyticsProvider.registerSetUserProperties(function (properties) {
    if(properties) {
      // add custom dimensions and metrics to each hit
      var dimsAndMets = dimensionsAndMetrics(properties);
      ga('set', dimsAndMets);
    }
  });

  /**
   * User Timings Event in GA
   * @name userTimings
   *
   * @param {object} properties Comprised of the mandatory fields:
   *     'timingCategory' (string),
   *     'timingVar' (string),
   *     'timingValue' (number)
   * Properties can also have the optional fields:
   *     'timingLabel' (string)
   *
   * @link https://developers.google.com/analytics/devguides/collection/analyticsjs/user-timings
   */
  $analyticsProvider.registerUserTimings(function (properties) {
    if (!properties || !properties.timingCategory || !properties.timingVar || typeof properties.timingValue === 'undefined') {
      console.log('Properties timingCategory, timingVar, and timingValue are required to be set.');
      return;
    }

    if(window.ga) {
      ga('send', 'timing', properties);
    }
  });

}]);
})(window, window.angular);
