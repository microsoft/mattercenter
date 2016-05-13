'use strict';

angular.module('drag_n_drop', [])
    .provider('dndDragAndDropConfig', function () {

        var defaults = {
                droppableOptions: {
                    addClasses: false,
                    tolerance: 'pointer',
                    greedy: true,
                    removeHoverClassEventName: 'dndRemoveHoverClass'
                },
                draggableOptions: {
                    addClasses: false,
                    helper: 'clone',
                    opacity: 0.35,
                    appendTo: 'body'
                }
            },
            userOptions = {};

        return {
            setGlobalDroppableOptions : function (options) {
                userOptions.droppableOptions = options;
            },
            setGlobalDraggableOptions : function (options) {
                userOptions.draggableOptions = options;
            },
            $get: function () {
                return {
                    droppableOptions: angular.extend({}, defaults.droppableOptions, userOptions.droppableOptions),
                    draggableOptions: angular.extend({}, defaults.draggableOptions, userOptions.draggableOptions)
                }
            }
        }
    })
    .directive('dndDroppable', ['$parse', 'dndDragAndDropConfig', '$timeout', '$rootScope', function($parse, dndDragAndDropConfig, $timeout, $rootScope) {
        return {
            restrict: 'A',
            link: function(scope, element, attrs) {

                var config = angular.extend({}, dndDragAndDropConfig.droppableOptions, scope.$eval(attrs.dndDroppable)),
                    handlersConfig = {},
                    watchOptions = {},
                    needDropTimeout = config.activeClass && attrs.onAccept,
                    needToRemoveHoverClass = config.hoverClass && config.greedy,
                    apply = scope.$apply.bind(scope);

                attrs.onDrop && createHandler(handlersConfig, 'drop', $parse(attrs.onDrop));
                attrs.onActivate && createHandler(handlersConfig, 'activate', $parse(attrs.onActivate));
                attrs.onDeactivate && createHandler(handlersConfig, 'deactivate', $parse(attrs.onDeactivate));
                attrs.onOut && createHandler(handlersConfig, 'out', $parse(attrs.onOut));
                attrs.onOver && createHandler(handlersConfig, 'over', $parse(attrs.onOver));
                attrs.onCreate && createHandler(handlersConfig, 'create', $parse(attrs.onCreate));
                attrs.onAccept && createAcceptHandler(handlersConfig, $parse(attrs.onAccept));

                if (config && config.watchOptions) {
                    config.watchOptions.forEach(function (optionName) {
                        watchOptions[optionName] = scope.$eval(config[optionName]);
                        scope.$watch(config[optionName], function (value) {
                            element.droppable('option', optionName, value);
                        });
                    });
                }

                element.droppable(angular.extend(config, watchOptions, handlersConfig));

                needToRemoveHoverClass && scope.$on(dndDragAndDropConfig.droppableOptions.removeHoverClassEventName, function () {
                    element.removeClass(config.hoverClass);
                });

                function createHandler(config, name, handler) {
                    var applyFunc;

                    if (handler) {
                        applyFunc = (name === 'drop' && needDropTimeout) ? $timeout : apply;

                        config[name] = function (event, ui) {
                            applyFunc(function () {
                                handler(scope, {
                                    draggableScope: ui && angular.element(ui.draggable).controller('dndDraggable').scope(),
                                    droppableScope: scope,
                                    $event: event
                                });

                                needToRemoveHoverClass && $timeout(function () {
                                    $rootScope.$broadcast(dndDragAndDropConfig.droppableOptions.removeHoverClassEventName);
                                });
                            });
                        };
                    }
                }

                function createAcceptHandler(config, handler) {
                    if (handler) {
                        config.accept = function (element) {
                            var ctrl = angular.element(element).controller('dndDraggable');
                            return ctrl && handler(scope, {
                                draggableScope: ctrl.scope(),
                                droppableScope: scope
                            });
                        };
                    }
                }
            }
        };
    }])
    .directive('dndDraggable', ['dndDragAndDropConfig', function(dndDragAndDropConfig) {
        return {
            restrict: 'A',
            controller: ['$scope', function ($scope) {
                this.scope = function () {
                    return $scope;
                };
            }],
            link: function(scope, element, attrs) {
                var config = scope.$eval(attrs.dndDraggable),
                    handlersConfig = {},
                    watchOptions = {};

                attrs.onCreate && createHandler(handlersConfig, 'create', $parse(attrs.onCreate));
                attrs.onDrag && createHandler(handlersConfig, 'drag', $parse(attrs.onDrag));
                attrs.onStart && createHandler(handlersConfig, 'start', $parse(attrs.onStart));
                attrs.onStop && createHandler(handlersConfig, 'stop', $parse(attrs.onStop));

                if (config && config.watchOptions) {
                    config.watchOptions.forEach(function (optionName) {
                        watchOptions[optionName] = scope.$eval(config[optionName]);
                        scope.$watch(config[optionName], function (value) {
                            element.draggable('option', optionName, value);
                        });
                    });
                }

                element.draggable(angular.extend({}, dndDragAndDropConfig.draggableOptions, config, watchOptions, handlersConfig));

                function createHandler(config, name, handler) {
                    if (handler) {
                        config[name] = function (event) {
                            scope.$apply(function (scope) {
                                handler(scope, {
                                    draggableScope: scope,
                                    $event: event
                                });
                            });
                        };
                    }
                }
            }
        };
    }]);
