/*
 * Single Window Application Template:
 * A basic starting point for your application.  Mostly a blank canvas.
 * 
 * In app.js, we generally take care of a few things:
 * - Bootstrap the application with any data we need
 * - Check for dependencies like device type, platform version or network connection
 * - Require and open our top-level UI component
 *  
 */

//bootstrap and check dependencies
if (Ti.version < 1.8 ) {
	alert('Sorry - this application template requires Titanium Mobile SDK 1.8 or later');	  	
}

// This is a single context application with multiple windows in a stack
(function() {
	//render appropriate components based on the platform and form factor
	var osname = Ti.Platform.osname,
		version = Ti.Platform.version,
		height = Ti.Platform.displayCaps.platformHeight,
		width = Ti.Platform.displayCaps.platformWidth;
	
	//considering tablet to have one dimension over 900px - this is imperfect, so you should feel free to decide
	//yourself what you consider a tablet form factor for android
	var isTablet = osname === 'ipad' || (osname === 'android' && (width > 899 || height > 899));
	
	var loginWin;
	var pWidth = Ti.Platform.displayCaps.platformWidth;
	var pHeight = Ti.Platform.displayCaps.platformHeight;
	
	Ti.App.SCREEN_WIDTH = (pWidth > pHeight) ? pHeight : pWidth;
	Ti.App.SCREEN_HEIGHT = (pWidth > pHeight) ? pWidth : pHeight;

	Ti.App.BUTTONHEIGHT = 80;
	Ti.App.HDRFONTSIZE = 20;
	Ti.App.ROWFONTSIZE = 20;
	Ti.App.ROWHEIGHT = 80;
	
	
	
	Ti.App.currUserCode = "";
	Ti.App.currOutletCode = "";
	
	Ti.App.hideSlow = Ti.UI.createAnimation({
		opacity: 0,
		duration: 500
	});
	
	Ti.App.showSlow = Ti.UI.createAnimation({
		opacity: 100,
		duration: 500
	});
	callbackReturn = function(dataBatch){
		for(var i = 0; i< dataBatch.length; i++){
			alert(dataBatch[i].BatchNum);
		}
	};
	
	Ti.App.indicator = Ti.UI.createActivityIndicator({
		width:50,
		height:50,
		message: 'loading...',
		color: '#FFFFFF'
	});
	
	
	//var service = Ti.App.iOS.registerBackgroundService({url:'ui/common/bgNotify.js'});

	
	if (isTablet) {
			loginWin = require('ui/common/LoginScreen');
			var login = loginWin.LoginScreen();
			login.open();
	
	}
	else {
		// Android uses platform-specific properties to create windows.
		// All other platforms follow a similar UI pattern.
		if (osname === 'android') {
			//Window = require('ui/handheld/android/ApplicationWindow');
		}
		else { // iphone
			Ti.App.HDRFONTSIZE = 13;
			Ti.App.ROWHEIGHT = 30;
			Ti.App.ROWFONTSIZE = 12;
			Ti.App.BUTTONHEIGHT = 40;
			
			loginWin = require('ui/common/LoginScreen');
			var login = loginWin.LoginScreen();
			login.open();
		}
	}
	
})();
