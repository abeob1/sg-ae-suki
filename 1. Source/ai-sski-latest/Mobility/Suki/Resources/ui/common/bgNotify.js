
/*
var notification =    Ti.App.iOS.scheduleLocalNotification({
  alertBody:"Suki background",
  alertAction:"Re-Launch!",
  userInfo:{"hello":"world"},
  sound:"pop.caf",
  date:new Date(new Date().getTime() + 10000) // 3 seconds after backgrounding
});
*/
//alert('Background Notify');
Ti.include('/ui/utilities/LocalDB.js');

DataRemote = function(_callbackRet){
	if(Titanium.Network.networkType != Titanium.Network.NETWORK_NONE){ 
		
		var cre = getCredential();
		if(cre != ''){
			var drvNo = cre.Username;
			var loader = Titanium.Network.createHTTPClient();
			loader.setRequestHeader("enctype", "multipart/form-data");
			loader.setRequestHeader("Content-Type", "application/json; charset=utf-8");
			loader.onload = function() {
				_callbackRet(this.responseText);
			};
			
			loader.onerror = function(){
		
			};
			loader.open("GET", Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetNumberOpenDO');
			loader.send({'driverNo' : drvNo});
		}
	}
};

callbackRet = function(_text){
	var doc = Ti.XML.parseString(_text);
	var items = doc.getElementsByTagName('int');
	if(items.length>0){
		var numDO = +items.item(0).text;
		if(numDO === 0){
			Titanium.UI.iPhone.appBadge = null;
		}else
		{
			Titanium.UI.iPhone.appBadge = numDO;	
		}
	}
};

getNumberNotify = function(){
	DataRemote(callbackRet);	
};

var reloadInt = +Titanium.App.Properties.getString("reloadinterval");
var milisec = reloadInt * 1000 * 60;
var timer  = setInterval(getNumberNotify, milisec);

