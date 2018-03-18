$(document).ready(function(){

	"use strict";

	//Template direction RTL flag// Set it to true for RTL direction.
	var rtlDirectionFlag=false;

	$.ajaxSetup({ cache: false });

	$('.kb-slider').kenburnIt();

	$('.vertical-carousel').DoubleCarousel();


	//back to top
	$('body').on('click','.back-to-top',function(){
		TweenMax.to($("html, body"),0.5,{ scrollTop: 0, ease:Power2.easeOut });
        return false;
	});

	// modify Isotope's absolute position method for RTL
	if (rtlDirectionFlag){
		$.Isotope.prototype._positionAbs = function( x, y ) {
		  return { right: x, top: y };
		};
	}
		


	//Parallax contents
	var parallaxElems={

		init:function(){

			this.scrollElems=$('.tj-parallax'),
			this.scrollWrappers=$(window);
			this.parallaxIt();
		},
		parallaxIt:function(){
			//this.scrollElems.each(function(){
				var $self=this.scrollElems;
				var parentHeight=this.scrollElems.parent('.parallax-parent').height(),
					elemBottom=parseInt(this.scrollElems.css('bottom')),
					elemOpacity=parseInt(this.scrollElems.css('opacity')),
					scrollAmount,animObj,posValue,opacityValue;

				this.scrollWrappers.scroll(function(){
					scrollAmount=$(this).scrollTop();

					posValue=-(elemBottom/parentHeight)*scrollAmount +elemBottom;
					opacityValue=-(elemOpacity/parentHeight)*scrollAmount +elemOpacity;

					animObj={'bottom':posValue,'opacity':opacityValue};

					TweenMax.to($self,0.2,animObj);
				});	

			//});
		}
	}

	
	/* Sidebar and menu animation
	----------------------------------------------*/
	var sideS, sidebar = {
		settings : {
			$sidebar : $('div#side-bar'),
			$sideContents :$('#side-contents'),
			$menuToggle:$('#menu-toggle-wrapper'),
			$sideFooter : $("div#side-footer"),
			$innerBar :$('#inner-bar'),
			$main : $("div#main-content, .page-side"),
			$navigation:$('#navigation'),
			$exteras : $('.move-with-js'),
			sideFlag:false,
			menuFlag:false,
			mobileFlag:false,
			showSide:$('body').hasClass('show-sidebar')
		},

		init : function(){
			sideS = this.settings;
			this.prepare();
			this.bindUiActions();

			if (!isTouchSupported()){
				sideS.$sidebar.find('.inner-wrapper').niceScroll({
					horizrailenabled:false
				});
			} 

			this.setMobileFlag();
		},

		bindUiActions : function(){
			
			var self = this;
			var toggleFlag=0;
			$('#menu-toggle-wrapper , #inner-bar').on('click', function(e){
				e.preventDefault();
				if (toggleFlag){
					self.toggleMenu('out');
					toggleFlag=0;
				}else{
					self.toggleMenu('in');
					toggleFlag=1;
				}
			});

			$(window).on('debouncedresize',function(){
				self.setMobileFlag();
				self.prepare();
				if(sideS.sideFlag && !sideS.mobileFlag){
					sideS.$menuToggle.trigger('click');
				}
			});

			//sub-menu handler
			sideS.$sideContents.find('li a').on('click',function(e){
				
				if (!sideS.menuFlag){
					sideS.menuFlag=true;
					var $this = $(this),
						$li = $this.parent('li'),
						$childUl = $this.siblings('ul');


					if ($childUl.length==1){
						e.preventDefault();
						$childUl.css('display','block');
						if (!rtlDirectionFlag){
							TweenMax.to($childUl,0.7,{left:0,ease:Power4.easeOut});	
						}else{
							TweenMax.to($childUl,0.7,{right:0,ease:Power4.easeOut});
						}
						
						$childUl.addClass('menu-in');
					}
				}
				
			});

			sideS.$sideContents.find('li.nav-prev').on('click',function(){
				var $subMenus=sideS.$sideContents.find('.sub-menu');
				if (!rtlDirectionFlag){
					TweenMax.to($subMenus,0.7,{left:'-100%',ease:Power4.easeOut,onComplete:function(){
						$subMenus.css('display','none');
						sideS.menuFlag=false;

					}});
				}else{
					TweenMax.to($subMenus,0.7,{right:'-100%',ease:Power4.easeOut,onComplete:function(){
						$subMenus.css('display','none');
						sideS.menuFlag=false;
					}});
				}

				$subMenus.removeClass('menu-in');

			});
		},

		toggleMenu : function(dir){
			
			var self = this,
				sideWidth = sideS.$sidebar.outerWidth(),
				compactWidth = sideS.$innerBar.outerWidth(),
				diff = sideWidth-compactWidth,
				timing = 0.4;

			if($(window).width()<992){
				diff = sideWidth;
			}

			dir || console.log('message: input argument missing');
			

			var animOut = new TimelineLite({paused:true});
			var animIn = new TimelineLite({paused:true});

			if (!rtlDirectionFlag){
				animOut
				.to(sideS.$innerBar,timing,{left:0,ease:Power4.easeOut,onStart:function(){
					sideS.$menuToggle.removeClass('anim-out');
					sideS.$sidebar.css('z-index',0);
					sideS.$main.css('display','block');
				},onComplete:function(){
					sideS.$sidebar.css('display','none');
				}},'start')
				.to(sideS.$exteras,timing,{marginRight:0,ease:Power4.easeOut},'start');
				
				if (!sideS.mobileFlag){
					animOut.to(sideS.$main,timing,{left:0,right:0,ease:Power4.ease},'start');
				}
				
				animIn
				.to(sideS.$innerBar,timing,{left:-compactWidth,ease:Power4.easeOut,onStart:function(){
					sideS.$menuToggle.addClass('anim-out');
					sideS.$sidebar.css('display','block');
				},onComplete:function(){
					sideS.$sidebar.css('z-index',10);
					if (sideS.mobileFlag){
						sideS.$main.css('display','none');
					}
					

				}},'start')
				.to(sideS.$exteras,timing,{marginRight:-diff,ease:Power4.easeOut},'start');

				if (!sideS.mobileFlag){
					animIn.to(sideS.$main,timing,{left:diff,right:-diff,ease:Power4.ease},'start');
				}
				
				
					
			}else{
				animOut
				.to(sideS.$innerBar,timing,{right:0,ease:Power4.easeOut,onStart:function(){
					sideS.$menuToggle.removeClass('anim-out');
					sideS.$sidebar.css('z-index',0);
					sideS.$main.css('display','block');
				},onComplete:function(){
					sideS.$sidebar.css('display','none');
				}},'start')
				.to(sideS.$exteras,timing,{marginReft:0,ease:Power4.easeOut},'start');

				if (!sideS.mobileFlag){
					animOut.to(sideS.$main,timing,{right:0,left:0,ease:Power4.ease},'start');
				}

				animIn
				.to(sideS.$innerBar,timing,{right:-compactWidth,ease:Power4.easeOut,onStart:function(){
					sideS.$menuToggle.addClass('anim-out');
					sideS.$sidebar.css('display','block');
				},onComplete:function(){
					sideS.$sidebar.css('z-index',10);
					if (sideS.mobileFlag){
						sideS.$main.css('display','none');
					}
				}},'start')
				.to(sideS.$exteras,timing,{marginRight:-diff,ease:Power4.easeOut},'start');

				if (!sideS.mobileFlag){
					console.log(sideS.mobileFlag);
					animIn.to(sideS.$main,timing,{right:diff,left:-diff,ease:Power4.ease},'start');
				}

			}

			if (dir=='out'){
				animOut.play();
				sideS.sideFlag=false;
			}else{
				animIn.play();
				sideS.sideFlag=true;
			}
		},
		prepare:function(){
			var self=this;

			var madineHeight=$(window).height()-$('#logo-wrapper').outerHeight();

			self.madineHeight=madineHeight;
			self.navHeight=sideS.$navigation.height();

			$('.sub-menu').css('height',madineHeight);

		},
		setMobileFlag:function(){
			var self=this;

			var tWidth=$(window).width();

			if (tWidth<600){
				sideS.$sidebar.width(tWidth);
				sideS.mobileFlag=true;

			}else{
				sideS.$sidebar.width('');
				sideS.mobileFlag=false;
				if (sideS.showSide){
					sideS.$sidebar.css('display','');
				}
			}
		
		}

	}




	/* cover images in a container
	----------------------------------------------*/
	var imageFill={
			
	  init:function($container,callback){
	    this.container=$container;
	    this.setCss(callback);
	    this.bindUIActions();

	  },
	  setCss:function(callback){
	    $container=this.container;
	    $container.imagesLoaded(function(){
	      var containerWidth=$container.width(),
	        containerHeight=$container.height(),
	        containerRatio=containerWidth/containerHeight,
	        imgRatio;

	      $container.find('img').each(function(){
	        var img=$(this);
	        imgRatio=img.width()/img.height();
	        
	        if (img.css('position')=='static'){
	        	img.css('position','relative');
	        }
	        if (containerRatio < imgRatio) {
	          // taller
	          img.css({
	              width: 'auto',
	              height: containerHeight,
	              top:0,
	              left:-(containerHeight*imgRatio-containerWidth)/2
	            });
	        } else {
	          // wider
	          img.css({
	              width: containerWidth,
	              height: 'auto',
	              top:-(containerWidth/imgRatio-containerHeight)/2,
	              left:0
	            });
	        }
	      })
	      if (typeof(callback) == 'function'){
	      	callback();
	      }
	    });
	  },
	  bindUIActions:function(){
	    var self=this;
	    $(window).on('debouncedresize',function(){
	        self.setCss();
	    });
	  }
	}


	


	/* google map
	----------------------------------------------*/
	var $gmap = $("#gmap , .gmap"); 
	if ($gmap.length > 0){
		$gmap.each(function(){
			var $gmap=$(this);
			var address= $gmap.attr('data-address') || 'Footscray VIC 3011 Australia';

			$gmap.gmap3({
			  map: {
			      options: {
			          maxZoom:15,
			          streetViewControl: false,
			          mapTypeControl: false,
			      }
			  },
			  styledmaptype: {
			      id: "mystyle",
			      options: {
			          name: "Style 1"
			      },
			      styles: [
			          {
			              featureType: "all",
			              stylers: [
			                  {"saturation": -100}, {"gamma": 0.9}
			              ]
			          }
			      ]
			  },
			  overlay:{
			    //Edit following line and enter your own address
			    address: address,
			    options:{
			      content: '<div id="map-marker"><i class="fa fa-map-marker"></i></div>',
			      offset:{
			        y:-65,
			        x:-20
			      }
			    }
			    },//Following maxZoom option is for setting the initial zoom of google map
				  autofit:{maxZoom: 15}
				},"autofit");

			$gmap.gmap3('get').setMapTypeId("mystyle");
		});
		
	
	}
	



	/* portfolios grid
	----------------------------------------------*/
	var portfolios={
		init:function(){
			this.hfolio=$('.horizontal-folio'),
			this.hfolioContainer=$('.horizontal-folio-wrapper'),
			this.gfolio=$('.grid-portfolio'),
			this.gfolioFilters=$('.grid-filters');
			this.transform=(rtlDirectionFlag)?false:true;

			
			this.run();
			this.bindUIActions();
		},
		run:function(){
			var self=this;
			self.sameRatioFlag=(self.gfolio.hasClass('same-ratio-items'))?true:false;

			self.gridSizer=self.gfolio.find('.grid-sizer').first();
			if (self.gridSizer.length<1){
				self.gridSizer=self.gfolio.find('.gp-item').first();
			}

			self.gfolioIsotop();

			var defaultFilter=self.gfolioFilters.find('.active');
			var defaultFilterVal=defaultFilter.children().first().attr('data-filter');


			if (defaultFilterVal!='*'){
				self.gfolio.isotope({ filter: defaultFilterVal });
			}

			self.hfolio.isotope({
				layoutMode: 'masonryHorizontal',
				transformsEnabled: self.transform
			});

			var folioScroll=this.hfolioContainer.niceScroll({
				cursoropacitymin :1,
				cursoropacitymax :1,
				rtlmode:rtlDirectionFlag,
				railalign:(rtlDirectionFlag)?'left':'right'
			});	
			
			
		},
		bindUIActions:function(){
			var self=this;

			$(window).on('debouncedresize',function(){
				self.gfolioIsotop();
			});

			 // click event in isotope filters
		    self.gfolioFilters.on('click', 'a',function(e){
		      e.preventDefault();
		      $(this).parent('li').addClass('active').siblings().removeClass('active');
		      var selector = $(this).attr('data-filter');
		      self.gfolio.isotope({ filter: selector });
		    });

		    //Filters toggle handler added since v1.5//

		    var filter=$('.fixed-filter , .grid-filters-wrapper');

		    filter.mouseenter(function(){
		    	filter.addClass('active');
		    }).mouseleave(function(){
		    	filter.removeClass('active');
		    })
		    $('.select-filter').on('touchend',function(){
		    	if (filter.hasClass('active')){
		    		filter.removeClass('active');
		    	}else{
		    		filter.addClass('active');
		    	}
			})
			//End of Filters toggle handler added since v1.5//
			    
		},
		//calculate column width for Mr. isotope
		colWidth : function ($container,itemSelector) {
			var self =this;
			var w = $container.width(), 
				columnNum = 1,
				columnWidth = 0;

		
			if (w > 1200) {
				columnNum  = $container.attr('lg-cols')|| 4;
			}else if(w >= 800){
				columnNum  = $container.attr('md-cols')|| 3;
			}else if (w >= 500) {
				columnNum  = $container.attr('sm-cols')|| 2;
			} else if (w >= 300) {
				columnNum  = $container.attr('xs-cols')|| 1;
			}
			columnWidth = Math.floor(w/columnNum);
			

			self.gridSizer.width(columnWidth);
			var gridSizerHeight=self.gridSizer.height();

			$container.find(itemSelector).each(function() {
				var $item = $(this),
					itemWidth,widthRatio;

				widthRatio=parseFloat($item.attr('data-width-ratio')||1);

				var itemWidth=Math.min( columnWidth*widthRatio , columnWidth*columnNum );

				$item.width(itemWidth);	
				
			    //Do we need to keep exact size based on grid sizer ratio?
				if (self.sameRatioFlag && !$item.hasClass('save-the-height')){
					
					var itemHeight=Math.round($item.height()/gridSizerHeight)*gridSizerHeight;
					$item.height(itemHeight);
				
					$item.find('img').css('height',itemHeight);
				}
		
			});

			return columnWidth;
		},
		prepareItems:function(){
			this.gfolio.find('.gp-item').each(function(){
				var $this=$(this);
				$this.css({
					width:'auto',
					height:'auto'
				});
				$this.find('img').css('height','auto');
				var ratio=$this.height()/$this.width();

				$this.data('ratio',ratio);
			})
		},
		gfolioIsotop:function(){
			var self=this;
			self.gfolio.imagesLoaded(function(){
				self.prepareItems();
				var colw=self.colWidth(self.gfolio,'.gp-item');
				self.gfolio.isotope({
					resizable:false,
					itemSelector: '.gp-item',
					transformsEnabled: self.transform,
					masonry: {
						columnWidth: colw,
						gutterWidth: 0
					}

				});
			});
			
		}
	}


	/* blog grid
	----------------------------------------------*/
	var blogrid={
		init:function(){
			this.gblog=$('.grid-blog-list'),
			this.gblogFilters=$('.grid-filters');
			this.transform=(rtlDirectionFlag)?false:true;
			
			this.run();
			this.bindUIActions();
		},
		run:function(){
			
			this.gblogIsotop();
		},
		
		bindUIActions:function(){
			var self=this;

			$(window).on('debouncedresize',function(){
				self.gblogIsotop();
			});

		},
		//calculate column width for Mr. isotope
		colWidth : function ($container,itemSelector) {
			var w = $container.width(), 
				columnNum = 1,
				columnWidth = 0;

			if (w > 900) {
				columnNum  = 2;
			}
			columnWidth = Math.floor(w/columnNum);
			
			$container.find(itemSelector).each(function() {
				var $item = $(this);
				$item.css({
					width: columnWidth
				});
			});

			var $pageContainer=$container.parent();

			if(columnNum==1){
				$pageContainer.addClass('no-vertical-line');
			}else{
				$pageContainer.addClass('no-vertical-line');
			}

			return columnWidth;
		},
		gblogIsotop:function(){
			var self=this;

			self.gblog.isotope({
				resizable:false,
				transformsEnabled: self.transform,
				itemSelector: '.grid-blog-list .post',
				masonry: {
					columnWidth: this.colWidth(this.gblog,'.post'),
					gutterWidth: 0
				}

			});
		}
	}

	var ajaxFolio={

		init:function(){
			var self=this;

			self.itemsClass='.ajax-portfolio, .portfolio-prev, .portfolio-next';
			self.itemContainer=$('#ajax-folio-item');
			self.loader=$('#ajax-folio-loader'); //loader element
			self.contentSelector = '#main-content'; //selector for the content we want to add to page
			self.contents=$(self.contentSelector).first();
			self.ajaxElements=$('.ajax-element');
			self.parentUrl = window.location.href;
			self.parentTitle = document.title;
			self.History =  window.History;
			self.rootUrl = self.History.getRootUrl();
			self.$body = $('body');
			self.listFlag = false,
			self.isAbsolute=(self.contents.hasClass('abs'))?true:false;
			
			// Check to see if History.js is enabled for our Browser
			if ( !self.History.enabled ) {
				return false;

			}
			
			self.ajaxify();
			self.bindStatechange();
			self.bindUIActions();
		},
		// ajaxify
		ajaxify : function(){

			var self = this;

			$('body').on('click',self.itemsClass,function(event){
				
				
				// Prepare
				var
					$this = $(this),
					url = $this.attr('href'),
					title = $this.attr('title')||null;

				// Continue as normal for cmd clicks etc
				if ( event.which == 2 || event.metaKey ) { return true; }

				// Ajaxify this link
				self.History.pushState(null,title,url);

				event.preventDefault();
				return false;
				
			});
		},
		// bind the state change to window and do stuff
		bindStatechange : function(){

			var self = this;

			$(window).bind('statechange',function(){
				// Prepare Variables
				var
					State = self.History.getState(),
					url = State.url,
					relativeUrl = url.replace(self.rootUrl,'');
				

				// Set Loading
				self.$body.addClass('loading');
				self.showLoader(function(){
					
					if (self.listFlag || url == self.parentUrl){

						self.itemContainer.html('');
						self.ajaxElements.removeClass('ajax-hide');
						if (self.isAbsolute){
							self.contents.addClass('abs');
						}
						$(window).trigger('debouncedresize');
						self.hideLoader();
						self.$body.removeClass('loading');
						self.listFlag = false;

					}else{
						//make ajax call
						$.ajax({
							type:'GET',
					        url:url,
					        datatype:'html',
					        success :function(data, textStatus, jqXHR){
					        	
					        	var
								$data = $(self.documentHtml(data)),
								$dataBody = $data.find('.document-body:first'),
								$dataContent = $dataBody.find(self.contentSelector).filter(':first'),
								contentHtml, $scripts;

								// Fetch the scripts
								$scripts = $dataContent.find('.document-script');
								if ( $scripts.length ) {
									$scripts.detach();
								}

								// Fetch the content
								contentHtml = $dataContent.html()||$data.html();
								if ( !contentHtml ) {
									document.location.href = url;
									return false;
								}

								//update content
								self.changeDom(contentHtml);

								// Update the title
								document.title = $data.find('.document-title:first').text();
								try {
									document.getElementsByTagName('title')[0].innerHTML = document.title.replace('<','&lt;').replace('>','&gt;').replace(' & ',' &amp; ');
								}
								catch ( Exception ) { }

								// Add the scripts
								$scripts.each(function(){
									var $script = $(this), 
										scriptText = $script.text(), 
										scriptNode = document.createElement('script');

									if ( $script.attr('src') ) {
										if ( !$script[0].async ) { scriptNode.async = false; }
										scriptNode.src = $script.attr('src');
									}
			    					scriptNode.appendChild(document.createTextNode(scriptText));
									contentNode.appendChild(scriptNode);
								});


								self.$body.removeClass('loading');
								self.hideLoader();

								// Inform Google Analytics of the change
								if ( typeof window._gaq !== 'undefined' ) {
									window._gaq.push(['_trackPageview', relativeUrl]);
								}
								

					        },
					        error: function(){
					        	document.location.href = url;
					        	self.hideLoader();
								self.$body.removeClass('loading');
								return false;
					        }
						});
					}

						
				});

				


			});
		},
		// utility function to translate the html response understandable for jQuery
		documentHtml : function(html){
			// Prepare
			var result = String(html)
				.replace(/<\!DOCTYPE[^>]*>/i, '')
				.replace(/<(html|head|body|title|meta|script)([\s\>])/gi,'<div class="document-$1"$2')
				.replace(/<\/(html|head|body|title|meta|script)\>/gi,'</div>')
			;

			// Return
			return $.trim(result);
		},

		showLoader:function(callback){
			var self=this;
			TweenMax.to(self.loader,1,{width:'100%', ease:Power2.easeOut,onComplete:function(){
				if (typeof (callback)=='function'){
					callback();
				}
			}});
		},
		hideLoader:function(){
			var self=this;
			TweenMax.to(self.loader,1,{width:0, ease:Power2.easeOut});

			
		},
		changeDom:function(data){
			var self=this;
			self.itemContainer.html(data);
			self.ajaxElements.addClass('ajax-hide');
			if (self.isAbsolute){
				self.contents.removeClass('abs');
			}
			self.itemContainer.css('display','block');
			self.hideLoader();
			self.itemContainer.trigger('newcontent');
			sideS.$exteras=$('.move-with-js').add('.mfp-wrap');
			$(window).scrollTop(0);
			inviewAnimate($('.inview-animate'));
			sideS.$main= $('div#main-content, .page-side');

		},
		bindUIActions:function(){
			var self=this;

			$('body').on('click','.portfolio-close',function(e){
				e.preventDefault();

				self.listFlag = true;
				self.History.pushState(null,self.parentTitle,self.parentUrl);
					
				
			});

			self.itemContainer.on('newcontent',function(){
				//Run functions that you need to excute on new portfolio item
			   
			    self.itemContainer.find('.sync-width').each(function(){
			    	var $this=$(this);
			    	 syncWidth($this,$this.parent('.sync-width-parent').first());
			    });
			    setBg(self.itemContainer.find('.set-bg'));

			    parallaxElems.init();
	   			
	   			videobg();
			});
		}

	}

	/* blog
	----------------------------------------------*/
	var bs,blog = {
	  
		settings : {
			isotopeContainer : $('#blog-list'), 
			postItems : $('#blog-list .post-item'),
			blogMore : $("#blog-more")
		},

		init : function(){
			bs = this.settings;
			this.buildIsotope();
			this.bindUIActions();
			this.transform=(rtlDirectionFlag)?false:true;

		},

		buildIsotope : function(){
			var self=this;

			bs.isotopeContainer.isotope({
			// options
			  itemSelector : '.post-item',
			  transformsEnabled: self.transform,
			  duration:750,
			  resizable:true,
			  resizesContainer:true,
			  layoutMode:'masonry'
			}); 
		},

		bindUIActions : function(){
			var self = this;

			//add items 
			//
			// You should implement this as an ajax callback
			//
			bs.blogMore.on('click',function(e){
				
			    e.preventDefault();
			    
			    //Here we are just inserting first 4 post
			    var $newItems = bs.postItems.filter(function(index){
			    	return index<5;
			    }).clone();
			      bs.isotopeContainer.isotope( 'insert', $newItems, function(){
			      	bs.isotopeContainer.isotope( 'reLayout');
			    });
			    
			    return false;
			});

		}
	}


	/* light box
	----------------------------------------------*/
	var lightBox={

		init:function(){
			var self=this;

			self.localvideo={
				autoPlay:false,
				preload:'metadata',
				webm :true,
				ogv:false	
			}

			this.bindUIActions();

			
		},generateVideo:function(src,poster){
			var self=this;
			//here we generate video markup for html5 local video
			//We assumed that you have mp4 and webm or ogv format in samepath (video/01/01.mp4 & video/01/01.webm)
			var basePath=src.substr(0, src.lastIndexOf('.mp4'));
			var headOptions='';
			if (self.localvideo.autoPlay){
				headOptions+=' autoplay';
			}
			headOptions +='preload="'+self.localvideo.preload+'"';

			var markup='<video class="mejs-player popup-mejs video-html5" controls '+headOptions+' poster="'+poster+'">'+
				'<source src="'+src+'" type="video/mp4" />';

			if (self.localvideo.webm){
				markup+='<source src="'+basePath+'.webm" type="video/webm" />'
			}
	
			if (self.localvideo.ogv){
				markup+='<source src="'+basePath+'.ogv" type="video/ogg" />'
			}
			markup+='</video>'+'<div class="mfp-close"></div>';

			return markup;

		},bindUIActions:function(){
			var self=this,
				$body=$('body');

			self.singleBox($('.tj-lightbox'));

			$('.tj-lightbox-gallery').each(function(){
				self.galleyBox($(this));	
			});

			$('body').on('click','.mfp-container',function(e){
				if( e.target !== this ) 
       				return;
				$(this).find('.mfp-close').trigger('click');
			});

			


		},singleBox:function($elem){
			var self=this;
			$elem.magnificPopup({
				type: 'image',
				closeOnContentClick: false,
				closeOnBgClick:false,
				mainClass: 'mfp-fade',
				 iframe: {
					markup: '<div class="mfp-iframe-scaler">'+
				            '<div class="mfp-close"></div>'+
				            '<iframe class="mfp-iframe" frameborder="0" allowfullscreen></iframe>'+
				            '<div class="mfp-title"></div>'+
				          '</div>'
				},
				callbacks:{
					elementParse: function(item) {
						var popType=item.el.attr('data-type')||'image';
						if (popType=='localvideo'){
							item.type='inline';
							var poster=item.el.attr('data-poster')||'';
							item.src=self.generateVideo(item.src,poster);
						}else{
							item.type=popType;
						}
				    },
		    		markupParse: function(template, values, item) {
				    	values.title = item.el.attr('title');
				    },
				    open: function() {
				    	sideS.$exteras=$('.move-with-js').add('.mfp-wrap');
				  		$('.popup-mejs').mediaelementplayer();
				  	}
		    	},
				image: {
					verticalFit: true
				}
			});

		},galleyBox:function($elem){
			var self=this,
				$this=$elem,
				itemsArray=[];


				
				$elem.magnificPopup({
					delegate: '.lightbox-gallery-item',
				    closeOnBgClick:false,
				    closeOnContentClick:false,
				    removalDelay: 300,
				    mainClass: 'mfp-fade',
				    iframe: {
						markup: '<div class="mfp-iframe-scaler">'+
					            '<div class="mfp-close"></div>'+
					            '<iframe class="mfp-iframe" frameborder="0" allowfullscreen></iframe>'+
					            '<div class="mfp-title"></div>'+
					            '<div class="mfp-counter"></div>'+
					          '</div>'
					},
				    gallery: {
				      enabled: true,
				       tPrev: 'Previous',
					   tNext: 'Next',
					   tCounter: '%curr% / %total%',
					   arrowMarkup: '<a class="tj-mp-action tj-mp-arrow-%dir% mfp-prevent-close" title="%title%"><i class="fa fa-angle-%dir%"></i></a>',
				    },
				    callbacks:{
				    	elementParse:function(item){
							
							var	popType=item.el.attr('data-type') || 'image',
								source=item.el.attr('href');
							

							if (popType=='localvideo'){
								item.src=self.generateVideo(source,item.el.attr('data-poster')||'');
								item.type='inline';
							}else{
								item.type=popType;
							}

				    	},
				    	open:function(){
							sideS.$exteras=$('.move-with-js').add('.mfp-wrap');
							$('.popup-mejs').mediaelementplayer();
				    	},
				    	change: function() {
					        if (this.isOpen) {
					            this.wrap.addClass('mfp-open');
					        }
					        //console.log($('.popup-mejs'));
					       $('.popup-mejs').mediaelementplayer();
					    }
				    },
				    type: 'image' // this is a default type
				});

				itemsArray=[];
		}
	}


	lightBox.init();
	
	/* accordion
		This has an option to open the desired tab by default
		you can add a class of .active to the item you want to be oppened by default
	----------------------------------------------*/
	var acc,accordion={
		settings : {
			accUIelClass : ".accordion > .item > .head a",
			openFirstOne : true //change to false if you want them all be closed by default 
		},
		init : function(){
			acc = this.settings;
			this.bindUIActions();

			//close all bodies except the stated one
			$(".accordion > .item:not(.active) > .body").hide();
			
			// if none of els are stated active open up the first one
			if ( $(".accordion > .item.active").length == 0 && acc.openFirstOne )
				$(".accordion > .item:first-child > .body").addClass('active').slideDown();
			 
		},
		bindUIActions : function(){
			var self = this, $body = $('body');
			$body.on('click', acc.accUIelClass, function(event){
				var $this = $(this);
				$this.parents(".item").addClass("active").siblings().removeClass("active");
				$(".accordion > .item:not(.active) > .body").slideUp();
				$this.parent().next().slideDown();
				
				event.preventDefault();
				return false;
			});
		}
	}
	accordion.init();

	/* Tabs
		This has an option to open the desired tab by default
		you can add a class of .active to li item and .tab-item to make it active
	----------------------------------------------*/
	var tabS,tabs={
		settings : {
			tabsBodiesExceptActive : $(".tabs > .tabs-body > .tab-item:not(.active)"),
			tabsUIelClass : ".tabs > ul.tabs-head a"
		},
		init : function(){
			tabS = this.settings;
			this.bindUIActions();

			//if we dont have an active one set the first one to be active
			if ( $(".tabs > .tabs-head > li.active").length == 0)
			$(".tabs > .tabs-body > .tab-item:first-child, .tabs > .tabs-head > li:first-child").addClass('active');
		},
		bindUIActions : function(){
			var self = this, 
				$body = $('body');

			$body.on('click', tabS.tabsUIelClass, function(event){
				var $this = $(this).parent(),
					index = $("ul.tabs-head li").index($this);

				$this.addClass('active').siblings().removeClass('active');
				$(".tabs > .tabs-body > .tab-item").eq(index).addClass('active').siblings().removeClass('active');

				
				//$this.parent().addClass('active').siblings().removeClass("active");
				
				event.preventDefault();
				return false;
			});
		}
	}
	tabs.init();


	/* Team carousel
	Please refer to the documentation of MasterSlider
	----------------------------------------------*/
	var teamCarousel = {
		
		init : function(){
			var slider = new MasterSlider();
		    slider.setup('teamcarousel' , {
		        loop:true,
		        width:240,
		        height:240,
		        speed:20,
		        view:'fadeBasic',
		        preload:0,
		        space:0,
		        wheel:true
		    });
		    slider.control('arrows');
		    slider.control('slideinfo',{insertTo:'#teamcarousel-info'});
		}
	}

	if ($('#teamcarousel').length>0){
		teamCarousel.init();
	}

	var msSlider={

		//This is the function that controlls all masterslider instances or atleast it tries to do!
		init:function($target){
			var self=this;

			//Select the target which can be passed to the function or auto selected from DOM
			var $elem=$target || $('.tj-ms-slider');

	
			//Call the main function that apply master slider on selected elements
			$elem.each(function(){

				var $this=$(this);

				//Some basic settings
				var msOptions={
					ewidth:$this.attr('data-width')|| $this.width(),
					eheight:$this.attr('data-height') || $this.height(),
					layout:$this.attr('data-layout') || 'boxed',
					view:$this.attr('data-view') || 'basic',
					dir:$this.attr('data-dir')||'h',
					showCounter:$this.attr('data-counter') || false,
					isGallery:$this.attr('data-gallery') || false,
					autoHeight:$this.attr('data-autoheight') || false,
					hasPlay:$this.attr('data-playbtn') || false,
					galleryWrapper:$this.parents('.tj-ms-gallery'),
					mouse:($this.attr('data-mouse')||true)==true,
					fillMode:$this.attr('data-fillmode')||'fill',
					initialPlay:false
				}

				self.runSlider($this,msOptions);
			});
		},
		runSlider:function($elem,options){

			var self=this;

			var slider = new MasterSlider(),
				elemID=$elem.attr('id')||'';


		    slider.setup(elemID , {
		        width:options.ewidth,
		        height:options.eheight,
		        layout:options.layout,
		        view:options.view,
		        dir:options.dir,
		        autoHeight:options.autoHeight,
		        space:0,
		        preload:1,
		        centerControls:false,
		        mouse:options.mouse,
		        fillMode:options.fillMode,
		        overPause:false
	   		 });

		    slider.control('arrows',{autohide:false}); 
		    if (options.hasPlay){
		    	slider.control('timebar'  , {autohide:false,color:"#dc971f"});
		    }

		     if (options.isGallery){
		    	self.makeGallery($elem,slider);	
		     }


		    slider.api.addEventListener(MSSliderEvent.INIT , function(){
		    	var $controlsWrapper=$elem.find('.ms-container'),
		    		$controlUI=$('<div class="tj-controlls tj-controlls-'+options.dir+'mode"></div>').appendTo($controlsWrapper);

		    	//Play pause button
		    	 if (options.hasPlay){
			    	var playBtn=$('<div></div>').addClass('tj-playbtn').appendTo($controlUI);
			    	self.addPlay(slider,playBtn,options);
			    	
			    }

		    	//Next & Prev buttons
		    	$controlUI.append($controlsWrapper.find('.ms-nav-prev'));
		    	//First one is prev and last next add anything in betwen(eg.counter)

			    if (options.showCounter){
	    			var counterMarkup='<div class="tj-ms-counter">'+
		    						'<span class="counter-current">'+(slider.api.index()+1)+'</span>'+
		    						'<span class="counter-divider">/</span>'+
		    						'<span class="counter-total">'+slider.api.count()+'</span>'+
	    						  '</div>';
	    			var $counterMarkup=$(counterMarkup).appendTo($controlUI),
	    				$current=$counterMarkup.find('.counter-current'); 
			    	self.sliderCounter($elem,slider,$current);


			    }

			    slider.api.resume();

		    	$controlUI.append($controlsWrapper.find('.ms-nav-next'));

		    	$(window).trigger('resize');
		    });


		},sliderCounter:function($elem,slider,$current){
		    var self=this;

			slider.api.addEventListener(MSSliderEvent.CHANGE_START , function(){
	    		//slider slide change listener
	    		$current.html(''+(slider.api.index()+1)+'');
			});

		},makeGallery:function($elem,slider){
			//Do the gallery things here... 
			var direction=($elem.hasClass('tj-vertical-gallery'))?'v':'h';

			slider.control('thumblist' , {autohide:false  , dir:direction});


		},addPlay:function(slider,playbtn,options){
			if(options.initialPlay){
				slider.api.resume();
				playbtn.addClass('btn-pause');
			}
		    playbtn.click(function(){
				if(slider.api.paused){
					 slider.api.resume();
					 playbtn.addClass('btn-pause');
				}else{
					 slider.api.pause();
					 playbtn.removeClass('btn-pause');
				}
			});
		}
	}

	

	/* Responsive behavior handler
	----------------------------------------------*/
	var responsive={
		init:function(){
			var self=this;
			
			self.mdWidth=992;
			self.$window=$(window);

			self.folio={},self.blog={};

			self.bindUIActions();



			if (self.$window.width()<=self.mdWidth){
				self.portfolio('md');
				self.absPortfolio('md');
			}else{
				self.portfolio();
				self.absPortfolio();
			}
			
		},
		portfolio:function(state){
			var self=this,
				wHeight=$(window).height();

			self.folio.pHead=$('.parallax-head');
			self.blog.pHead=$('.header-cover');
			self.folio.pContent = $('.parallax-contents');
			self.folio.mdDetail = $(".portfolio-md-detail");



			var self=this;
			if (state == 'md'){
				self.folio.pHead.css('height',wHeight);
				self.blog.pHead.css('height',wHeight);
				self.folio.pContent.css('marginTop',self.$window.height());
				self.folio.mdDetail.css('marginTop',-$(".portfolio-md-detail").outerHeight());
				
			}else{
				self.folio.pHead.css('height',600);
				self.blog.pHead.css('height',600);
				self.folio.pContent.css('marginTop',600);
			}
			

		},
		absPortfolio:function(state){
			var self=this;
			if (state == 'md'){
				$(".set-height-mobile").height(self.$window.height()-$(".page-side").outerHeight());
			}else{
				$(".set-height-mobile").css('height','100%');
			}
		},
		bindUIActions:function(){
			var self=this;
			$(window).on('debouncedresize',function(){
				if ($(this).width()<=self.mdWidth){
					self.portfolio('md');
					self.absPortfolio('md');
				}else{
					self.portfolio();
					self.absPortfolio();
				}
			});
		}
	}


	/*Initialize require methods 
	----------------------------------------------*/
	var initRequired={

	  init:function(){
	  	$(window).load(function(){
	  		portfolios.init();
	  		blogrid.init();	
	  	})

	  	msSlider.init();
	  	ajaxFolio.init();
	    blog.init();
	    //imageFill.init($('.fill-images'));
	    $('.sync-width').each(function(){
	    	 syncWidth($(this),$(this).parent('.sync-width-parent').first());
	    });
	    setBg($('.set-bg'));
	   
	    sidebar.init();

	    responsive.init();
	    parallaxElems.init();
	    videobg();
	    //contact form handler
	    submitContact();
	    setMinHeight();
	    inviewAnimate($('.inview-animate'));
	    touchDevices();
	  
	    this.bindUIActions();


	  },
	  bindUIActions:function(){
	    	$(window).on('debouncedresize',function(){
	    		setMinHeight();
	    	});


	  }
	}
	initRequired.init();

});

