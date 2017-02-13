/// DROP BOX SESSION
/*	
 * 
/*
var batchDataPush = [];
callbackReturnMain = function(batchData){
	for(var i = 0; i< batchData.length; i++){
		var bNum = batchData[i].LineNum;
		for(var j = 0; j< batchDataPush.length; j++){
			if (batchDataPush[j].LineNum == bNum)
				batchDataPush.splice(j, 1);
		}
	}
	
	for(var v = 0; v< batchData.length; v++){
		batchDataPush.push(batchData[v]);
	}
};
*/

/*
	var picker_view = Titanium.UI.createView({
		height:251,
		bottom:-251
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
		items:[cancel,spacer,done],
				backgroundGradient: {
			      type: 'linear',
			      colors: ['#b5bdc8','#000000'],//28343b
			      startPoint: {x:0,y:0},
			      endPoint:{x:0,y:35},
			      backFillStart:false}
	});
	 
	var picker = Titanium.UI.createPicker({
			top:43
	});
	picker.selectionIndicator=true;

	//FOR ANIMATION
	var slide_in =  Titanium.UI.createAnimation({bottom:0});
	var slide_out =  Titanium.UI.createAnimation({bottom:-251});
	
	cboStatus.addEventListener('focus', function() {
		picker_view.animate(slide_out);
	});
	 
	drop_button.addEventListener('click',function() {
		picker_view.animate(slide_in);
		cboStatus.blur();
	});
	 
	cancel.addEventListener('click',function() {
		picker_view.animate(slide_out);
	}); 
	 
	done.addEventListener('click',function() {
		cboStatus.value = picker.getSelectedRow(0).title;
		picker_view.animate(slide_out);
		if(picker.getSelectedRow(0).id != 0){
			viewContain.animate(shorten);
			viewContainEditMode.animate(shorten);
			viewTapToAcknow.setVisible(true);
		}else{
			viewContain.animate(enlarge);
			viewContainEditMode.animate(enlarge);
			viewTapToAcknow.setVisible(false);
		}
	});
	
	var datapic = [];
	datapic.push(Titanium.UI.createPickerRow({title:'Processing', id:0}));
	datapic.push(Titanium.UI.createPickerRow({title:'Completed', id:1}));

	picker.add(datapic);
	picker_view.add(toolbar);
	picker_view.add(picker);
	/// END DROP BOX
	
	
	lbTapToAcknow.addEventListener('click', function(e){
		
		var ACK  = require('/ui/common/Acknowledge');
		var acknow = ACK.Acknowledge(_header.DocEntry, _header.DBName, callbackClose);
		acknow.open({modal:true});
		
	});
	
	var btn_TapToAck = Ti.UI.createButton({
		title: 'Tap to sign and acknowledge'	
	});
	
	
	viewTapToAcknow.add(lbTapToAcknow);
	
	//win.add(picker_view);
	win.add(viewTapToAcknow);
	
	var viewTapToAcknow = Ti.UI.createView({
		backgroundColor:'transparent',
		bottom:0,
		height:40,
		visible:false	
	});
	var lbTapToAcknow = Ti.UI.createLabel({
		text: 'Tap to sign and acknowledge'
	});
	
	/*
		viewRow.addEventListener('click', function(e){
			
			if(_isBatch === 'Y'){
				var toBatch = [];
				for(var i = 0; i< batchDataPush.length; i++){
					if (batchDataPush[i].LineNum == _no){
						toBatch.push(batchDataPush[i]);
					}
				}
				var batch = require('ui/common/BatchControl');
				var bt = batch.BatchControl({LineNum: _no, ItemCode:_itemID, Dscription: _description, TotalQty: _qty,  AvailableBatch:toBatch}, callbackReturnMain);
				bt.open({modal:true});
			}
		});
*/