/*!
 *  Video background plugin by owwwlab
 *  Version   : 0.9
 *  Date      : 2014-06-05
 *  Licence   : All rights reserved 
 *  Author    : owwwlab (Ehsan Dalvand & Alireza Jahandideh)
 *  Contact   : owwwlab@gmail.com
 *  Web site  : http://themeforest.net/user/owwwlab
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
    
    var vs;

    var owlVideoBg = {
      init: function( options , elem ){
          var self = this; //store a reference to this

          self.elem = elem;
          self.$elem = $(elem);
          self.options = $.extend( true,{}, $.fn.owlVideoBg.options, options);

          vs=self.options;
          self.$elem.addClass(vs.elemClass);

          generatedMarkup=self.generateMarkup();

          if (generatedMarkup){
            self.inject(generatedMarkup.videoHTML);
          }else{
            self.setPoster();
          }  

         

      },
      generateMarkup : function(){
        var self=this,
            srcPath,basePath,ogvPath,webmPath,posterPath,videoHTML,videoHeadTag;

        vs.autoplay = self.$elem.hasClass('autoplay')||vs.autoplay,
        vs.muted = self.$elem.hasClass('muted') || vs.muted,
        vs.loop =  self.$elem.hasClass('loop') || vs.loop,
        vs.playonHover =self.$elem.hasClass('hoverPlay') || vs.playonHover;

        var headOption='';

        if (vs.autoplay & !vs.playonHover){
          headOption+=' autoplay';
        }
        if (vs.muted){
          headOption+=' muted';
        }

        if(vs.loop){
          headOption+=' loop';
        }

        if (vs.preload && !vs.autoplay){
           headOption += ' preload="'+vs.preload+'"';
        }
       

        videoHeadTag='';


        if (vs.autoGenerate.basedOnSrc){
          //We should generate other video formats and poster image path

          //Do the user provide inline src ?
          var inlineSrc=(vs.autoGenerate.inlineBaseSrc!='');

          if (inlineSrc){
             srcPath=vs.autoGenerate.inlineBaseSrc;
          }else{
             // read the base source path from HTML markup
             srcPath=self.$elem.attr(vs.autoGenerate.srcProperty);
          }


          if (srcPath==undefined || ''){
            return false;
          }

          
          basePath=srcPath.substr(0, srcPath.lastIndexOf('.mp4')) || srcPath;
          
          //Generate the video element

          videoHTML=videoHeadTag+'<source src="'+srcPath+'" type="video/mp4">';


          //Generate all formats file path
          vs.autoGenerate.formats.forEach(function(entry) {
               videoHTML+='<source src="'+basePath+'.'+entry.fileFormat+'" type="'+entry.mime+'">';
          });

          //Add other if you need to
          videoHTML+='</video>';


          //Generate Poster image path
          posterPath=basePath+'.'+vs.autoGenerate.posterImageFormat;
          posterPath=self.$elem.attr(vs.autoGenerate.posterProperty)||posterPath;



        }else{
          //We should use provided source by user and don't generate any

          if($.isEmptyObject(vs.srcSetup)){
             return false
          }
          else{
            
            //Generate the video element
            videoHTML=videoHeadTag;
            vs.srcSetup.forEach(function(entry) {
                 videoHTML+='<source src="'+entry.path+'" type="'+entry.mime+'">';
            });

            //Add other if you need to
            videoHTML+='</video>';

            }

            posterPath=vs.posterPath;
        }

        headOption+=' poster="'+posterPath+'"';

        videoHeadTag='<video '+headOption+'>';

        videoHTML=videoHeadTag+videoHTML;

        //update poster path
        vs.posterPath=posterPath;

        return {
            videoHTML:videoHTML,
            posterPath:posterPath
          }
        
      },
      inject:function(videoHTML){
        var self=this;

        self.$video=$(videoHTML);

        self.$videoWrapper=$('<div class="'+vs.wrapperClass+'"></div>').wrapInner(self.$video);
        

        //Do we want to inject the video !? yes we should use only poster images in touch devices because they won't let us autoplay video
        if(self.isTouchSupported()){
          //Sorry we should use poster image as element background
          
          self.setPoster();
        }
        else{
          //Yes inject it
          
          self.fillIt();//Fit the size and center video
          self.$elem.append(self.$videoWrapper);//Finally append it to the DOM 
          self.bindUIActions();
        }

      },
      //cover video in container
      fillIt:function(){
          var self=this,
            responsiveFlag = false;
          function fillCore(){

            var containerWidth=self.$elem.outerWidth(),
              containerHeight=self.$elem.outerHeight(),
              containerRatio=containerWidth/containerHeight;
              

            var videoRatio=vs.videoRatio;

            //check if the user has given a width to the container el
            if (containerHeight==0 || responsiveFlag){
              containerHeight = containerWidth/vs.videoRatio;
              containerRatio = vs.videoRatio;
              responsiveFlag = true;
              self.$elem.css('height',containerHeight);
            }

            if (containerRatio < videoRatio) {
              // taller
              self.$video.css({
                width: 'auto',
                height: containerHeight,
                marginTop:0,
                marginLeft:-(containerHeight*videoRatio-containerWidth)/2
              });
            } else {
              // wider
              self.$video.css({
                width: containerWidth,
                height: 'auto',
                marginTop:-(containerWidth/videoRatio-containerHeight)/2,
                marginLeft:0
              });
            }  
          }

          fillCore();

          $(window).on('debouncedresize',function(){
              fillCore();
          });
      },
      isTouchSupported:function(){
        //check if device supports touch
        var msTouchEnabled = window.navigator.msMaxTouchPoints;
        var generalTouchEnabled = "ontouchstart" in document.createElement("div");
     
        if (msTouchEnabled || generalTouchEnabled) {
            return true;
        }
        return false;

      },
      setPoster:function(){
        var self=this;

        if (vs.posterPath==''){
          return false;
        }

        self.$elem.css({
          'background-image' :'url('+vs.posterPath+')',
          'background-size' :'cover',
          'background-position': '50% 0'
        })
      },
      bindUIActions: function(){/**/
        var self = this;

          if (vs.playonHover){
            self.$elem.on('mouseenter',function(){
              self.$video.get(0).play();
            });
            self.$elem.parent().on('mouseenter',function(){
              self.$video.get(0).play();
            });

            self.$elem.on('mouseleave',function(){
              self.$video.get(0).pause();
            });
            self.$elem.parent().on('mouseleave',function(){
              self.$video.get(0).pause();
            });
          }
      }
    }

    
    $.fn.owlVideoBg = function( options ) {
        return this.each(function(){
            var obj = Object.create( owlVideoBg ); 
            obj.init( options, this );
        }); 
    };

    $.fn.owlVideoBg.options = {
 
      elemClass:'owl-videobg',// This class will be added to element
      wrapperClass:'owl-video-wrapper', // Video tag will be wrapped inside a div with this class
      autoGenerate:{
        basedOnSrc:true,
        formats:[
          {
            fileFormat:'webm',
            mime :'video/webm'
          },
          {
            fileFormat:'ogv',
            mime :'video/ogg'
          }
        ],
        posterImageFormat :'jpg',
        srcProperty: 'data-src', // For reading from HTMl markup
        posterProperty :'data-poster',
        inlineBaseSrc : '' ,//--- This should be the source to mp4 file, we will use this to generate other formats and poster image 
      },
      srcSetup: [ // Object containing path and mime type of formats you want to use and also poster image path
        /*{
          path:'video/susanna/susanna.mp4',//path to file 
          mime :'video/mp4'  //type of it for example video/mp4
        },
        {
          path:'video/susanna/susanna.webm',//path to file 
          mime :'video/webm'  //type of it for example video/mp4
        }*/
        //Add as much as you have
      ],
      posterPath:'' ,// if autoGenerate is off you should set this to the path of poster image file
      autoplay:false, // If you set playonHover to true this will be automatically set to false
      muted :true,
      loop :true,
      preload:'none', // none, auto,metadata or false to ignore it
      playonHover: false,
      videoRatio :1.778 // 16/9
      
    };

})(jQuery, window, document);