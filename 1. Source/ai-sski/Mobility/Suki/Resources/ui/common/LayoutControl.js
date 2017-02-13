createButton = function( _title, _left, _top, _width, _height){
		return Ti.UI.createButton({
			top:_top,
			height:_height,
			title:_title,
			width:_width,
			left: _left,
			borderColor:'#FFFFFF',
			//backgroundColor:'#660000',
			selectedColor:'#DE0000',
			borderRadius:4,
			font:{fontSize:Ti.App.ROWFONTSIZE,fontFamily:'Helvetica-Bold'},
			backgroundImage: 'NONE',
			backgroundGradient: {
		      type: 'linear',
		      colors: ['#4D0000','#D65454'],
		      //colors: ['#660000','#D65454'],//8C3131
		      startPoint: {x:0,y:0},
		      endPoint:{x:0,y: _height},
		      backFillStart:false}
		});
	};
	
createLabelBold = function(_text, _left, _top, _width, _height, _color, _align){
	return Ti.UI.createLabel({
		text: _text,
		left: _left,
		top: _top,
		width: _width,
		height: _height,
		font:{fontSize: Ti.App.HDRFONTSIZE,fontFamily:'Helvetica-Bold'},
		color: _color,
		textAlign: _align
	});
};

createLabel = function(_text, _left, _top, _width, _height, _color, _align){
	return Ti.UI.createLabel({
		text: _text,
		left: _left,
		top: _top,
		width: _width,
		height: _height,
		font:{fontSize: Ti.App.HDRFONTSIZE, fontFamily:'Helvetica-Bold'},
		color: _color,
		textAlign: _align
	});
};

createLabelDetail = function(_text, _left, _width, _align){
	return Ti.UI.createLabel({
		text: _text,
		left: _left,
		width: _width,
		height: Ti.App.ROWHEIGHT ,
		font:{fontSize:Ti.App.ROWFONTSIZE, fontFamily:'Helvetica-Bold'},
		textAlign: _align
	});
};

ShowMessage = function(_message){
	var dlg = Ti.UI.createAlertDialog({
		title: 'Suki',
		message: _message
	});
	dlg.show();
};
