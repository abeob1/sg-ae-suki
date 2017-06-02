exports.Settings = function(){
	
	Ti.include('/ui/utilities/LocalDB.js');
	var win  = Ti.UI.createWindow({
		 backgroundColor:'#FFFFFF',
		 title:'Settings',
		 layout: 'vertical'
	});
	
	
	var txt_Service = Ti.UI.createTextField({
		width: Ti.App.SCREEN_WIDTH - 20,
		borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
		autocapitalization:false
	});
	
	txt_Service.addEventListener('blur', function(e){
		if(svcPath === '')
			insertSetting('wsPath', txt_Service.value);
		else
			updateSetting('wsPath', txt_Service.value);
	});
	
	/*
	var txt_dbName = Ti.UI.createTextField({
		width: Ti.App.SCREEN_WIDTH - 20,
		borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
		autocapitalization:false
	});
	txt_dbName.addEventListener('blur', function(e){
		if(dbName === '')
			insertSetting('dbName', txt_dbName.value);
		else
			updateSetting('dbName', txt_dbName.value);
	});
	*/
	win.add(Ti.UI.createView({height:20}));
	win.add(Ti.UI.createLabel({text: 'Service location: ', width: Ti.App.SCREEN_WIDTH - 20, textAlign:'left'}));
	win.add(txt_Service);
	//win.add(Ti.UI.createLabel({text: 'HQ Database Name: ', width: Ti.App.SCREEN_WIDTH - 20, textAlign:'left'}));
	//win.add(txt_dbName);

	
	var svcPath = selectSetting('wsPath');
	//var dbName =  selectSetting('dbName');

	txt_Service.value = svcPath;
	//txt_dbName.value = dbName;

	var nav = Titanium.UI.iPhone.createNavigationGroup({
    	window:win
	});
	
	var root = Titanium.UI.createWindow({
		//modal:true,
		//title:'Settings'
	});
	var btn_Save = Ti.UI.createButton({
		title:'Close'
	});
	btn_Save.addEventListener('click', function(e){
		root.close();
	});
	win.setRightNavButton(btn_Save);
	root.add(nav);
	return root;
	
};