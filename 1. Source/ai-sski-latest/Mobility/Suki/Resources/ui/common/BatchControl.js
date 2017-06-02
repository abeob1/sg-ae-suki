exports.BatchControl = function(_header, _callbackReturn){
	Ti.include('/ui/common/LayoutControl.js');
	var isEditing = false;

	var win  = Ti.UI.createWindow({
		 backgroundColor:'#FFFFFF',
		 title:'Batch'
	});
	
	var uie = require('/ui/utilities/WinIndicator');
	var indicator = uie.createIndicatorWindow();
	
	var btn_Add = Ti.UI.createButton({
		systemButton: Ti.UI.iPhone.SystemButton.ADD
	});
	
	var topTable = Ti.UI.createTableView({
		top:10,
		left:10,
		width: Ti.App.SCREEN_WIDTH - 20,
		height: 80,
		borderWidth:2,
		borderColor:'#bbb',
		borderRadius:7,
		scrollable:false
	});
	
	
	var leftCell = [2, 64, 156, 218 ];
	var widthCell = Ti.App.SCREEN_WIDTH * 0.23;
	var widthContain = Ti.App.SCREEN_WIDTH * 0.7;
	var row1 = Ti.UI.createTableViewRow({
		height: 40,
		layout: 'horizontal'
	});

	var lbLineNum = createLabel(_header.LineNum, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	var lbItemCode = createLabel(_header.ItemCode, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	
	var lbDscription = createLabel(_header.Dscription, 'NONE', 'NONE', widthContain, 40, '#660000', 'left');
	
	row1.add(createLabelBold(' Line Num. ', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row1.add(lbLineNum);
	row1.add(createLabelBold(' Item Code', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row1.add(lbItemCode);
	
	
	var row2 = Ti.UI.createTableViewRow({
		height: 40,
		layout: 'horizontal'
	});
	row2.add(createLabelBold(' Item Name', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row2.add(lbDscription);
	
	var rowData= [];
	rowData.push(row1);
	rowData.push(row2);
	topTable.data = rowData;
	
		
	
	var leftHeader = [2, 4];
	
	var viewHeader = Ti.UI.createView({
		top: 80 + 15,
		left: 0,
		height: 50,
		//backgroundColor:'#660000',
		layout:'horizontal',
		backgroundGradient: {
		      type: 'linear',
		      colors: ['#660000','#8C3131'],
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y:50},
		      backFillStart:false}
	});
	
	var perWidthHdr = ['45%', '30%', '20%'];
	
	//createLabelBold = function(_text, _left, _top, _width, _height, _color, _align)

	viewHeader.add(createLabelBold('  Batch No', 'NONE', 'NONE', perWidthHdr[0], 40, '#FFFFFF', 'left'));
	viewHeader.add(createLabelBold(' Qty', 'NONE', 'NONE', perWidthHdr[1], 40, '#FFFFFF', 'center'));


	var tableHeightFull = Ti.App.SCREEN_HEIGHT - 80 - 40 - 72;
	
	var detailData = [];
	var viewContain = Ti.UI.createTableView({
		top: 80 + 50 + 10,
		
		height: tableHeightFull,
		borderWidth:2,
		borderColor:'#660000',
		data:detailData,
		editable:true
	});
	
	///////////
	var slideUP = Titanium.UI.createAnimation({
		top: -250,
		duration: 300	
	});
	
	var slideDown = Titanium.UI.createAnimation({
		top: 0,
		duration: 300
	});
	
createTextField = function(_text, _left, _width, _align){
	
		var send = Titanium.UI.createButton({
		    title : 'Done',
		    style : Titanium.UI.iPhone.SystemButtonStyle.DONE,
		});
		
		send.addEventListener('click', function(e) {
	      	e.source.activeFld.blur();
	      	root.animate(slideDown);
	    });
		
		var cancel = Titanium.UI.createButton({
		    systemButton : Titanium.UI.iPhone.SystemButton.CANCEL
		});
		
		var flexSpace = Titanium.UI.createButton({
		    systemButton : Titanium.UI.iPhone.SystemButton.FLEXIBLE_SPACE
		});
	
		var textField =  Ti.UI.createTextField({
			value: _text,
			left:_left,
			width:_width,
			height: Ti.App.ROWHEIGHT,
			textAlign:_align,
			font:{fontSize:12},
			borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
			keyboardType: Ti.UI.KEYBOARD_NUMBER_PAD,
			keyboardToolbar:[flexSpace, send]
		});
		 textField.addEventListener('blur', function(e) {
		 	root.animate(slideDown);
	     });
	     
		 textField.addEventListener('focus', function(e) {
		 	root.animate(slideUP);
	      	send.activeFld = textField;
	     });
	      
      return textField;
};
createSeparateRow = function(_width){
	return Ti.UI.createView({
		backgroundColor:'transparent',
		height:20,
		width:_width
	});
};

createDetailRow = function(_rid, _batchNo, _qty){
	var row = Ti.UI.createTableViewRow({
		height:Ti.App.ROWHEIGHT,
		layout:'horizontal',
	});
	
	row.rowid = _rid;
	
	var viewRow = Ti.UI.createView({
		width:Ti.App.SCREEN_WIDTH,
		height: Ti.App.ROW_HEIGHT,
		layout: 'horizontal'
	});
	
	var txt_BatchNo =  Ti.UI.createTextField({
		value: _batchNo,
		width:perWidthHdr[0],
		height: Ti.App.ROWHEIGHT - 3,
		textAlign:'left',
		font:{fontSize:Ti.App.ROWFONTSIZE},
		borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED
		
	});
	
	var txt_Qty = Ti.UI.createTextField({
		value: _qty,
		width:perWidthHdr[1],
		height: Ti.App.ROWHEIGHT - 3,
		textAlign:'right',
		font:{fontSize:Ti.App.ROWFONTSIZE},
		borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
		keyboardType: Ti.UI.KEYBOARD_NUMBER_PAD
	});
	
	//var btn_Delete = createButton('Delete', 'NONE', 'NONE', 'NONE', Ti.App.ROWHEIGHT);
	

	var btn_Delete = Ti.UI.createButton({
		height: Ti.App.ROWHEIGHT - 4,
		title: 'Delele',
		width: perWidthHdr[2],
		backgroundImage: 'NONE',
		selectedColor:'#DE0000',
		borderRadius:5,
		font:{fontSize:Ti.App.ROWFONTSIZE,fontFamily:'Helvetica-Bold'},
		backgroundGradient: {
		      type: 'linear',
		      colors: ['#660000','#8C3131'],
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y:Ti.App.ROWHEIGHT},
		      backFillStart:false}
	});
	
	btn_Delete.addEventListener('click', function(e){
		for(var i = 0; i< detailData.length; i++){
			if(detailData[i].rowid === _rid){
				detailData.splice(i, 1);
				viewContain.deleteRow(i);
				break;
			}
		}
	});
	
	viewRow.add(createSeparateRow(4));
	viewRow.add(txt_BatchNo);
	viewRow.add(createSeparateRow(4));
	viewRow.add(txt_Qty);
	viewRow.add(createSeparateRow(4));
	viewRow.add(btn_Delete);
	
	row.add(viewRow);
	return row;
};
	
	
	viewContain.addEventListener('', function(e){
		alert(e.source.index);
	});
	
	callbackError = function(){
		indicator.closeIndicator();
	};
	
	callbackDeliveryDetail = function(_jsonText){
		/*
		var detailData = [];
		var detailDataEditMode = [];
		
		var jsonData = XMLToJSON(_jsonText);
		if(jsonData !== null){
			var i = 0;
			WhseCode = jsonData[i].WhsCode;
			for(i = 0; i< jsonData.length; i++){
				
				detailData.push(createDetailRow(' ' +  jsonData[i].LineNum, jsonData[i].ItemCode, jsonData[i].Dscription, jsonData[i].Quantity, jsonData[i].U_RecQty, jsonData[i].unitMsr, false));
				detailDataEditMode.push(createDetailRow(' ' + jsonData[i].LineNum, jsonData[i].ItemCode, jsonData[i].Dscription, jsonData[i].Quantity, jsonData[i].U_RecQty, jsonData[i].unitMsr, true));
			}
		}
		viewContain.setData(detailData);
		viewContainEditMode.setData(detailDataEditMode);
		indicator.closeIndicator();
		*/
	};
	
	//getRemoteData(selectSetting('wsPath') + '/Mobile.asmx/GetOrderDetail', {'DocEntry' : _header.DocEntry, 'currSisCode':Ti.App.currSisCode}, callbackDeliveryDetail, callbackError);
	
	btn_Add.addEventListener('click', function(e){
		//viewContain.data.push(createDetailRow(rid, '', ''));
		
	});
	
	
	var btn_Close = Ti.UI.createButton({
		title:'Close'
	});
	
	btn_Close.addEventListener('click', function(e){
		root.close();
	});
	
	var btn_Save = Ti.UI.createButton({
		title:'Save'
	});
	
	var btnBar = Ti.UI.createButtonBar({
		labels:['   Save   ', '  Add  ']
	});
	
	var rid = 0;
	btnBar.addEventListener('click', function(e){
		switch(e.index){
			case 0://save
				getBatchData();
				break;
			case 1://add new row
				detailData.push(createDetailRow(rid, '', ''));
				viewContain.setData(detailData);
				rid += 1;
				break;
		}
	});
	
	getBatchData = function(){
		var batchData = [];
		var accQty = 0;
		for(var i = 0; i< detailData.length; i++){
			var bNum = detailData[i].children[0].children[1].value;
			var bQty = detailData[i].children[0].children[3].value;
			batchData.push({LineNum: _header.LineNum, BatchNum: bNum, BatchQty: bQty});
			accQty += +bQty;
		}
		if(accQty !== _header.TotalQty)
			ShowMessage('Total batch qty must equal item qty (' + _header.TotalQty + ')');
		else{	
			root.close();
			_callbackReturn(batchData);
		}
	};
	
	win.add(viewHeader);
	win.add(topTable);
	win.add(viewContain);
	//win.setRightNavButton(btn_Add);
	win.setRightNavButton(btnBar);
	win.setLeftNavButton(btn_Close);
	
	for(var i = 0; i< _header.AvailableBatch.length; i++){
		detailData.push(createDetailRow(rid, _header.AvailableBatch[i].BatchNum, _header.AvailableBatch[i].BatchQty));
		rid++;
	}
	if(detailData.length>0)
		viewContain.setData(detailData);
	var nav = Titanium.UI.iPhone.createNavigationGroup({
    	window:win
	});
	
	var root = Titanium.UI.createWindow({
	});
	
	root.add(nav);
	
	return root;	
};