exports.AllOrders = function(mainNav){
	
	Ti.include('/ui/common/LayoutControl.js');
	
	var win  = Ti.UI.createWindow({
		title:'Completed Orders Within 7 Days',
		 backgroundColor:'#FFFFFF',
		 layout:'vertical'//,
		 //navBarHidden:true
	});
	
	var uie = require('/ui/utilities/WinIndicator');
	var indicator = uie.createIndicatorWindow();

	win.add(Ti.UI.createView({
		height: 10,
		backgroundColor:'transparent'
	}));
	
	var alertWindow = Titanium.UI.createAlertDialog({
	    title: 'Suki',
	    message: 'Log out?',
	    cancel:1,
	    buttonNames: ['OK','Cancel']
	});
 
alertWindow.addEventListener('click',function(ev){
  
    if(ev.index === 0)
    {
		//root.close(Ti.App.hideSlow);
		win.close(Ti.App.hideSlow);
		loginWin = require('ui/common/LoginScreen');
		var login = loginWin.LoginScreen();
		login.open(Ti.App.showSlow);  
    }
});
	
	
	
	var viewTop = Ti.UI.createView({
		height: Ti.App.BUTTONHEIGHT ,
		backgroundColor: 'transparent',
		layout:'vertical',
		width:260
	});
	
	var perWidthAll = ['17%', '23%', '15%', '16%', '25%'];
	var detailFont = {fontSize:Ti.App.ROWFONTSIZE, fontFamily:'Helvetica-Bold'};
createTableRow = function(_id, _date, _orderNum, _outlet, _status, _addCode, _PONo, _driver, _acNo, _dbName, isAll, _outletCode, _docdate){
		
		var row = Ti.UI.createTableViewRow({
			DocEntry: _id,
			DBName: _dbName,
			hasChild: true,
			height: Ti.App.ROWHEIGHT,
			layout:'horizontal',
			borderColor:'#000000'
		});
		
		var viewRow = Ti.UI.createView({
			height: Ti.App.ROWHEIGHT,
			layout:'horizontal',
			width:Ti.App.SCREEN_WIDTH - 10
		});
		
		var space = Ti.UI.createView({
			width:5,
			backgroundColor:'transparent'
		});
		
		var lbDate = Ti.UI.createLabel({
			text: _date,
			height: Ti.App.ROWHEIGHT,
			left: 8,
			width: perWidthAll[0],
			font:detailFont,
			textAlign:'left'
		});
		
		var lbOrderNum= Ti.UI.createLabel({
			text: _orderNum,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[1],
			font:detailFont,
			textAlign:'left'
		});
		
		var lbOutlet = Ti.UI.createLabel({
			text: _outlet,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[2],
			font:detailFont,
			textAlign:'left'
		});
		
		var _lbStatus = Ti.UI.createLabel({
			text: _status,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[3],
			font:detailFont,
			textAlign:'left'
		});
		
		var lbAddCode = Ti.UI.createLabel({
			text: _addCode,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[4],
			font:detailFont,
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
								NumAtCard: _PONo, U_Driver: _driver, ACNo: _acNo, DBName: _dbName, OutletCode:_outletCode, DocDate: _docdate};
			var winDetail = WDO.DODetail(transParams, callBackShowNavBar);
			mainNav.open(winDetail);
		});
		
		return row;
	};
	
	callBackShowNavBar = function(){
		//root.showNavBar();
	};
	
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
	
	
	var lbExpDel = createLabelBold('Expected \nDelivery Date', 'NONE', 'NONE', perWidthAll[0], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbOrder = createLabelBold('DO No', 'NONE', 'NONE', perWidthAll[1], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbOutlet = createLabelBold('Outlet', 'NONE', 'NONE', perWidthAll[2], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbStatus = createLabelBold('Status', 'NONE', 'NONE', perWidthAll[3], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbAddCode = createLabelBold('Address', 'NONE', 'NONE', perWidthAll[4], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	
	viewHeader.add(lbExpDel);
	viewHeader.add(lbOrder);
	viewHeader.add(lbOutlet);
	viewHeader.add(lbStatus);
	viewHeader.add(lbAddCode);
	
	layoutAllOrders = function(){
		//root.showNavBar();
		indicator.openIndicator();
		lbExpDel.width = perWidthAll[0];
		lbOrder.width = perWidthAll[1];
		lbOutlet.width = perWidthAll[2];
		lbStatus.width = perWidthAll[3];
		lbAddCode.width = perWidthAll[4];
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetCompletedOrder', {'driverNo': Ti.App.currUserCode}, callbackAllOrder, callbackError);
	};
	
	win.add(viewHeader);
	
	var table = Ti.UI.createTableView({
			
		});
	win.add(table);
	
	formatShortDate = function(jsonDate){
		var date = jsonDate;
		var dateCvt = new Date(parseInt(date.replace('/Date(', '')));
		var month = dateCvt.getMonth() + 1;// month from 0
		return dateCvt.getDate() + "/" + month + "/" + dateCvt.getFullYear();
	};
	
	callbackError = function(){
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
		title:'Completed Orders Within 7 Days'
	});
	
	root.add(nav);
	*/
	layoutAllOrders();
	return win;	
};