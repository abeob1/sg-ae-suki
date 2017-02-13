exports.Acknowledge = function(_DocEntry, _DBName, _GRPODate, _callbackClose){
	
	Ti.include('/ui/utilities/RemoteAccess.js');
	Ti.include('/ui/utilities/LocalDB.js');
	
	Ti.include('/ui/common/LayoutControl.js');
	
	var newIndicat = Ti.UI.createActivityIndicator({
		style: Ti.UI.iPhone.ActivityIndicatorStyle.PLAIN,
		width:50,
		height:50,
		message: 'Processing...',
		color:'#FFFFFF'
	});
	
	newIndicat.hide();
	var win  = Ti.UI.createWindow({
		title:'Acknowledge',
		backgroundColor:'transparent',
		backgroundImage:'NONE',
		opacity:0
	});
	
	var slideUP = Titanium.UI.createAnimation({
		top: -250,
		duration: 300	
	});
	
	var slideDown = Titanium.UI.createAnimation({
		top:0,
		duration: 300
	});
	
	var centerView = Ti.UI.createView({
		width: 400,
		height: 300,
		borderRadius: 20,
		borderWidth: 2,
		layout:'vertical',
		backgroundImage:'/images/backgr.jpg'
		
	});
	
	var separateView = Ti.UI.createView({
		backgroundColor:'transparent',
		height:10	
	});
	
	win.add(Ti.App.indicator);
	Ti.App.indicator.hide();
	
	var lbLoginInfo = Ti.UI.createLabel({
		text:'Account',
		width:'auto',
		color:'#FFFFFF',
		font:{fontSize:20, fontFamily:'Helvetica-Bold'}
	});
	var txt_Username  =Ti.UI.createTextField({
		hintText: 'Username',
		height:70,
		width:380,
		textAlign:'left',
		borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
		autocapitalization:false,
		font:{fontSize:19, fontFamily:'Helvetica-Bold'}
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
	/*
	txt_Username.addEventListener('focus', function(e) {
	 	win.animate(slideUP);
     });
     
	txt_Password.addEventListener('blur', function(e) {
	 	win.animate(slideDown);
     });
	*/
	var btn_Acknowledge = Ti.UI.createButton({
		title: 'Acknowledge',
		width: 160,
		height:60,
		font:{fontSize:19, fontFamily:'Helvetica-Bold'}
	});
	
	var btn_Cancel = Ti.UI.createButton({
		title: 'Cancel',
		width: 160,
		height:60,
		font:{fontSize:19, fontFamily:'Helvetica-Bold'}
	});
	
	btn_Cancel.addEventListener('click', function(e){
		win.close();
	});

/*
	callbackFunFact = function(text){
		
		var jsonData = XMLToJSON(text);

		if(jsonData !== null){
			//alert(jsonData.FullName);
			root.close(Ti.App.hideSlow);
			var winDo = require('ui/common/DeliveryOrder');
			var deliveryOrder = winDo.DeliveryOrder();
			deliveryOrder.open(Ti.App.showSlow);
		}else
		{
			ShowMessage('Wrong username or password');
			//alert('wrong usname' + text);
		}
	};
	*/	
	var isAcknowledge = true;
	
	callbackError = function(){
		newIndicat.hide();
		//indicator.closeIndicator();
	};
	
	
	
	
	callbackAfterAcknowled = function(_text){
		var doc = Ti.XML.parseString(_text);
		var items = doc.getElementsByTagName('string');
		var _message = items.item(0).text;
		
		newIndicat.hide();
		var alert = Titanium.UI.createAlertDialog({ title: 'Suki', 
			message: _message, 
			buttonNames: ['Ok'], 
			cancel: 0,
			persistent:true });
			
		alert.addEventListener('click', function(e) { 
			
			if (e.cancel === e.index || e.cancel === true) {
				
				if(_message === "Operation complete successful!"){
					isAcknowledge = false;
					//btn_Acknowledge.title = "Close";
					//btn_Cancel.hide();
					_callbackClose();
					win.close();
				}
				if(_message === "Error: Internal error (-2010) occurred"){
					alert.message = "No Internet connection. Pls Try again";
				}
			}
		});
		alert.show();
	};
	
	btn_Acknowledge.addEventListener('click', function(e){
		if(isAcknowledge===true){
			
			newIndicat.show();
			var _username = txt_Username.value;
			var _password = txt_Password.value;
			//var jsonString = JSON.stringify(_batchDataPush);
			getRemoteData(Titanium.App.Properties.getString("locationservice") + "/Mobile.asmx/Acknowledgment", {'DocEntry':_DocEntry, '_username':_username, '_password':_password, '_outlet': Ti.App.currOutletCode,  'DBName': _DBName,  'GRPODate': _GRPODate}, callbackAfterAcknowled, callbackError);
			return;
		}else{
			win.close();
		}
		
	});
	
	var btnView = Ti.UI.createView({
		height:70,
		width:320,
		backgroundColor:'transparent',
		layout:'horizontal'
	});
	

	btnView.add(btn_Cancel);
	btnView.add(btn_Acknowledge);
	
	centerView.add(separateView);
	centerView.add(lbLoginInfo);
	centerView.add(newIndicat);
	centerView.add(txt_Username);
	centerView.add(Ti.UI.createView({height:5}));
	centerView.add(txt_Password);
	centerView.add(Ti.UI.createView({height:5}));
	
	centerView.add(btnView);
	win.add(centerView);
	
	win.addEventListener('open', function(e){
		txt_Username.focus();
	});
/*
	win.setRightNavButton(btn_Acknowledge);
	win.setLeftNavButton(btn_Cancel);
	
	
	var nav = Titanium.UI.iPhone.createNavigationGroup({
    	window:win
	});
	
	var root = Titanium.UI.createWindow({
	});
	
	root.add(nav);
	return root;	*/
	
	return win;
};