//Add some fallbacks for touch devices
function touchDevices(){
	if(isTouchSupported()){
		$('body').addClass('touch-device');
	}else{
		$('.videobg-fallback').on('click',function(e){
			e.preventDefault();
			return false
		});
	}

}

//trigger background videos
function videobg(){
	$('.owl-videobg').owlVideoBg({
		    
    	autoGenerate:{
    		posterImageFormat:'png'
    	},
    	preload:'auto'
	    	
    });
}

//Set min-height for pages
function setMinHeight(){
	var pageWrapper=$('.page-wrapper');
	if (pageWrapper.parents('#main-content.abs').length==0){
		pageWrapper.css('min-height',$(window).height());
	}	
}

//center an element
function centerIt(elem,height,offset){

	if (offset==undefined){
		offset=100;
	}

	if(height=='parent'){
		height=elem.parents().height();
	}

	var elemHeight=elem.height(),
		elemMargin;

	if ((height-elemHeight)>2*offset){
		//We have enough space
		elemMargin=(height-elemHeight)/2;
	}
	else{
		//Just set basic margin
		elemMargin=offset;
	}

	elem.css('margin-top',elemMargin);
}

//Inview scale handler
function inviewAnimate(elem){
	elem.each(function(){
		$(this).bind('inview', function(event, isInView, visiblePartX, visiblePartY) {
		  var $this=$(this);

		  if (isInView) {
		    // element is now visible in the viewport
		    $this.addClass('visible-view');
	      	$this.unbind('inview');
		    if (visiblePartY == 'top') {
		      // top part of element is visible
		      $this.addClass('visible-view');
		      $this.unbind('inview');
		    } 
		  }
		});
	});
		
}

