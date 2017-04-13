/**
 *
 * jQuery module for accessibility
 *
 * The MIT License (MIT)
 *
 * Copyright (c) 2013, 2014 Dylan Barrell
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy of
 * this software and associated documentation files (the "Software"), to deal in
 * the Software without restriction, including without limitation the rights to
 * use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
 * the Software, and to permit persons to whom the Software is furnished to do so,
 * subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS
 * FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER
 * IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN
 * CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 *
 */

(function (jQuery) {
    var $politeAnnouncer = jQuery("#jquery-a11yfy-politeannouncer"),
        $assertiveAnnouncer = jQuery("#jquery-a11yfy-assertiveannouncer"),
        methods = {
            showAndFocus: function (focus) {
                var $focus = focus ? jQuery(focus) : focus;
                return this.each(function (index, value) {
                    var $this = jQuery(value);

                    $this.show();
                    if ($focus && $focus.length) {
                        if (platform === "iOS") {
                            jQuery("body").focus();
                            setTimeout(function () {
                                $focus.focus();
                            }, 1000);
                        } else {
                            $focus.focus();
                        }
                    }
                });
            },
            focus: function () {
                return this.each(function (index, value) {
                    var $this = jQuery(value);

                    if (platform === "iOS") {
                        jQuery("body").focus();
                        setTimeout(function () {
                            $this.focus();
                        }, 1000);
                    } else {
                        $this.focus();
                    }
                });
            },
            validate: function (options) {
                var opts = jQuery.extend({}, jQuery.fn.a11yfy.defaults.validate, options);
                return this.each(function (index, value) {

                    function errorPlacement() {
                        // do nothing - overrides default behavior
                    }
                    function showErrors() {
                        // do nothing - overrides default behavior
                    }

                    function invalidHandler(event, validator) {
                        var id, invalidIds = [],
                            $this = jQuery(this),
                            $errorSummary = $this.find(".a11yfy-error-summary"),
                            $errorSummaryList = jQuery("<ul>");

                        for (id in validator.invalid) {
                            if (validator.invalid.hasOwnProperty(id)) {
                                invalidIds.push(id);
                            }
                        }

                        // remove any previous validation markup
                        $this.find("a.a11yfy-skip-link").remove(); // remove all the old skip links
                        $this.find(".a11yfy-validation-error").removeClass("a11yfy-validation-error"); // Remove old validation errors
                        $this.find(".a11yfy-error-message").remove(); // remove the old error messages
                        $errorSummary.empty();

                        jQuery(invalidIds).each(function (index, invalidId) {
                            var $input = jQuery("#" + invalidId),
                                $label = jQuery("label[for=\"" + invalidId + "\"]"),
                                $next, $span;
                            $label.addClass("a11yfy-validation-error");
                            $input.addClass("a11yfy-validation-error");

                            // create the summary entry
                            $errorSummaryList.append("<li><a class=\"a11yfy-skip-link a11yfy-summary-link\" href=\"#" + invalidId + "\">" + $label.text() + "</a>" + " : " + validator.invalid[invalidId] + "</li>");

                            // add link to the next field with a validation error
                            if (index < (invalidIds.length - 1) && opts.skipLink) {
                                $next = jQuery("<a href=\"#\" class=\"a11yfy-skip-link\">");
                                $next.text(jQuery.a11yfy.getI18nString("skipToNextError", undefined, jQuery.fn.a11yfy.defaults.strings));
                                $next.attr("href", "#" + invalidIds[index + 1]);
                                if ($input.parent()[0].nodeName === "P") {
                                    $input.parent().after($next);
                                } else {
                                    $input.after($next);
                                }
                            }

                            // Add the error message into the label
                            $span = jQuery("<span class=\"a11yfy-error-message\">");
                            $span.text(" - " + validator.invalid[invalidId]);
                            $label.append($span);
                        });
                        if (opts.summary) {
                            // Add the summary to the document
                            $errorSummary.append($errorSummaryList);
                        }
                    }
                    var $this = jQuery(value),
                        vOptions = jQuery.extend({}, opts.validatorOptions, {
                            invalidHandler: invalidHandler,
                            errorPlacement: errorPlacement,
                            showErrors: showErrors
                        });

                    $this.validate(vOptions);
                    if (opts.skipLink) {
                        $this.delegate("a.a11yfy-skip-link", "click", function (e) {
                            var $target = jQuery(e.target);

                            jQuery($target.attr("href")).select().focus();
                            e.preventDefault();
                            e.stopPropagation();
                        });
                    }
                    $this.children().first().before(
                        jQuery("<div class=\"a11yfy-error-summary\" role=\"alert\" aria-live=\"assertive\">")
                    );
                    // Add the aria-required attributes to all the input elements that have the required
                    // attribute
                    $this.find("[required]").attr("aria-required", "true");
                });
            },
            menu: function () {
                return this.each(function (index, value) {
                    var $this = jQuery(value),
                        $menu = $this;

                    if (value.nodeName !== "UL") {
                        throw new Error("The menu container must be an unordered list");
                    }
                    /* First make all anchor tags in the structure non-naturally focussable */
                    $this.find("a").attr("tabindex", "-1");
                    /* Set the roles for the menubar */
                    $this.attr("role", "menubar").addClass("a11yfy-top-level-menu");
                    /* set the aria attributes and the classes for the sub-menus */
                    $this.find(">li>ul")
                        .addClass("a11yfy-second-level-menu")
                        .parent()
                            .addClass("a11yfy-has-submenu")
                            .attr("aria-haspopup", "true");
                    $this.find(">li>ul>li>ul")
                        .addClass("a11yfy-third-level-menu")
                        .parent()
                            .addClass("a11yfy-has-submenu")
                            .attr("aria-haspopup", "true");
                    /*
                     * Set up the keyboard and mouse handlers for all the individual menuitems
                     */
                    $this.find("li").each(function (index, value) {
                        /* Set the roles for the sub-menus and the menuitems */
                        var $this = jQuery(value);

                        $this.attr({
                            "role": "menuitem",
                            "tabindex": "-1"
                        });
                        $this.find("ul").each(function (index, value) {
                            jQuery(value).attr("role", "menu");
                        });

                    }).on("keypress", function (e) {
                        /*
                         * This implements the WAI-ARIA-PRACTICES keyboard functionality where
                         * pressing the key, corresponding to the first letter of a VISIBLE element
                         * will move the focus to the first such element after the currently focussed
                         * element
                         */
                        var keyCode = e.charCode || e.which || e.keyCode,
                            keyString = String.fromCharCode(keyCode).toLowerCase(),
                            ourIndex = -1,
                            currentItem = this,
                            $this = jQuery(this),
                            $nextItem, $prevItem,
                            $menuitems = $menu.find("li[role=\"menuitem\"]:visible");

                        if (keyCode === 9) {
                            return true;
                        }

                        $menuitems.each(function (index, value) {
                            if (value === currentItem) {
                                ourIndex = index;
                            }
                            if (index > ourIndex && !$nextItem) {
                                if (jQuery(value).text().trim().toLowerCase().indexOf(keyString) === 0) {
                                    if (ourIndex !== -1) {
                                        $nextItem = jQuery(value);
                                    } else if (!$prevItem) {
                                        $prevItem = jQuery(value);
                                    }
                                }
                            }
                        });
                        if (!$nextItem && $prevItem) {
                            $nextItem = $prevItem;
                        }
                        if ($nextItem) {
                            $nextItem.attr("tabindex", "0").focus();
                            $this.attr("tabindex", "-1");
                            if ($nextItem.parent().get(0) !== $this.parent().get(0)) {
                                $this.parent().parent("li").removeClass("open");
                            }
                        }
                        e.stopPropagation();
                    }).on("keydown", function (e) {
                        /*
                         * This implements the WAI-ARIA-PRACTICES keyboard navigation functionality
                         */
                        var keyCode = e.which || e.keyCode,
                            handled = false,
                            $this = jQuery(this);

                        if (e.ctrlKey || e.shiftKey || e.altKey || e.metaKey) {
                            // not interested
                            return;
                        }
                        /*
                         * Open a sub-menu and place focus on the first menuitem within it
                         */
                        function openMenu() {
                            if ($this.hasClass("a11yfy-has-submenu")) {
                                $this.addClass("open").find(">ul>li:visible").first().attr("tabindex", "0").focus();
                                $this.attr("tabindex", "-1");
                            }
                        }
                        /*
                         * Move the focus to the menuitem preceding the current menuitem
                         */
                        function prevInMenu() {
                            var $context = $this;
                            $this.attr("tabindex", "-1");
                            while (true) {
                                if ($context.prev().is(':visible')) {
                                    $context.prev().attr("tabindex", "0").focus();
                                    return
                                }
                                $context = $context.prev();
                                if (!$context.prev().length) {
                                    $context = $this.parent().find(">li").last();
                                    if ($context.is(':visible')) {
                                        $context.attr("tabindex", "0").focus();
                                        return
                                    }
                                }
                                if ($context[0] === $this[0]) {
                                    $this.attr("tabindex", "0")
                                    break;
                                }
                            }
                        }
                        /*
                         * Move the focus to the next menuitem after the currently focussed menuitem
                         */
                        function nextInMenu() {
                            var $context = $this;
                            $this.attr("tabindex", "-1");
                            while (true) {
                                if ($context.next().is(':visible')) {
                                    $context.next().attr("tabindex", "0").focus();
                                    return
                                }
                                $context = $context.next();
                                if (!$context.next().length) {
                                    $context = $this.parent().find(">li").first();
                                    if ($context.is(':visible')) {
                                        $context.attr("tabindex", "0").focus();
                                        return
                                    }
                                }
                                if ($context[0] === $this[0]) {
                                    $this.attr("tabindex", "0")
                                    break;
                                }
                            }
                        }
                        switch (keyCode) {
                            case 32: // space
                            case 13: // enter
                                handled = true;
                                if ($this.find(">a").length) {
                                    if ($this.find(">a")[0].click) {
                                        /* If this is a leaf node, activate it*/
                                        $this.find(">a")[0].click();
                                    } else {
                                        // This is a hack for PhantomJS
                                        $this.find(">a").first().trigger("click");
                                    }
                                } else {
                                    /* If it has a sub-menu, open the sub-menu */
                                    openMenu();
                                }
                                break;
                            case 37: //left
                            case 27: //esc
                                handled = true;
                                if (keyCode === 37 && $this.parent().hasClass("a11yfy-top-level-menu")) {
                                    /* If in the menubar, then simply move to the previous menuitem */
                                    prevInMenu();
                                } else {
                                    if ($this.parent().attr("role") === "menu") {
                                        // this is part of a submenu, set focus on containing li
                                        $this.parent().parent().attr("tabindex", "0").focus()
                                            .removeClass("open");
                                        $this.attr("tabindex", "-1");
                                    }
                                }
                                break;
                            case 38: //up
                                handled = true;
                                if ($this.parent().hasClass("a11yfy-top-level-menu")) {
                                    /* If in the menubar, then open the sub-menu */
                                    openMenu();
                                } else {
                                    /* If in sub-menu, move to previous element */
                                    prevInMenu();
                                }
                                break;
                            case 39: //right
                                handled = true;
                                if ($this.parent().hasClass("a11yfy-top-level-menu")) {
                                    /* If in menubar, move to next menuitem */
                                    nextInMenu();
                                } else {
                                    /* If in sub-menu, open sub-sub-menu */
                                    openMenu();
                                }
                                break;
                            case 40: //down
                                handled = true;
                                if ($this.parent().hasClass("a11yfy-top-level-menu")) {
                                    /* If in menubar, open sub-menu */
                                    openMenu();
                                } else {
                                    /* If in sub-menu, move to the next menuitem */
                                    nextInMenu();
                                }
                                break;
                        }
                        if (handled) {
                            e.preventDefault();
                            e.stopPropagation();
                        }
                        return true;
                    }).on("blur", function () {
                    }).on("click", function () {
                        var $this = jQuery(this);

                        if (!$this.hasClass("open")) {
                            $this.addClass("open");
                        } else {
                            $this.removeClass("open");
                        }
                    }).first().attr("tabindex", "0"); // Make the first menuitem in the menubar tab focussable
                    $this.on("keydown", function (e) {
                        /*
                         * This callback handles the tabbing out of the widget
                         */
                        var focusInTopMenu = false,
                            keyCode = e.which || e.keyCode;

                        if (e.ctrlKey || e.altKey || e.metaKey) {
                            // not interested
                            return;
                        }
                        if (keyCode !== 9) {
                            return true;
                        }
                        /* Find out whether we are currently in the menubar */
                        $this.find(">li").each(function (index, value) {
                            if (jQuery(value).attr("tabindex") === "0") {
                                focusInTopMenu = true;
                            }
                        });
                        if (!focusInTopMenu) {
                            /*
                             * If not in the menubar, close sub-menus and set the tabindex of the top item in the
                             * menubar so it receives focus when the user tabs back into the menubar
                             */
                            $this.find(">li li[tabindex=0]").attr("tabindex", "-1");
                            setTimeout(function () {
                                // This code is in a setTimeout so that shift tab works correctly AND
                                // because there is a Firefox (Windows) bug that
                                // causes the default event for a TAB to not happen properly if the visibility of the
                                // currently focussed node is chanhed mid event (e.g. removal of the open class)
                                $this.find("li.open").each(function (index, value) {
                                    if (jQuery(value).parent().hasClass("a11yfy-top-level-menu")) {
                                        jQuery(value).attr("tabindex", "0");
                                    }
                                }).removeClass("open");
                            }, 0);
                        }
                        return true;
                    });
                });
            }
        },
        ua = window.navigator.userAgent,
        platform = ua.match(/iPhone|iPad|iPod/) ? "iOS" :
                    ua.match(/Mac OS X/) ? "OSX" :
                    ua.match(/Windows/) ? "Windows" : "Other";

    jQuery.a11yfy = function () {
        $assertiveAnnouncer.empty();
    };

    jQuery.fn.a11yfy = function (method) {
        if (methods[method]) {
            return methods[method].apply(this, Array.prototype.slice.call(arguments, 1));
        } else {
            jQuery.error("Method " + method + " does not exist on jQuery.a11yfy");
        }
    };

    jQuery.fn.a11yfy.defaults = {
        strings: {
            skipToNextError: "skip to next field with an error"
        },
        validate: {
            skipLink: true,
            summary: true,
            validatorOptions: {}
        }
    };

    jQuery.a11yfy.getI18nString = function (str, values, strings) {
        var msg = strings[str], v;

        if (values) {
            for (v in values) {
                msg = msg.replace("${" + v + "}", values[v]);
            }
        }
        return msg;
    };

    jQuery.a11yfy.politeAnnounce = function (msg) {
        $politeAnnouncer.empty().append(jQuery("<p>").text(msg));
        
    };

    jQuery.a11yfy.assertiveAnnounce = function (msg) {
        $assertiveAnnouncer.empty().append(jQuery("<p>").text(msg));
        
    };

    // Add the polite announce div to the page
    if (!$politeAnnouncer || !$politeAnnouncer.length) {
        jQuery(document).ready(function () {
            $politeAnnouncer = jQuery("<div>").attr({
                "id": "jquery-a11yfy-politeannounce",
                "role": "log",
                "aria-live": "polite",
                "aria-relevant": "additions"
            }).addClass("offscreen");
            jQuery("body").append($politeAnnouncer);
        });
    }
    // Add the polite announce div to the page
    if (!$assertiveAnnouncer || !$assertiveAnnouncer.length) {
        jQuery(document).ready(function () {
            $assertiveAnnouncer = jQuery("<div>").attr({
                "id": "jquery-a11yfy-assertiveannounce",
                "role": "log",
                "aria-live": "assertive",
                "aria-relevant": "additions"
            }).addClass("offscreen");
            jQuery("body").append($assertiveAnnouncer);
           
        });
    }

})(jQuery);