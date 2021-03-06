exports.SummaryOrder = function(){
	
	Ti.include('/ui/common/LayoutControl.js');
	
	var win  = Ti.UI.createWindow({
		title:'Summary Order List',
		 backgroundColor:'#FFFFFF',
		 layout:'vertical'
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
		root.close(Ti.App.hideSlow);
		loginWin = require('ui/common/LoginScreen');
		var login = loginWin.LoginScreen();
		login.open(Ti.App.showSlow);  
    }
});
	
	var btn_Logout = Ti.UI.createButton({
		title: 'Logout',
		width:'100'
	});
	
	btn_Logout.addEventListener('click', function(e){
		alertWindow.show();
	});
	
	win.setRightNavButton(btn_Logout);
	
	var viewTop = Ti.UI.createView({
		height: 90,
		width: 400,
		backgroundColor: 'transparent',
		layout:'horizontal'
	});
	
	var perWidthAll = ['10%', '20%', '30%', '15%', '22%'];
	
	var detailFont = {fontSize:Ti.App.ROWFONTSIZE, fontFamily:'Helvetica-Bold'};
	
createTableRowSumary = function(_RowNum, _OutletCode, _Outlet, _TotalOpen, _Address){
		
		var row = Ti.UI.createTableViewRow({
			OutletCode: _OutletCode,
			hasChild: true,
			height: Ti.App.ROWHEIGHT,
			layout:'horizontal'
		});
		
		var viewRow = Ti.UI.createView({
			height: Ti.App.ROWHEIGHT,
			layout:'horizontal',
			width:Ti.App.SCREEN_WIDTH
		});
		
		var space = Ti.UI.createView({
			width:15,
			backgroundColor:'transparent'
		});
		
		var lbRowNum = Ti.UI.createLabel({
			text: ' ' + _RowNum,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[0],
			font: detailFont,
			textAlign:'left'
		});
		
		var lbOutletCode = Ti.UI.createLabel({
			text: _OutletCode,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[1],
			font: detailFont,
			textAlign:'left'
		});
		
		var lbOutlet = Ti.UI.createLabel({
			text: _Outlet,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[2],
			font: detailFont,
			textAlign:'left'
		});
		
		var lbTotalOpen = Ti.UI.createLabel({
			text: _TotalOpen,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[3],
			font: detailFont,
			textAlign:'center'
		});
		
		var lbAddress = Ti.UI.createLabel({
			text: _Address,
			height: Ti.App.ROWHEIGHT,
			width: perWidthAll[4],
			font:detailFont,
			textAlign:'left'
		});
		
		viewRow.add(space);
		viewRow.add(lbRowNum);
		viewRow.add(lbOutletCode);
		viewRow.add(lbOutlet);
		viewRow.add(lbTotalOpen);
		viewRow.add(lbAddress);
		
		row.add(viewRow);
		
		row.addEventListener('click', function(e){
			var selectedOutletCode = e.rowData.OutletCode;
			Ti.App.currOutletCode = selectedOutletCode;
			var WDO = require('ui/common/DeliveryOrder');
			var transParams = {OutletCode: selectedOutletCode};
			var winDetail = WDO.DeliveryOrder(transParams, getSummaryOrder, nav);
			nav.open(winDetail);
		});
		
		return row;
	};

	//var btn_Refresh = createButton('Refresh', 'NONE', 'NONE', 130, 40);
	
	var btn_Refresh = createButton('Refresh', 'NONE', 'NONE', 200, Ti.App.BUTTONHEIGHT);

	var btn_AllOrder = createButton('All Orders', 'NONE', 'NONE', 200, Ti.App.BUTTONHEIGHT);

	btn_Refresh.addEventListener('click', function(e){
		getSummaryOrder();		
	});
	
	
	btn_AllOrder.addEventListener('click', function(e){
			
			var WDO = require('ui/common/AllOrders');
			var winAllOrder = WDO.AllOrders(nav);
			nav.open(winAllOrder);	
	});
	
	viewTop.add(btn_Refresh);
	viewTop.add(btn_AllOrder);
	
	win.add(viewTop);

	var viewHeader = Ti.UI.createView({
		height: Ti.App.BUTTONHEIGHT,
		width: Ti.App.SCREEN_WIDTH,
		//backgroundColor:'#660000',
		layout:'horizontal',
		backgroundGradient: {
		      type: 'linear',
		      colors: ['#660000','#8C3131'],
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y:Ti.App.BUTTONHEIGHT},
		      backFillStart:false}
	});
	
	
	var lbNo = createLabelBold(' No', 'NONE', 'NONE', perWidthAll[0], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbOutletCode = createLabelBold('Outlet Code', 'NONE', 'NONE', perWidthAll[1], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbOutlet = createLabelBold('Outlet Name', 'NONE', 'NONE', perWidthAll[2], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbTotal = createLabelBold('Total \nOpen DO', 'NONE', 'NONE', perWidthAll[3], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	var lbAddress = createLabelBold('Address', 'NONE', 'NONE', perWidthAll[4], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left');
	
	viewHeader.add(lbNo);
	viewHeader.add(lbOutletCode);
	viewHeader.add(lbOutlet);
	viewHeader.add(lbTotal);
	viewHeader.add(lbAddress);
	
	getSummaryOrder = function(){
		
		indicator.openIndicator();
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetSummaryOrder', {'driverNo' : Ti.App.currUserCode}, callbackSummaryOrder, callbackError);
	};
	
	win.add(viewHeader);
	
	var table = Ti.UI.createTableView({
			
	});
	win.add(table);

	callbackError = function(){
		indicator.closeIndicator();
	};
	
	//var dataTableSummary = [];
	
	callbackSummaryOrder = function(_jsonText){
		var dataTableSummary = [];
		var totalDO = 0;
		if(_jsonText !== '[]'){
			var jsonData = XMLToJSON(_jsonText);
			if(jsonData !== null){
				var i = 0;
				
				for(i = 0; i< jsonData.length; i++){
					totalDO += +jsonData[i].OpenDOQty;
					dataTableSummary.push(createTableRowSumary(i + 1, jsonData[i].Code, jsonData[i].Name, jsonData[i].OpenDOQty, jsonData[i].Address));
				}
			}
			table.setData(dataTableSummary);
			if(totalDO === 0)
				Titanium.UI.iPhone.appBadge = null;
			else
				Titanium.UI.iPhone.appBadge = totalDO;
		}
		indicator.closeIndicator();
	};
	//_RowNum, _OutletCode, _Outlet, _TotalOpen, _Address

	
	var nav = Titanium.UI.iPhone.createNavigationGroup({
    	window:win
	});
	var root = Titanium.UI.createWindow({
		opacity:0
	});
	
	root.add(nav);
	getSummaryOrder();
	return root;	
};