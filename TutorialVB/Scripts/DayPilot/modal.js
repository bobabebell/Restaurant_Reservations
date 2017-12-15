/* Copyright © 2005 - 2012 Annpoint, s.r.o.
   Use of this software is subject to license terms. 
   http://www.daypilot.org/

*/

if (typeof(DayPilot) === 'undefined') {
    DayPilot = {};
}

(function() {

    DayPilot.ModalStatic = {};
	
	DayPilot.ModalStatic.list = [];
	
	// hide the last one
	DayPilot.ModalStatic.hide = function() {
		if (this.list.length > 0) {
			var last = this.list.pop();
			if (last) {
				last.hide();
			}
		}
	};
	
	DayPilot.ModalStatic.remove = function(modal) {
		var list = DayPilot.ModalStatic.list;
		for (var i = 0; i < list.length; i++) {
			if (list[i] === modal) {
				list.splice(i, 1);
				return;
			}
		}
	};
	
	DayPilot.ModalStatic.result = function(r) {
	    var list = DayPilot.ModalStatic.list;
	    if (list.length > 0) {
	        list[list.length - 1].result = r;
	    }
	};
	
	DayPilot.ModalStatic.displayed = function(modal) {
		var list = DayPilot.ModalStatic.list;
		for (var i = 0; i < list.length; i++) {
			if (list[i] === modal) {
				return true;
			}
		}
		return false;
	};

    var isIE = (navigator && navigator.userAgent && navigator.userAgent.indexOf("MSIE") != -1);

    DayPilot.Modal = function() {

        // default values
        this.width = 300;
        this.height = 460;
        this.top = 20;
        this.opacity = 30;
        this.border = "1px solid black";
        this.zIndex = null;
        this.corners = 'Rounded';
        this.className = null;
        
        // internal
        var This = this;

        this.id = '_' + new Date().getTime() + 'n' + (Math.random()*10); 
        
        // event handler
        this.closed = null;
        
        this.registered = false;
        
        // drag&drop
        this.start = null;
        this.coords = null;
        
        // additional config
        this.useIframe = true;
        
        // experimental
        this.dragDrop = true;

        this.showHtml = function(html) {

			if (DayPilot.ModalStatic.displayed(this)) {
				throw "This modal dialog is already displayed.";
			}

	        if (!this.div) {
		        this.create();
	        }
	        this.update();

	        if (this.useIframe) {
		        var delayed = function(p, innerHTML) {
	    	        return function() {
	        	        p.setInnerHTML(p.id + "iframe", innerHTML);
	                };
	            };
	                
		        window.setTimeout(delayed(this, html), 0);
	        }
	        else {
	        	this.div.innerHTML = html;
	        }
	        
	        this.register();

        };
        
        this.rounded = function() {
            return this.corners && this.corners.toLowerCase() == 'rounded';
        };

        this.showUrl = function(url) {

			if (DayPilot.ModalStatic.displayed(this)) {
				throw "This modal dialog is already displayed.";
			}
		
        	this.useIframe = true; // forced
        
            // another modal is already open
			/*
            if (DayPilot.ModalStatic) {
                return;
            }
			*/

	        if (!this.div) {
		        this.create();
	        }

            this.update();
	        //DayPilot.ModalStatic = this;
        	
	        this.iframe.src = url;

	        this.register();
        };
        
        this.update = function() {
            var scrollY = window.pageYOffset ? window.pageYOffset : ((document.documentElement && document.documentElement.scrollTop) ? document.documentElement.scrollTop : document.body.scrollTop);
            var height = function () {
                var D = document;
                return Math.max(
                    Math.max(D.body.scrollHeight, D.documentElement.scrollHeight),
                    Math.max(D.body.offsetHeight, D.documentElement.offsetHeight),
                    Math.max(D.body.clientHeight, D.documentElement.clientHeight)
                ) - (isIE ? 4 : 0);  // guess value, fix for IE
            };

	        this.hideDiv.style.height = height() + "px";
	        this.hideDiv.style.filter = "alpha(opacity=" + this.opacity + ")";
	        this.hideDiv.style.opacity = "0." + this.opacity;
	        this.hideDiv.style.backgroundColor = "black";
	        if (this.zIndex) {
	            this.hideDiv.style.zIndex = this.zIndex;
	        }
		    this.hideDiv.style.display = '';

		    this.div.className = this.className;
	        this.div.style.border = this.border;
	        if (this.rounded()) {
	            this.div.style.MozBorderRadius = "5px";
	            this.div.style.webkitBorderRadius = "5px";
	            this.div.style.borderRadius = "5px";
	        }
	        this.div.style.marginLeft = '-' + Math.floor(this.width/2) + "px";  // '-45%'
	        this.div.style.top = (scrollY + this.top) + 'px';
	        this.div.style.width = this.width + 'px';  // '90%'
	        if (this.height) {
		        this.div.style.height = this.height + 'px';
	        }
	        if (this.zIndex) {
	            this.div.style.zIndex = this.zIndex;
	        }
	        
	        if (this.useIframe && this.height) {
		        this.iframe.style.height = (this.height) + 'px';
	        }
	        
            this.div.style.display = '';
    		DayPilot.ModalStatic.list.push(this);
        };
        
        this.register = function() {
            if (this.registered) {
                return;
            }
            this.re(window, 'resize', this.resize);
            this.re(window, 'scroll', this.resize);
            
            if (this.dragDrop) {
                this.re(document, 'mousemove', this.drag);
                this.re(document, 'mouseup', this.drop);
            }
            this.registered = true;
            
        };
        
        this.drag = function(e) {
            if (!This.coords) {
                return;
            }
            
            var e = e || window.event;
            var now = This.mc(e);
            
            var x = now.x - This.coords.x;
            var y = now.y - This.coords.y;
            
            //This.iframe.style.display = 'none';
            This.div.style.marginLeft = '0px';
            This.div.style.top = (This.start.y + y) + "px";
            This.div.style.left = (This.start.x + x) + "px";
            
        };
        
        this.drop = function(e) {
            // no drag&drop
            if (!This.coords) {
                return;
            }
            //This.iframe.style.display = '';
            This.unmaskIframe();
            
            This.coords = null;
        };
        
        this.maskIframe = function() {
        	if (!this.useIframe) {
        		return;
        	}
        	
            var opacity = 80;
        
            var mask = document.createElement("div");
            mask.style.backgroundColor = "#ffffff";
	        mask.style.filter = "alpha(opacity=" + opacity + ")";
	        mask.style.opacity = "0." + opacity;
	        mask.style.width = "100%";
	        mask.style.height = this.height + "px";
	        mask.style.position = "absolute";
	        mask.style.left = '0px';
	        mask.style.top = '0px';
    	    
	        this.div.appendChild(mask);
	        this.mask = mask;
        };
        
        this.unmaskIframe = function() {
        	if (!this.useIframe) {
        		return;
        	}

            this.div.removeChild(this.mask);
            this.mask = null;
        };
        
        this.resize = function() {
        
            if (!This.hideDiv) {
                return;
            }
            if (!This.div) {
                return;
            }
            if (This.hideDiv.style.display == 'none') {
                return;
            }
            if (This.div.style.display == 'none') {
                return;
            }
            
            var scrollY = window.pageYOffset ? window.pageYOffset : ((document.documentElement && document.documentElement.scrollTop) ? document.documentElement.scrollTop : document.body.scrollTop);
            var height = function () {
                var D = document;
                return Math.max(
                    Math.max(D.body.scrollHeight, D.documentElement.scrollHeight),
                    Math.max(D.body.offsetHeight, D.documentElement.offsetHeight),
                    Math.max(D.body.clientHeight, D.documentElement.clientHeight)
                );
            };        
            
            This.hideDiv.style.height = height() + "px";
            This.div.style.top = (scrollY + This.top) + 'px';
        };
        
        // already available in common.js but this file should be standalone
        this.re = function (el, ev, func) {
            if (el.addEventListener) {
                el.addEventListener (ev, func, false);
            } else if (el.attachEvent) {
                el.attachEvent ("on" + ev, func);
            } 
        };
        
        // mouse coords
        this.mc = function(ev){
            if(ev.pageX || ev.pageY){
	            return {x:ev.pageX, y:ev.pageY};
            }
            return {
	            x:ev.clientX + document.documentElement.scrollLeft,
	            y:ev.clientY + document.documentElement.scrollTop
            };
        };
        
        // absolute element position on page
        this.abs = function (element) {
            var r = { 
                x: element.offsetLeft, 
                y: element.offsetTop
            };
            
            while (element.offsetParent) {
                element = element.offsetParent;   
                r.x += element.offsetLeft;
                r.y += element.offsetTop;
                //document.title += ' ' + element.offsetTop;
            }
            
            return r;
        };    
        

        this.create = function() {
	        var hide = document.createElement("div");
	        hide.id = this.id + "hide";
	        hide.style.position = 'absolute';
	        hide.style.left = "0px";
	        hide.style.top = "0px";
	        hide.style.width = "100%";
	        hide.style.backgroundColor = "black";
	        hide.onclick = function() { This.hide(); };
	        hide.oncontextmenu = function() { return false; };

	        document.body.style.height = '100%';

	        document.body.appendChild(hide);

	        var div = document.createElement("div");
	        div.id = this.id + 'popup';
	        div.style.position = 'absolute';
	        div.style.left = '50%';
	        div.style.backgroundColor = 'white';
	        if (this.dragDrop) {
	            div.onmousedown = this.dragStart;
	        }

	        var iframe = null;
	        if (this.useIframe) {
		        iframe = document.createElement("iframe");
		        iframe.id = this.id + "iframe";
		        iframe.name = this.id + "iframe";
		        iframe.frameBorder = '0';
		        iframe.style.width = '100%';
		        iframe.style.height = (this.height) + 'px';
		        div.appendChild(iframe);
	        }

	        document.body.appendChild(div);

	        this.div = div;
	        this.iframe = iframe;
	        this.hideDiv = hide;
        };
        
        
        this.dragStart = function(e) {
            This.maskIframe();
            This.coords = This.mc(e || window.event);
            This.start = {x : This.div.offsetLeft, y: This.div.offsetTop};
        };

        this.setInnerHTML = function(id, innerHTML) {
	        var frame = window.frames[id];

	        var doc = frame.contentWindow || frame.document || frame.contentDocument;
	        //alert(id + ' ' + frame);
	        if (doc.document) {
		        doc = doc.document;
	        }

	        doc.body.innerHTML = innerHTML;
        };

        this.hide = function() {
	        if (this.div) {
		        this.div.style.display = 'none';
		        this.hideDiv.style.display = 'none';
		        if (!this.useIframe) {
		        	this.div.innerHTML = null;
		        }
	        }
            //DayPilot.ModalStatic = null;
			DayPilot.ModalStatic.remove(this);
			
	        if (this.closed) {
	            this.closed();
	        }
        };

    };

})();
