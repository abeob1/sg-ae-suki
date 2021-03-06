exports.DeliveryOrder = function(_params, _callbackClose, mainNav){
	
	Ti.include('/ui/common/LayoutControl.js');
	
	var win  = Ti.UI.createWindow({
		//title:'Delivery Order',
		 backgroundColor:'#FFFFFF',
		 layout:'vertical'
		 //navBarHidden:true
	});
	
	var uie = require('/ui/utilities/WinIndicator');
	var indicator = uie.createIndicatorWindow();

	win.add(Ti.UI.createView({
		height: 10,
		backgroundColor:'transparent'
	}));
	
	hdrAll = [2, 74, 126, 188, 240];
	hdrWorking = [2, 82, 154, 0, 226];
	
	widthAll = [70, 50, 60, 50, 80];
	widthWorking = [80, 70, 70, 50, 80];
	
	var detailFont = {fontSize:Ti.App.ROWFONTSIZE, fontFamily:'Helvetica-Bold'};
	
	var alertWindow = Titanium.UI.createAlertDialog({
	    title: 'Suki',
	    message: 'Log out?',
	    cancel:1,
	    buttonNames: ['OK','Cancel']
	});
 
alertWindow.addEventListener('click',function(ev){
  
    if(ev.index === 0)
    {
		win.close(Ti.App.hideSlow);
		loginWin = require('ui/common/LoginScreen');
		var login = loginWin.LoginScreen();
		login.open(Ti.App.showSlow);  
    }
});
	
	
	
	var btn_Menu = Ti.UI.createButton({
		title:'Menu'
	});
	
	var viewTop = Ti.UI.createView({
		height: 90,
		width: 400,
		backgroundColor: 'transparent',
		layout:'horizontal'
		
	});
	
	var perWidthAll = ['17%', '23%', '15%', '16%', '25%'];
	var perWidthWorking = ['22%', '23%', '25%', '0%', '26%'];
	var detailFont = {fontSize:Ti.App.ROWFONTSIZE, fontFamily:'Helvetica-Bold'};
createTableRow = function(_id, _date, _orderNum, _outlet, _status, _addCode, _PONo, _driver, _acNo, _dbName, isAll, _outletCode, _docdate){
		
		var row = Ti.UI.createTableViewRow({
			DocEntry: _id,
			DBName: _dbName,
			hasChild: true,
			height: Ti.App.ROWHEIGHT,
			layout:'horizontal',
			DocDate: _docdate
		});
		
		var viewRow = Ti.UI.createView({
			height: Ti.App.ROWHEIGHT,
			layout:'horizontal',
			width:Ti.App.SCREEN_WIDTH
		});
		
		var space = Ti.UI.createView({
			width:5,
			backgroundColor:'transparent'
		});
		
		var lbDate = Ti.UI.createLabel({
			text: "  " + _date,
			height: Ti.App.ROWHEIGHT,
			left: 8,
			width: isAll===true?perWidthAll[0]:perWidthWorking[0],
			font: detailFont,
			textAlign:'left'
		});
		
		var lbOrderNum= Ti.UI.createLabel({
			text: _orderNum,
			height: Ti.App.ROWHEIGHT,
			width: isAll===true?perWidthAll[1]:perWidthWorking[1],
			font: detailFont,
			textAlign:'left'
		});
		
		var lbOutlet = Ti.UI.createLabel({
			text: _outlet,
			height: Ti.App.ROWHEIGHT,
			width: isAll===true?perWidthAll[2]:perWidthWorking[2],
			font: detailFont,
			textAlign:'left'
		});
		
		var _lbStatus = Ti.UI.createLabel({
			text: _status,
			height: Ti.App.ROWHEIGHT,
			width: isAll===true?perWidthAll[3]:perWidthWorking[3],
			font: detailFont,
			textAlign:'left'
		});
		
		var lbAddCode = Ti.UI.createLabel({
			text: _addCode,
			height: Ti.App.ROWHEIGHT,
			width: isAll===true?perWidthAll[4]:perWidthWorking[4],
			font: detailFont,
			textAlign:'left'
		});
		
		viewRow.add(space);
		viewRow.add(lbDate);
		viewRow.add(lbOrderNum);
		viewRow.add(lbOutlet);
		viewRow.add(_lbStatus);
		viewRow.add(lbAddCode);
		
		row.add(viewRow);
		
		
		
		row.addEventListener('click', function(e){
			//root.hideNavBar();
			var selectedDOID = e.rowData.DocEntry;
			var WDO = require('ui/common/DODetail');
			//_id, _date, _orderNum, _outlet, _status, _addCode, _PONo, _driver, _acNo, isAll
			var transParams = {DocEntry: selectedDOID, DocNum: _orderNum, 
								DocDueDate: _date, CardCode: _outlet, Descr: _status, Address2:_addCode, 
								NumAtCard: _PONo, U_Driver: _driver, ACNo: _acNo, DBName: _dbName, OutletCode: _outletCode, DocDate: _docdate};
			if(isAll === true){					
				var winDetail = WDO.DODetail(transParams, callbackDoNothing);
				mainNav.open(winDetail);
			}
			else{
				var winDetail = WDO.DODetail(transParams, layoutWorkingOrders);
				mainNav.open(winDetail);
			}
		});
		
		return row;
	};
	
	callbackDoNothing = function(){
		
	};
	
	//var btn_WorkingOrder = createButton('Working Orders', (322 - 260)/2, 5, 130, 40);
	var btn_WorkingOrder = createButton('Working Orders', 'NONE', 'NONE', 200, Ti.App.BUTTONHEIGHT);
	btn_WorkingOrder.addEventListener('click', function(e){
		layoutWorkingOrders();
	});
	
	var btn_AllOrder =  createButton('All Orders', 'NONE', 'NONE', 200, Ti.App.BUTTONHEIGHT);
	btn_AllOrder.addEventListener('click', function(e){
		layoutAllOrders();
	});
	
	viewTop.add(btn_WorkingOrder);
	viewTop.add(btn_AllOrder);
	
	win.add(viewTop);
	//win.add(Ti.UI.createLabel({text:'Order List'}));
	
	var viewHeader = Ti.UI.createView({
		height:Ti.App.BUTTONHEIGHT,
		//backgroundColor:'#660000',
		layout:'horizontal',
		backgroundGradient: {
		      type: 'linear',
		      colors: ['#660000','#8C3131'],
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y:Ti.App.BUTTONHEIGHT},
		      backFillStart:false}
	});
	
	
	var lbExpDel = createLabelBold('Expected \nDelivery Date', 'NONE', 'NONE', perWidthWorking[0], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbOrder = createLabelBold('DO No', 'NONE', 'NONE', perWidthWorking[1], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbOutlet = createLabelBold('Outlet', 'NONE', 'NONE', perWidthWorking[2], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbStatus = createLabelBold('Status', 'NONE', 'NONE', perWidthWorking[3], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbAddCode = createLabelBold('Address', 'NONE', 'NONE', perWidthWorking[4], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	
	viewHeader.add(lbExpDel);
	viewHeader.add(lbOrder);
	viewHeader.add(lbOutlet);
	viewHeader.add(lbStatus);
	viewHeader.add(lbAddCode);
	
	layoutWorkingOrders = function(){
		btn_AllOrder.backgroundGradient = {type: 'linear',
		      colors: ['#333333','#999999'],//8C3131
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y: Ti.App.BUTTONHEIGHT},
		      backFillStart:false};
		      
		btn_WorkingOrder.backgroundGradient = {type: 'linear',
		      colors: ['#4D0000','#D65454'],//8C3131
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y: Ti.App.BUTTONHEIGHT},
		      backFillStart:false};
		
		//root.showNavBar();
		indicator.openIndicator();
		lbExpDel.width = perWidthWorking[0];
		lbOrder.width = perWidthWorking[1];
		lbOutlet.width = perWidthWorking[2];
		lbStatus.width = perWidthWorking[3];
		lbAddCode.width = perWidthWorking[4];
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetWorkingOrder', {'numRecord' : 1000, 'outletCode':_params.OutletCode, 'driverNo': Ti.App.currUserCode}, callbackWorkingOrder, callbackError);
	};
	
	layoutAllOrders = function(){
		btn_AllOrder.backgroundGradient = {type: 'linear',
				colors: ['#4D0000','#D65454'],//8C3131
		      
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y: Ti.App.BUTTONHEIGHT},
		      backFillStart:false};
		      
		btn_WorkingOrder.backgroundGradient = {type: 'linear',
		      colors: ['#333333','#999999'],//8C3131
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y: Ti.App.BUTTONHEIGHT},
		      backFillStart:false};
		      
		//root.showNavBar();
		indicator.openIndicator();
		lbExpDel.width = perWidthAll[0];
		lbOrder.width = perWidthAll[1];
		lbOutlet.width = perWidthAll[2];
		lbStatus.width = perWidthAll[3];
		lbAddCode.width = perWidthAll[4];
	
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetAllOrder', {'outletCode':_params.OutletCode, 'driverNo': Ti.App.currUserCode}, callbackAllOrder, callbackError);
	};
	
	win.add(viewHeader);
	
	var table = Ti.UI.createTableView({
			
		});
	win.add(table);
	
	formatShortDate = function(jsonDate){
		var date = jsonDate;
		
		var dateCvt = new Date(parseInt(date.replace('/Date(', '')));
		var datePlus = dateCvt.getDate();
		var month = dateCvt.getMonth() + 1;// month from 0
		return  datePlus + "/" + month + "/" + dateCvt.getFullYear();
	};
	
	callbackError = function(){
		indicator.closeIndicator();
	};
	
	callbackWorkingOrder = function(_jsonText){
		dataTableWorking = [];
		
		var jsonData = XMLToJSON(_jsonText);
		if(jsonData !== null){
			var i = 0;
			for(i = 0; i< jsonData.length; i++){
				dataTableWorking.push(createTableRow(jsonData[i].DocEntry, formatShortDate(jsonData[i].DocDate), jsonData[i].DocNum, jsonData[i].CardCode, jsonData[i].Descr, jsonData[i].Address2, jsonData[i].NumAtCard, jsonData[i].U_AB_DriverNo, jsonData[i].ACNo, jsonData[i].U_DBName, false, jsonData[i].OutletCode, formatShortDate(jsonData[i].DocDate)));
			}
		}
		table.setData(dataTableWorking);
		indicator.closeIndicator();
	};
	
	callbackAllOrder = function(_jsonText){
		dataTableAll = [];
		
		var jsonData = XMLToJSON(_jsonText);
		if(jsonData !== null){
			var i = 0;
			for(i = 0; i< jsonData.length; i++){
				dataTableAll.push(createTableRow(jsonData[i].DocEntry, formatShortDate(jsonData[i].DocDate), jsonData[i].DocNum, jsonData[i].CardCode, jsonData[i].Descr, jsonData[i].Address2, jsonData[i].NumAtCard, jsonData[i].U_AB_DriverNo, jsonData[i].ACNo, jsonData[i].U_DBName, true, jsonData[i].OutletCode, formatShortDate(jsonData[i].DocDate)));
			}
		}
		table.setData(dataTableAll);
		indicator.closeIndicator();
	};
	
	/*
	var nav = Titanium.UI.iPhone.createNavigationGroup({
    	window:win
    	
	});
	var root = Titanium.UI.createWindow({
		title:'Delivery Orders'
	});
	root.add(nav);
	root.addEventListener('close', function()
	{
			_callbackClose();
	});
	*/
	win.addEventListener('close', function()
	{
			_callbackClose();
	});
	layoutWorkingOrders();
	return win;	
};