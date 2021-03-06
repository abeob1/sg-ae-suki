exports.LoginScreen = function(){
	
	Ti.include('/ui/utilities/RemoteAccess.js');
	Ti.include('/ui/utilities/LocalDB.js');
	Ti.include('/ui/common/LayoutControl.js');
	
	var newIndicat = Ti.UI.createActivityIndicator({
		style: Ti.UI.iPhone.ActivityIndicatorStyle.PLAIN,
		width:50,
		height:50,
		message: ''
	});
	
	newIndicat.hide();
	
	var win  = Ti.UI.createWindow({
		title:'Login',
		 backgroundColor:'#FFFFFF'
	});
	
	var slideUP = Titanium.UI.createAnimation({
		top: -100,
		duration: 300	
	});
	
	var slideDown = Titanium.UI.createAnimation({
		top: 0,
		duration: 300
	});
	
	var centerWidth = 400;
	var centerHeight = 300;
	
	var centerView = Ti.UI.createView({
		width: centerWidth,
		height: centerHeight,
		top: Ti.App.SCREEN_HEIGHT/2 - 250,//80,
		left: Ti.App.SCREEN_WIDTH/2 - 200,//25,
		borderRadius:20,
		layout:'vertical',
		backgroundImage:'/images/backgr.jpg'
	});
	
	var separateView = Ti.UI.createView({
		backgroundColor:'transparent',
		height:10	
	});

	var lbLoginInfo = Ti.UI.createLabel({
		text:'    Login Information',
		width:'auto',
		color:'#FFFFFF',
		font:{fontSize:18}
	});
	var txt_Username  =Ti.UI.createTextField({
		hintText: 'Username',
		height:  70,
		width: 380,
		textAlign:'left',
		borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
		autocapitalization:false,
		font:{fontSize:20, fontFamily:'Helvetica-Bold'}
	});
	
	 txt_Username.addEventListener('focus', function(e) {
	 	win.animate(slideUP);
     });
     txt_Username.addEventListener('blur', function(e) {
	 	win.animate(slideDown);
     });
     
     
	var txt_Password = Ti.UI.createTextField({
		hintText: 'Password',
		height: 70,
		width: 380,
		textAlign:'left',
		borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
		passwordMask:'true',
		autocapitalization:false,
		font:{fontSize:19, fontFamily:'Helvetica-Bold'}
	});
	txt_Password.addEventListener('focus', function(e) {
	 	win.animate(slideUP);
     });
	txt_Password.addEventListener('blur', function(e) {
	 	win.animate(slideDown);
     });
	
	var cre = getCredential();
	if(cre != ''){
		txt_Username.value = cre.Username;
		txt_Password.value = cre.Password;
	}
	/*
	var btn_Setting = Ti.UI.createButton({
		title:'Settings'
		//systemButton:Ti.UI.iPhone.SystemButton.EDIT
	});
	
	btn_Setting.addEventListener('click', function(e){
		var SW = require('ui/common/Settings');
		var setting = SW.Settings();
		setting.open({modal:true});
	});
	*/
	var btn_Login = Ti.UI.createButton({
		title: 'Submit',
		width:'100'
	});
	
	var slideRight = Ti.UI.createAnimation();
	slideRight.left = 320;
	slideRight.duration = 300;

	callbackFunFact = function(_text){
		newIndicat.hide();
		var doc = Ti.XML.parseString(_text);
		var items = doc.getElementsByTagName('string');
		if(items.length>0){
			if(items.item(0).text !== ''){
				Ti.App.currUserCode = items.item(0).text;
				root.close(Ti.App.hideSlow);
				var winDo = require('ui/common/SummaryOrder');
				var summaryOrder = winDo.SummaryOrder();
				summaryOrder.open(Ti.App.showSlow);
			}
			else{
				ShowMessage('Wrong username or password');
			}		
		}else
			ShowMessage('Data lost.');
	};	

	callbackError = function(_message){
		newIndicat.hide();
		ShowMessage(_message);
	};
	
	btn_Login.addEventListener('click', function(e){
		newIndicat.show();
		var _username = txt_Username.value;
		var _password = txt_Password.value;
		if(swCreden.value === true){
			if(cre === ''){
				saveCredential(_username, _password);
			}else{
				updateCredential(_username, _password);
			}
		}else
			deleteCredential();
		
		userLogin(_username, _password, callbackFunFact, callbackError);
	});
	
	var viewCreden = Ti.UI.createView({
		backgroundColor:'transparent',
		width:240,
		layout: 'horizontal',
		height:70
	});
	
	var lbCredential  =Ti.UI.createLabel({
		text:'Save Credential?',
		width:'auto',
		color:'#FFFFFF',
		font:{fontSize:18}
	});
	
	
	
	var swCreden = Ti.UI.createSwitch({
		value:true
	});
	
	viewCreden.add(lbCredential);
	viewCreden.add(swCreden);

	centerView.add(separateView);
	centerView.add(lbLoginInfo);
	centerView.add(newIndicat);
	centerView.add(txt_Username);
	centerView.add(Ti.UI.createView({height:5}));
	centerView.add(txt_Password);
	centerView.add(Ti.UI.createView({height:5}));
	centerView.add(viewCreden);
	
	win.add(centerView);
	
	var logo = Ti.UI.createView({
		width: 100,
		height: 100,
		borderRadius:5,
		top: Ti.App.SCREEN_HEIGHT/2 - 280,//80,
		left: Ti.App.SCREEN_WIDTH/2 - 190,//25,
		backgroundImage: '/images/SukiS.png'
	});

	win.add(logo);
	win.setRightNavButton(btn_Login);
	
	var nav = Titanium.UI.iPhone.createNavigationGroup({
    	window:win
	});
	
	var root = Titanium.UI.createWindow({
	});
	
	root.add(nav);
	return root;	
};