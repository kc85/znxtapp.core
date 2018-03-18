/*!
 *  Double Carousel Plugin for JQuery
 *  Version   : 1.0
 *  Date      : 2014-01-02
 *  Licence   : All rights reserved 
 *  Author    : owwwlab (Ehsan Dalvand & Alireza Jahandideh)
 *  Contact   : owwwlab@gmail.com
 *  Web site  : http://themeforest.net/user/owwwlab
 *  Dependencies: tweenmx, imagesLoaded
 */

// Utility
if ( typeof Object.create !== 'function'  ){ // browser dose not support Object.create
    Object.create = function (obj){
        function F(){};
        F.prototype = obj;
        return new F();
    };
};

(function($, window, document, undefined) {
    
    var dcs,inAnimation=false;

    var DoubleCarousel = {
      init: function( options , elem ){
          var self = this; //store a reference to this

          self.elem = elem;
          self.$elem = $(elem);
          self.options = $.extend( {}, $.fn.DoubleCarousel.options, options);

          dcs=self.options;
          dcs.allItems=self.$elem.find('.item');
          
          dcs.leftItems=dcs.leftSide.find('.item');
          dcs.rightItems=dcs.rightSide.find('.item');


          self.prepare();
          //self.handleBgColors();

          self.bindUIActions();
        

      },
      prepare : function(){

        var self=this;

        //slides count
        dcs.slideCount = dcs.leftSide.find('.item').length; 
        
        dcs.currentSlideIndex = 0;
        
        dcs.counterTotal.html(dcs.slideCount);
        dcs.counterCurrent.html(1);

        //Show/hide Controllers
        dcs.nextButton.fadeIn();
        dcs.prevButton.fadeOut();


        dcs.rightDirectionSign=(dcs.rightSideDir=='down')?-1:1;
        dcs.leftDirectionSign=(dcs.leftSideDir=='down')?-1:1;

        self.update();

        var leftFillMode=dcs.leftSide.attr('data-fill'),
            rightFillMode=dcs.rightSide.attr('data-fill');
       
        if (leftFillMode){
          self.fillCore(leftFillMode,'left');
        }

        if (rightFillMode){
          self.fillCore(rightFillMode,'right');
        }
       
      },
      update:function(){

        var self=this;

        dcs.carouselHeight=self.$elem.height();

        var cHeight=dcs.carouselHeight;

        //Arrange slides based on sides direction
        dcs.rightItems.each(function(){
          var $this=$(this),
              index=$this.index();

          TweenMax.to($this,0,{y:dcs.rightDirectionSign*index*cHeight});

        });

         dcs.leftItems.each(function(){
          var $this=$(this),
              index=$this.index();

          TweenMax.to($this,0,{y:dcs.leftDirectionSign*index*cHeight});

        });

         (new TimelineLite())
          .to(dcs.rightWrapper,0,{y:-dcs.rightDirectionSign*dcs.currentSlideIndex*cHeight})
          .to(dcs.leftWrapper,0,{y:-dcs.leftDirectionSign*dcs.currentSlideIndex*cHeight});  
      },
      nextSlide : function(){
        if (dcs.currentSlideIndex>=dcs.slideCount-1 || inAnimation){
          return false
        }

        var self=this,
            cHeight=dcs.carouselHeight;

        inAnimation=true;
        dcs.currentSlideIndex++;

        if (dcs.currentSlideIndex+1==dcs.slideCount)
          dcs.nextButton.fadeOut();
        dcs.prevButton.fadeIn();
        
        self.changeSlide(dcs.currentSlideIndex);
      },

      prevSlide : function(){
        if (dcs.currentSlideIndex<=0 || inAnimation){
          return false
        }

        var self=this,
            cHeight=dcs.carouselHeight;

        inAnimation=true;
        dcs.currentSlideIndex--;
        if (dcs.currentSlideIndex==0){
          //hide the nexr arrow
          dcs.prevButton.fadeOut();
        }
        dcs.nextButton.fadeIn();
        self.changeSlide(dcs.currentSlideIndex);
      
      },
      gotoSlide:function(sIndex){
        var self=this,
            cHeight=dcs.carouselHeight;

        inAnimation=true;

        self.changeSlide(sIndex);
           
      },
      changeSlide:function(sIndex){
        var self=this,
            cHeight=dcs.carouselHeight;

        self.$elem.trigger('start-change');
        self.updateCounter(dcs.currentSlideIndex);
        (new TimelineLite({onComplete:function(){
          inAnimation=false;
          self.$elem.trigger('end-change');
        }
        }))
          .to(dcs.rightWrapper,1,{y:-dcs.rightDirectionSign*sIndex*cHeight,ease:Power4.easeOut})
          .to(dcs.leftWrapper,1,{y:-dcs.leftDirectionSign*sIndex*cHeight,ease:Power4.easeOut},'-=1');  
      },
      updateCounter : function(currentSlideIndex){
        dcs.counterCurrent.html(currentSlideIndex+1);
      },
      bindUIActions: function(){
        self = this;

        dcs.nextButton.on('click',function(){
          self.nextSlide();
        })
        dcs.prevButton.on('click',function(){
          self.prevSlide();
        })

        $(window).on('resize',function(){
          self.update();
        });

        if (dcs.mouse){
           self.scrollControll();
        }

        if (dcs.keyboard){
           self.keyboardControll();
        }

        if (dcs.touchSwipe){
          self.touchControll();
        }
        if (dcs.bulletControll){
           self.bulletControll();
        }
      
      },
      scrollControll:function(){
       

        self.$elem.on('DOMMouseScroll mousewheel', function (e) { 

          if(e.originalEvent.detail > 0 || e.originalEvent.wheelDelta < 0) {
              self.nextSlide();

          } else {
              self.prevSlide();
            
          }

        });
      },
      keyboardControll:function(){
        $(document).keydown(function(e) {
          switch(e.which) {
              case 38: // up
                self.prevSlide();
              break;

              case 40: // down
              self.nextSlide();
              break;

              default: return; 
          }
          e.preventDefault(); 
      });
      },
      touchControll:function(){
        self.$elem.swipe({
          
          swipe:function(event, direction, distance) {
            if (direction=='down'){
              self.nextSlide();
            }else if (direction=='up'){
              self.prevSlide();
            }
          }

        });
      },
      bulletControll:function(){
        var self=this;
       
        var $bWrapper=$('<ul></ul>').addClass('vc-bullets');

        if (dcs.bulletNumber){
          $bWrapper.addClass('bullet-numbers');
        }

        var lis='',j,liContent;

        for (var i = 0; i <= dcs.slideCount - 1; i++) {
          
          j=(i<10)?('0'+(i+1)):(i+1);

          liContent=(dcs.bulletNumber==true)?'<span>'+j+'</span>':'';

          lis+='<li><span>'+liContent+'</span></li>';  
        };


        $bWrapper.append(lis).appendTo(self.$elem);

        dcs.bullets=$bWrapper.children();


        //Set the position of bullets
        if (dcs.bulletCenter=='vertical'){
          $bWrapper.css('margin-top',-($bWrapper.height()/2));
        }else if (dcs.bulletCenter=='horizontal'){
          $bWrapper.css('margin-left',-($bWrapper.width()/2));
        }

        dcs.bullets.eq(dcs.currentSlideIndex).addClass('active');

        dcs.bullets.on('click',function(){
          var $this=$(this),
              index=$(this).index();

          if (index==dcs.currentSlideIndex){
            return false
          }

          dcs.currentSlideIndex=index;

          self.gotoSlide(index);

        });
        self.$elem.on('start-change',function(){
          dcs.bullets.removeClass('active');
          dcs.bullets.eq(dcs.currentSlideIndex).addClass('active');
        });

      },
      //cover images in a container
      fillCore:function(mode,side){
          var self=this;

          var items=(side=='left')?dcs.leftItems:dcs.rightItems;

          var state=(mode=='side-fill')?'manual':'auto';

          runFill();

          function runFill(){
            items.each(function(){
              fillIt($(this),state,self.$elem.width(),mode,side);
            });
          }

          function fillIt($container,state,cWidth,mode,side){

            $container.imagesLoaded(function(){
              
                var containerWidth=(state=='manual')?cWidth:$container.width(),
                    containerHeight=$container.height(),
                    containerRatio=containerWidth/containerHeight,
                    imgRatio;

                $container.find('img').each(function(){
                  var img=$(this);
                    imgRatio=img.width()/img.height();

                  if (containerRatio < imgRatio) {
                    // taller
                    img.css({
                      width: 'auto',
                      height: containerHeight,
                      marginTop:0,
                      marginLeft:-(containerHeight*imgRatio-containerWidth)/2
                    });
                  } else {
                    // wider
                    img.css({
                        width: containerWidth,
                        height: 'auto',
                        marginTop:-(containerWidth/imgRatio-containerHeight)/2,
                        marginLeft:0
                      });
                    }

                    if (mode=='side-fill'){
                      img.addClass('fill-'+side);
                    }
                    

                });
            });
          }

          $(window).on('resize',function(){
            runFill();
          });
        
      }
    }

    
    $.fn.DoubleCarousel = function( options ) {
        return this.each(function(){
            var dCar = Object.create( DoubleCarousel ); 
            dCar.init( options, this );
        }); 
    };

    $.fn.DoubleCarousel.options = {
      rightSide       : $('.right-side'), 
      leftSide        : $('.left-side'),
      leftWrapper     : $('.left-side-wrapper'),
      rightWrapper    : $('.right-side-wrapper'),
      nextButton      : $('.vcarousel-next'),
      prevButton      : $('.vcarousel-prev'),
      counterTotal    : $(".vcarousel-counter .counter-total"),
      counterCurrent  : $(".vcarousel-counter .counter-current"),
      rightSideDir    :'up',
      leftSideDir     :'down',
      leftSideDuration : 1,
      rightSideDuration :1,
      mouse :true,
      keyboard   :true,
      touchSwipe :true,  
      bulletControll:true,
      bulletNumber:false,
      bulletCenter:'vertical'//vertical, horizontal or none
    };



})(jQuery, window, document);