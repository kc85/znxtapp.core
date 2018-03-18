
/*
# AccelHoverScroll v0.1
# Accelerated Hoverscroll: mouse over the left or right sides
# of a container and the containerw will scroll in that direction,
# the closer to the edge of the container the faster it goes.
#
# Do as thou wilt with this code but send any surplus love
# over to usefulrobot.io
#
# Copyright 2012 Alex Matchneer / Useful Robot
*/


(function() {
  var $,
    __bind = function(fn, me){ return function(){ return fn.apply(me, arguments); }; };

  $ = jQuery;

  $.AccelHoverScroll = (function() {

    function AccelHoverScroll(element, options) {
      var position;
      if (options == null) {
        options = {};
      }
      this._onMouseMove = __bind(this._onMouseMove, this);

      this._reset = __bind(this._reset, this);

      this._onMouseLeave = __bind(this._onMouseLeave, this);

      this._onMouseEnter = __bind(this._onMouseEnter, this);

      this._onResize = __bind(this._onResize, this);

      this.$outerContainer = $(element);
      this.options = {
        maxScrollSpeed: 800,
        scrollGutterPercentage: 0.3,
        scrollGutterSlices: 5,
        overrideCursor: true
      };
      $.extend(this.options, options);
      this.defaultCursor = this.$outerContainer.css('cursor');
      this.$contentContainer = this.$outerContainer.find('.hoverscroll-content');
      if (!this.$contentContainer.length) {
        console.log("accelHoverScroll: Couldn't find required .hoverscroll-content class");
        return;
      }
      position = this.$contentContainer.css('position');
      if (position !== 'relative' && position !== 'absolute') {
        position = 'relative';
      }
      this.$contentContainer.css({
        position: position,
        left: "0px"
      });
      this.$outerContainer.css({
        overflow: 'hidden'
      });
      this.currentScrollSpeed = 0;
      this.$leftArrow = $('.hoverscroll-left');
      this.$rightArrow = $('.hoverscroll-right');
      this.$leftArrow.hide();
      this.$outerContainer.hover(this._onMouseEnter, this._onMouseLeave);
      return;
      if ($.event.special.smartresize) {
        $(window).smartresize(this._onResize);
      } else {
        $(window).resize(this._onResize);
      }
    }

    AccelHoverScroll.prototype._onResize = function() {
      return console.log("Resizeed!");
    };

    AccelHoverScroll.prototype.pause = function() {
      this._onMouseLeave();
      return this.isPaused = true;
    };

    AccelHoverScroll.prototype.resume = function() {
      return this.isPaused = false;
    };

    AccelHoverScroll.prototype.scrollZero = function() {
      this._reset();
      return this.$contentContainer.css('left', "0px");
    };

    AccelHoverScroll.prototype._onMouseEnter = function(e) {
      var gutterWidth;
      if (this.isPaused) {
        return;
      }
      this.currentScrollSpeed = 0;
      this.contentContainerWidth = this.$contentContainer.outerWidth(true);
      this.outerWidth = this.$outerContainer.outerWidth(true);
      gutterWidth = this.outerWidth * this.options.scrollGutterPercentage;
      this.currentSliceSize = Math.ceil(gutterWidth / this.options.scrollGutterSlices);
      return this.$outerContainer.mousemove(this._onMouseMove);
    };

    AccelHoverScroll.prototype._onMouseLeave = function(e) {
      if (this.isPaused) {
        return;
      }
      this._reset();
      return this.$contentContainer.unbind('mousemove');
    };

    AccelHoverScroll.prototype._reset = function(jumpToEnd) {
      if (jumpToEnd == null) {
        jumpToEnd = false;
      }
      this.currentScrollSpeed = 0;
      this.$contentContainer.stop(true, jumpToEnd);
      if (this.options.overrideCursor) {
        return this.$outerContainer.css('cursor', this.defaultCursor);
      }
    };

    AccelHoverScroll.prototype._onMouseMove = function(e) {
      var accelPerc, contentContainerLeft, distanceToGo, duration, gutterPerc, left, maxScroll, perc, s, sliceIndex, sliceSize, targetScrollSpeed,
        _this = this;
      if (this.isPaused) {
        return;
      }
      gutterPerc = this.options.scrollGutterPercentage;
      perc = e.clientX / this.outerWidth;
      targetScrollSpeed = 0;
      if (perc < gutterPerc) {
        left = true;
        accelPerc = 1 - (perc / gutterPerc);
        targetScrollSpeed = accelPerc * this.options.maxScrollSpeed;
      } else if (perc > (1 - gutterPerc)) {
        left = false;
        accelPerc = (perc - 1 + gutterPerc) / gutterPerc;
        targetScrollSpeed = accelPerc * this.options.maxScrollSpeed;
      } else {
        this._reset();
        return;
      }
      sliceSize = 1.0 / this.options.scrollGutterSlices;
      sliceIndex = Math.floor(accelPerc / sliceSize);
      if (sliceIndex === this.oldSliceIndex) {
        return;
      }
      this.oldSliceIndex = sliceIndex;
      targetScrollSpeed = Math.floor(targetScrollSpeed / 10) * 10;
      targetScrollSpeed = Math.max(10, targetScrollSpeed);
      if (left) {
        targetScrollSpeed = -targetScrollSpeed;
      }
      if (targetScrollSpeed === this.currentScrollSpeed) {
        return;
      }
      this.currentScrollSpeed = targetScrollSpeed;
      contentContainerLeft = parseInt(this.$contentContainer.css('left'));
      maxScroll = this.contentContainerWidth - this.outerWidth;
      distanceToGo = left ? Math.abs(contentContainerLeft) : maxScroll + contentContainerLeft;
      if (distanceToGo < 0) {
        this._reset(true);
        return;
      }
      duration = Math.floor(distanceToGo / targetScrollSpeed * 1000);
      duration = Math.abs(duration);
      if (!duration) {
        return;
      }
      this.$leftArrow.fadeIn();
      this.$rightArrow.fadeIn();
      if (this.options.overrideCursor) {
        if (left) {
          this.$outerContainer.css('cursor', 'w-resize');
        } else {
          this.$outerContainer.css('cursor', 'e-resize');
        }
      }
      s = left ? "0px" : "-" + (Math.abs(maxScroll)) + "px";
      return this.$contentContainer.stop(true, false).animate({
        left: s
      }, duration, 'linear', function() {
        if (left) {
          _this.$leftArrow.fadeOut();
        } else {
          _this.$rightArrow.fadeOut();
        }
        return _this._reset();
      });
    };

    return AccelHoverScroll;

  })();

  $.fn.accelHoverScroll = function(options) {
    var args;
    if (options == null) {
      options = {};
    }
    if (typeof options === 'string') {
      args = Array.prototype.slice.call(arguments, 1);
      this.each(function() {
        var instance;
        instance = $.data(this, 'hoverscroll');
        return instance[options].apply(instance, args);
      });
    } else {
      this.each(function() {
        return $.data(this, 'hoverscroll', new $.AccelHoverScroll(this, options));
      });
    }
    return this;
  };

}).call(this);