//Set background image base on child image
function setBg($elem){
	$elem.each(function(){
		var $this=$(this);
		var bgImg = $this.find('img').first();
		$this.css({
			'background' : 'url('+bgImg.attr('src')+') no-repeat 50% 50%',
			'background-size':'cover'
		});

		bgImg.hide();
	});
	
}

//Sync width child,parent
function syncWidth($child,$parent){
	$child.css('width',$parent.width());

	$(window).on('debouncedresize',function(){
		syncWidth($child,$parent);
	});
}


//Ajax contact form 
function submitContact() {
    var contactForm = $('form#contact-form');

    contactForm.submit(function(e) {
        e.preventDefault();
        if ($("#alert-wrapper").length) {
            return false;
        }

        var alertWrapper = $('<div id="alert-wrapper"><button type="button" class="close" data-dismiss="alert">X</div>').appendTo(contactForm);
        $('form#contact-form .alert').remove();

        var hasError = false,
            ajaxError = false;

        //form input validation     
        contactForm.find('.requiredField').each(function() {
            if ($.trim($(this).val()) == '') {
                var labelText = $(this).attr('placeholder');
                alertWrapper.append('<div class="alert">You forgot to enter your ' + labelText + '.</div>');
                hasError = true;
            } else if ($(this).hasClass('email')) {
                var emailReg = /^([\w-\.]+@([\w-]+\.)+[\w-]{2,4})?$/;
                if (!emailReg.test($.trim($(this).val()))) {
                    var labelText = $(this).attr('placeholder');
                    alertWrapper.append('<div class="alert"> You\'ve entered an invalid ' + labelText + '.</div>');
                    hasError = true;
                }
            }
        });

        //Showing alert popup
        var showAlert = new TimelineLite({paused: true});
        hideAlert = new TimelineLite({paused: true});
        showAlert.to(alertWrapper, 0.3, {opacity: 1, top: '30%'});
        hideAlert.to(alertWrapper, 0.3, {opacity: 0, top: '60%', onComplete: function() {
                alertWrapper.remove();
        }});

        if (hasError) {
            //Thers is  error in form inputs show alerts
            showAlert.play();
            alertWrapper.find('button').on('click', function() {
                hideAlert.play();
            })
        }
        else {
            //Validation passed send form data to contact.php file via ajax
            var formInput = $(this).serialize();
            $.ajax({
                type: 'POST',
                url: $(this).attr('action'),
                dataType: 'json',
                data: formInput,
                success: function(data) {
                    //Ajax request success
                    if (data.status == "error") {
                        ajaxError = true;
                        contactForm.append('<div class="alert"><strong>Sorry</strong> There was an error sending your message!</div>');
                    } else if (data.status == 'ok') {
                        contactForm.slideUp(300, function() {
                            $(this).before('<div class="alert"><strong>Thanks</strong> Your email has been delivered. </div>');
                        });
                    }
                },
                error: function() {
                    //Ajax request success
                    ajaxError = true;
                    $('form#contact-form').append('<div class="alert"><strong>Sorry</strong> There was an error sending your message!</div>');
                }
            });
        }
        if (ajaxError) {
            //Ajax request had some errors
            showAlert.play();
            alertWrapper.find('button').on('click', function() {
                hideAlert.play();
            });
        }
        return false;
    });
}

/* Detect touch devices*/
function isTouchSupported(){
        //check if device supports touch
        var msTouchEnabled = window.navigator.msMaxTouchPoints;
        var generalTouchEnabled = "ontouchstart" in document.createElement("div");
     
        if (msTouchEnabled || generalTouchEnabled) {
            return true;
        }
        return false;

  }

/*
 * debouncedresize: special jQuery event that happens once after a window resize
 */
(function($) {

var $event = $.event,
	$special,
	resizeTimeout;

$special = $event.special.debouncedresize = {
	setup: function() {
		$( this ).on( "resize", $special.handler );
	},
	teardown: function() {
		$( this ).off( "resize", $special.handler );
	},
	handler: function( event, execAsap ) {
		// Save the context
		var context = this,
			args = arguments,
			dispatch = function() {
				// set correct event type
				event.type = "debouncedresize";
				$event.dispatch.apply( context, args );
			};

		if ( resizeTimeout ) {
			clearTimeout( resizeTimeout );
		}

		execAsap ?
			dispatch() :
			resizeTimeout = setTimeout( dispatch, $special.threshold );
	},
	threshold: 150
};

})(jQuery);



