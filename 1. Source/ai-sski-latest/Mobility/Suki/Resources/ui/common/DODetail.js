exports.DODetail = function(_header, _callbackReloadData){
	Ti.include('/ui/common/LayoutControl.js');
	var isEditing = false;

	var win  = Ti.UI.createWindow({
		 backgroundColor:'#FFFFFF',
			navBarHidden:true
	});
	
	var uie = require('/ui/utilities/WinIndicator');
	var indicator = uie.createIndicatorWindow();
	
	var btn_Edit = Ti.UI.createButton({
		title:'Edit'
	});
	
	var topTable = Ti.UI.createTableView({
		top:10,
		left:10,
		width: Ti.App.SCREEN_WIDTH - 20,
		height: 200,
		borderWidth:2,
		borderColor:'#bbb',
		borderRadius:7,
		scrollable:false
	});
	
	
	var leftCell = [2, 64, 156, 218 ];
	var widthCell = Ti.App.SCREEN_WIDTH * 0.23;
	var row1 = Ti.UI.createTableViewRow({
		height: 50,
		layout: 'horizontal'
	});

	var lbDONum = createLabel(_header.DocNum, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	var lbDODate = createLabel(_header.DocDueDate, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	
	var dateValue = new Date();// current date
	var parts = _header.DocDueDate.split('/');
	var deliDate = new Date(parts[2], parts[1]-1, parts[0]);
	
	var txtDODate = Ti.UI.createTextField({
		height: 35,
		color: '#660000',
		width: widthCell,
		borderStyle:Titanium.UI.INPUT_BORDERSTYLE_ROUNDED,
		font:{fontSize:Ti.App.ROWFONTSIZE, fontFamily:'Helvetica-Bold'},
		editable:false,
		value: (_header.Descr == 'Completed'?_header.DocDate:'')//_header.DocDate //dateValue.getDate() + "/" + (dateValue.getMonth()+1) + "/" + dateValue.getFullYear() 
	
	});
	
	
	//DO Date zone
	var picker_view = Titanium.UI.createView({
		height:751,
		bottom:-751,
		layout:'vertical',
		width:300
	});

	var cancel =  Titanium.UI.createButton({
		title:'Cancel',
		style:Titanium.UI.iPhone.SystemButtonStyle.BORDERED
	});

	var done =  Titanium.UI.createButton({
		title:'Done',
		style:Titanium.UI.iPhone.SystemButtonStyle.DONE
	});
	
	var spacer =  Titanium.UI.createButton({
		systemButton:Titanium.UI.iPhone.SystemButton.FLEXIBLE_SPACE
	});

	var toolbar =  Titanium.UI.createToolbar({
		top:0,
		width:300,
		items:[cancel,spacer,done]
	});
	
	var picker = Ti.UI.createPicker({
		type:Ti.UI.PICKER_TYPE_DATE,
		minDate:deliDate,
		maxDate:dateValue,
		value:dateValue,
		selectionIndicator:true // turn on the selection indicator (off by default)
	});
	
	picker_view.add(toolbar);
	picker_view.add(picker);
	

	var slide_in =  Titanium.UI.createAnimation({bottom:0});
	var slide_out =  Titanium.UI.createAnimation({bottom:-751});


//CANCEL BUTTON
	cancel.addEventListener('click', function() {
		picker_view.animate(slide_out);
	});
//UPDATE VALUE IF CHANGED
	picker.addEventListener('change', function() {
		var rDate = picker.value;
		var month = rDate.getMonth() + 1;
		txtDODate.value = rDate.getDate() + "/" + month + "/" + rDate.getFullYear();
	});
//SET TEXTFIELD VALUE AND CLOSE PICKER
	done.addEventListener('click', function() {
		var rDate = picker.value;
		var month = rDate.getMonth() + 1;
		//check date here 
		
		txtDODate.value = rDate.getDate() + "/" + month + "/" + rDate.getFullYear();
		picker_view.animate(slide_out);
	});
	
	txtDODate.addEventListener('click', function(e){
		if(dateValue < deliDate){
			ShowMessage('Can not choose receipt date. This order must be delivered after ' + deliDate.toDateString());
			return;
		}
		picker_view.animate(slide_in);
		//var alert11 = Titanium.UI.createAlertDialog({ title: 'No need to know?', message: '', buttonNames: ['Yes', 'No'], cancel: 1 });
		//alert11.show();
	});
	//End DO Date
	
	
	row1.add(createLabelBold(' Delivery Order', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row1.add(lbDONum);
	row1.add(createLabelBold(' Delivery Date', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row1.add(lbDODate);

	
	
	var row2 = Ti.UI.createTableViewRow({
		height: 50,
		layout: 'horizontal'
	});
	
	var lbPONo = createLabel(_header.NumAtCard, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	var lbOutlet = createLabel(_header.OutletCode, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	
	row2.add(createLabelBold(' P/O No.', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row2.add(lbPONo);
	row2.add(createLabelBold(' Outlet Code', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row2.add(lbOutlet);
	
	var row3 = Ti.UI.createTableViewRow({
		height: 50,
		layout: 'horizontal'
	});
	
	var tr = Titanium.UI.create2DMatrix();
	tr = tr.rotate(90);
	var lbDriver = createLabel(_header.U_Driver, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	var lbStatus = createLabel(_header.Descr, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	var drop_button =  Titanium.UI.createButton({
			style:Titanium.UI.iPhone.SystemButton.DISCLOSURE,
			transform:tr
	});
		
	var cboStatus = Titanium.UI.createTextField({
		left:'NONE',
		value: _header.Descr,
		height:35,
		width:widthCell,
		borderStyle:Titanium.UI.INPUT_BORDERSTYLE_ROUNDED,
		font:{fontSize:Ti.App.ROWFONTSIZE, fontFamily:'Helvetica-Bold'},
		enabled:false, //(_header.Descr !== 'Complete'),
		color: '#660000'
	});
	
	row3.add(createLabelBold(' Driver ', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row3.add(lbDriver);
	row3.add(createLabelBold(' Delivery Status', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	//row3.add(lbStatus);
	row3.add(cboStatus);
	
	
	var row4 = Ti.UI.createTableViewRow({
		height: 50,
		layout: 'horizontal'
	});
	
	row4.add(createLabelBold(' Receipt Date', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row4.add(txtDODate);
	/*
	var row4 = Ti.UI.createTableViewRow({
		height: 40,
		layout: 'horizontal'
	});
	
	var lbAcNo = createLabel(_header.ACNo, 'NONE', 'NONE', widthCell, 40, '#660000', 'left');
	row4.add(createLabelBold(' A/C No. ', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row4.add(lbAcNo);
	
	var row5 = Ti.UI.createTableViewRow({
		height: 40,
		layout: 'horizontal'
	});
	
	var lbAddress = createLabel(_header.Address2, 'NONE', 'NONE', widthCell + widthCell, 40, '#660000', 'left');
	row5.add(createLabelBold(' Address ', 'NONE', 'NONE', widthCell, 40, '#000000', 'left'));
	row5.add(lbAddress);
	*/
	var rowData= [];
	rowData.push(row1);
	rowData.push(row2);
	rowData.push(row3);
	rowData.push(row4);
	//rowData.push(row5);
	topTable.data = rowData;
	
	
	
	var viewHeader = Ti.UI.createView({
		top: 200 + 15,
		left: 0,
		height: Ti.App.BUTTONHEIGHT,
		//backgroundColor:'#660000',
		layout:'horizontal',
		backgroundGradient: {
		      type: 'linear',
		      colors: ['#660000','#8C3131'],
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y:Ti.App.BUTTONHEIGHT},
		      backFillStart:false}
	});

	var perWidthHdr = ['7%', '17%','30%', '11%', '11%', '11%', '10%', '6%'];
	
	//createLabelBold = function(_text, _left, _top, _width, _height, _color, _align)
	viewHeader.add(createLabelBold('No', 'NONE', 'NONE', perWidthHdr[0], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'center'));
	viewHeader.add(createLabelBold('Item ID', 'NONE', 'NONE', perWidthHdr[1], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left'));
	viewHeader.add(createLabelBold('Description', 'NONE', 'NONE', perWidthHdr[2], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'left'));
	viewHeader.add(createLabelBold('DO\nQty', 'NONE', 'NONE', perWidthHdr[3], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'right'));
	viewHeader.add(createLabelBold('PO\nQty', 'NONE', 'NONE', perWidthHdr[4], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'right'));
	viewHeader.add(createLabelBold('Rec\nQty', 'NONE', 'NONE', perWidthHdr[5], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'right'));
	viewHeader.add(createLabelBold('Unit', 'NONE', 'NONE', perWidthHdr[6], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'center'));
	//viewHeader.add(createLabelBold('Bch', 'NONE', 'NONE', perWidthHdr[7], Ti.App.BUTTONHEIGHT, '#FFFFFF', 'center'));

	var tableHeightFull = Ti.App.SCREEN_HEIGHT - 200 - Ti.App.BUTTONHEIGHT - 72;
	var tableHeightShort = Ti.App.SCREEN_HEIGHT - 200 - Ti.App.BUTTONHEIGHT - 72 - 60;
	
	var viewContain = Ti.UI.createTableView({
		top: 200 + Ti.App.BUTTONHEIGHT + 10,
		
		height: tableHeightFull,
		borderWidth:2,
		borderColor:'#660000'
	});
	
	var viewContainEditMode= Ti.UI.createTableView({
		top: 200 + Ti.App.BUTTONHEIGHT + 10,
		height: tableHeightFull,
		borderWidth:2,
		borderColor:'#1872CC',
		visible:false
	});

	///////////
	var slideUP = Titanium.UI.createAnimation({
		top: -170,
		duration: 300	
	});
	
	var slideDown = Titanium.UI.createAnimation({
		top: 0,
		duration: 300
	});
	
	var shorten = Titanium.UI.createAnimation({
		height:tableHeightShort,
		duration:300
	});
	
	var enlarge = Titanium.UI.createAnimation({
		height: tableHeightFull,
		duration:300
	});
	
	var opaConfirm = Titanium.UI.createAnimation({
		duration:300,
		opacity:1
	});
	
	createTextField = function(_text, _left, _width, _align, _doQty){
	
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
			font:{fontSize:Ti.App.ROWFONTSIZE,  fontFamily:'Helvetica-Bold'},
			borderStyle:Ti.UI.INPUT_BORDERSTYLE_ROUNDED,
			keyboardType: Ti.UI.KEYBOARD_NUMBER_PAD,
			keyboardToolbar:[flexSpace, send]
		});
		 textField.addEventListener('blur', function(e) {
		 	var rQty = parseFloat(textField.value);
		 	var ddoQty = parseFloat(_doQty);
		 	if(rQty > ddoQty){
		 		ShowMessage('The receive qty must less or equal DO qty');
		 		textField.focus();
		 	}else
		 		root.animate(slideDown);
	     });
	     
		 textField.addEventListener('focus', function(e) {
		 	root.animate(slideUP);
	      	send.activeFld = textField;
	     });
	      
	      return textField;
	};

createDetailRow = function(_no, _itemID, _description, _qty,_poQty,  _recQty, _unit, _isBatch, _editMode){
	var row = Ti.UI.createTableViewRow({
		height:Ti.App.ROWHEIGHT,
		layout:'horizontal' 
	});
	
	var viewRow = Ti.UI.createView({
		width:Ti.App.SCREEN_WIDTH,
		height: Ti.App.ROW_HEIGHT,
		layout: 'horizontal'
	});
	
	viewRow.add(createLabelDetail(_no + 1, 'NONE', perWidthHdr[0], 'center'));
	viewRow.add(createLabelDetail(_itemID,  'NONE', perWidthHdr[1], 'left'));
	viewRow.add(createLabelDetail(_description,  'NONE', perWidthHdr[2], 'left'));
	viewRow.add(createLabelDetail(_qty,  'NONE', perWidthHdr[3], 'right'));
	viewRow.add(createLabelDetail(_poQty,  'NONE', perWidthHdr[4], 'right'));
	
	if(_editMode === false){
		viewRow.add(createLabelDetail(_recQty,  'NONE', perWidthHdr[5], 'right'));
	}
	else{
		viewRow.add(createTextField(_recQty,  'NONE', perWidthHdr[5], 'right', _qty));
	}
	viewRow.add(createLabelDetail(_unit,  'NONE', perWidthHdr[6], 'center'));
	
	//viewRow.add(createLabelDetail((_isBatch==='Y'?'Y':''),  'NONE', perWidthHdr[7], 'center'));
	
	
	row.add(viewRow);
	return row;
};
	
	var WhseCode;
	////////////
	callbackError = function(){
		indicator.closeIndicator();
	};
	
	callbackDeliveryDetail = function(_jsonText){
		var detailData = [];
		var detailDataEditMode = [];
		
		var jsonData = XMLToJSON(_jsonText);
		if(jsonData !== null){
			var i = 0;
			WhseCode = jsonData[i].WhsCode;
			for(i = 0; i< jsonData.length; i++){
				
				var recQ = jsonData[i].U_RecQty;
				if(recQ === 0)
					recQ = jsonData[i].Quantity;
				//var lNum = jsonData[i].LineNum + 1;
				detailData.push(createDetailRow(jsonData[i].LineNum, jsonData[i].ItemCode, jsonData[i].Dscription, jsonData[i].Quantity, jsonData[i].U_AB_POQty, jsonData[i].U_RecQty, jsonData[i].UnitMsr, jsonData[i].ManbtchNum, false));
				detailDataEditMode.push(createDetailRow(jsonData[i].LineNum, jsonData[i].ItemCode, jsonData[i].Dscription, jsonData[i].Quantity, jsonData[i].U_AB_POQty, recQ, jsonData[i].UnitMsr, jsonData[i].ManbtchNum, true));
			}
		}
		viewContain.setData(detailData);
		viewContainEditMode.setData(detailDataEditMode);
		indicator.closeIndicator();
	};
	if(_header.Descr == 'Completed'){
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetOrderDetailComplete', {'DocEntry' : _header.DocEntry, 'DBName':_header.DBName}, callbackDeliveryDetail, callbackError);
	}
	else
	{
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetOrderDetail', {'DocEntry' : _header.DocEntry, 'DBName':_header.DBName}, callbackDeliveryDetail, callbackError);
	}
	

	var alert = Titanium.UI.createAlertDialog({ title: 'Acknowledge?', message: '', buttonNames: ['Yes', 'No'], cancel: 1 });
	alert.addEventListener('click', function(e) { 
		
		if (e.cancel === e.index || e.cancel === true) {
			turnBackZero();
			return;
		}
	
		switch (e.index) {
			case 0: 
				var ACK  = require('/ui/common/Acknowledge');
				var DODate = new Date(picker.value);
				var monthX = DODate.getMonth() + 1;
				var dateStr = monthX + '/' + DODate.getDate() + '/' + DODate.getFullYear();
				//var DODate = new Date(picker.value);
				//var alert22 = Titanium.UI.createAlertDialog({ title: String.formatDate(DODate, "short"), message: '', buttonNames: ['Yes', 'No'], cancel: 1 });
				//alert22.show();
				//var acknow = ACK.Acknowledge(_header.DocEntry, _header.DBName, String.formatDate(DODate, "short"), callbackClose);
				var acknow = ACK.Acknowledge(_header.DocEntry, _header.DBName, dateStr, callbackClose);
		//acknow.open({modal:true});
				acknow.open(opaConfirm);
  				break;
  			
  			default:
  				break;
  		}
	});

	callbackAfterSave = function(){
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetOrderDetail', {'DocEntry' : _header.DocEntry, 'DBName':_header.DBName}, callbackDeliveryDetail, callbackError);
		btn_Edit.setTitle('Edit');
		viewContain.setVisible(true);
		viewContainEditMode.setVisible(false);
		isEditing = false;
		indicator.closeIndicator();
		//cboStatus.setEnabled(true);
		alert.show();
		
	};
		
	/*
	 * Update 13 AUG 2014
	 * 
	 */	
	turnBackZero = function(){
		indicator.openIndicator();
		var jsonObjs = [];
		if(typeof viewContainEditMode.data[0].rows != 'undefined')
		{
			var numOfItem = viewContainEditMode.data[0].rows.length;
			for(var i = 0; i< numOfItem; i++){
				var LineNum = +viewContainEditMode.data[0].rows[i].children[0].children[0].text;
	
				jsonObjs.push({"LineNum":LineNum - 1, "DocEntry": _header.DocEntry, "RecQty":0});
			}
			var jsonString = JSON.stringify(jsonObjs);
			getRemoteData(Titanium.App.Properties.getString("locationservice") + "/Mobile.asmx/PostOrderDetail", {jsonText:jsonString, DBName: _header.DBName}, callbackAfterTurnBack, callbackError);			
		}
	};
	
	callbackAfterTurnBack = function(){
		getRemoteData(Titanium.App.Properties.getString("locationservice") + '/Mobile.asmx/GetOrderDetail', {'DocEntry' : _header.DocEntry, 'DBName':_header.DBName}, callbackDeliveryDetail, callbackError);
		btn_Edit.setTitle('Edit');
		viewContain.setVisible(true);
		viewContainEditMode.setVisible(false);
		isEditing = false;
		indicator.closeIndicator();
		//cboStatus.setEnabled(true);
	};
	/*
	 * END UPDATE 13 AUG 2014
	 */
	
	
	
	btn_Edit.addEventListener('click', function(e){
		if(dateValue<deliDate){
			ShowMessage('Can not process. This order must be delivered after ' + deliDate.toDateString());
			return;
		}
		
		txtDODate.touchEnabled = true;
		if(txtDODate.value.trim() == ''){
			ShowMessage('Please input the receipt date.');
			picker_view.animate(slide_in);
			return;
		}
			
		if(isEditing === false){
			btn_Edit.setTitle('Save');
			viewContain.setVisible(false);
			viewContainEditMode.setVisible(true);
			isEditing = true;
			return;
		}else{ //save data
			if(checkRecQtyOrderQty()===false){
				root.animate(slideDown);
				ShowMessage('There some invalid receive quantities, please check.');
				return;
			}
			indicator.openIndicator();
			var jsonObjs = [];
			if(typeof viewContainEditMode.data[0].rows != 'undefined')
			{
				var numOfZero = 0;
				var numOfItem = viewContainEditMode.data[0].rows.length;
				for(var i = 0; i< numOfItem; i++){
					var qtyCop = viewContainEditMode.data[0].rows[i].children[0].children[3].text;
					
					var LineNum = +viewContainEditMode.data[0].rows[i].children[0].children[0].text;
					var recQty = viewContainEditMode.data[0].rows[i].children[0].children[5].value;
					
					if(IsNumeric(recQty) === true ){
							if(+recQty === 0)
								numOfZero++;						
						//if(+recQty !== 0){
							//recQty = qtyCop;
							jsonObjs.push({"LineNum":LineNum - 1, "DocEntry": _header.DocEntry, "RecQty":recQty});
						//}else{
							//indicator.closeIndicator();
							//ShowMessage('Reveive Qty must greater than 0');
							//return;
						//}
					}
					else{
						indicator.closeIndicator();
						ShowMessage('Please input numeric');
						return;
					}
				}
				if(numOfZero === numOfItem){
					indicator.closeIndicator();
					ShowMessage('Please input at least 1 item with qty greater than 0');
					return;
				}
				
				var jsonString = JSON.stringify(jsonObjs);
				getRemoteData(Titanium.App.Properties.getString("locationservice") + "/Mobile.asmx/PostOrderDetail", {jsonText:jsonString, DBName: _header.DBName}, callbackAfterSave, callbackError);
					
			}
			
		}
		
	});
	
	function checkRecQtyOrderQty(){
		if(typeof viewContainEditMode.data[0].rows != 'undefined')
		{
			var numOfItem = viewContainEditMode.data[0].rows.length;
			for(var i = 0; i< numOfItem; i++){
				var adoQty = +viewContainEditMode.data[0].rows[i].children[0].children[3].text;
				var arecQty = +viewContainEditMode.data[0].rows[i].children[0].children[5].value;
				if(arecQty>adoQty)
					return false;
			}
			return true;
				
		}else
		{
			return false;
		}
	}

	function IsNumeric(input)
	{
    	return (input - 0) == input && (input+'').replace(/^\s+|\s+$/g, "").length > 0;
	}
	
	callbackClose = function(){
		cboStatus.setValue('Completed');
		root.setRightNavButton(null);
	};
	
	
	
	
	
	win.add(viewHeader);
	win.add(topTable);
	win.add(viewContain);
	win.add(viewContainEditMode);
	win.add(picker_view);
	
	var nav = Titanium.UI.iPhone.createNavigationGroup({
    	window:win
	});
	
	var root = Titanium.UI.createWindow({
		//backButtonTitle:'Back',
		title: 'Delivery Order Detail'
	});
	if(_header.Descr !== 'Completed'){
		root.setRightNavButton(btn_Edit);
	}
	else{
		txtDODate.touchEnabled = false;
	}
	
	
	root.add(nav);
	root.addEventListener('close', function()
	{
		//if(_header.Descr !== 'Completed')
			_callbackReloadData();
	});
	return root;	
};