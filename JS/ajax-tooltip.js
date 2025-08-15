var x_offset_tooltip = 5;
var y_offset_tooltip = 0;



var ajax_tooltipObj = false;
var ajax_tooltipObj_iframe = false;
var divHeight = 0;

var ajax_tooltip_MSIE = false;
if(navigator.userAgent.indexOf('MSIE')>=0)ajax_tooltip_MSIE=true;


function ajax_showTooltip(externalFile,inputObj,e)
{
    if (externalFile == "") {
        return;
    }
	if(!ajax_tooltipObj)	/* Tooltip div not created yet ? */
	{
		ajax_tooltipObj = document.createElement('DIV');
		ajax_tooltipObj.style.position = 'absolute';
		ajax_tooltipObj.id = 'ajax_tooltipObj';		
		document.body.appendChild(ajax_tooltipObj);

		var leftDiv = document.createElement('DIV');	/* Create arrow div */
		leftDiv.className='ajax_tooltip_arrow';
		leftDiv.id = 'ajax_tooltip_arrow';
		ajax_tooltipObj.appendChild(leftDiv);
		
		var contentDiv = document.createElement('DIV'); /* Create tooltip content div */
		contentDiv.className = 'ajax_tooltip_content';
		contentDiv.id = 'ajax_tooltip_content';
		ajax_tooltipObj.appendChild(contentDiv);

		divHeight = contentDiv.offsetHeight;
		
		if(ajax_tooltip_MSIE){	/* Create iframe object for MSIE in order to make the tooltip cover select boxes */
		    ajax_tooltipObj_iframe = document.createElement("iframe");
		    ajax_tooltipObj_iframe.setAttribute("frameborder", "0"); 
			ajax_tooltipObj_iframe.style.position = 'absolute';
			ajax_tooltipObj_iframe.border='0';
			ajax_tooltipObj_iframe.frameborder=0;
			ajax_tooltipObj_iframe.style.backgroundColor='#FFF';
			ajax_tooltipObj_iframe.src = 'about:blank';
			contentDiv.appendChild(ajax_tooltipObj_iframe);
			ajax_tooltipObj_iframe.style.left = '0px';
			ajax_tooltipObj_iframe.style.top = '0px';
		}	
	}
	// Find position of tooltip
    ajax_tooltipObj.style.display = 'block';

	ajax_loadContent('ajax_tooltip_content', externalFile + "&tlt=1");
	



	if (ajax_tooltip_MSIE) {
	    if (ajax_tooltipObj.clientWidth > 0)
	        ajax_tooltipObj_iframe.style.width = ajax_tooltipObj.clientWidth + 'px';
		if (ajax_tooltipObj.clientHeight > 0)
		    ajax_tooltipObj_iframe.style.height = ajax_tooltipObj.clientHeight + 'px';
    }

    ajax_positionTooltip(inputObj, e);
    
	
	
}
function getWindowHeight() {
    var windowHeight=0;
    var ttyOffset = 0; 

      if( typeof( window.pageYOffset ) == 'number' ) {
        //Netscape compliant
        ttyOffset = window.pageYOffset;
      } else if( document.body && ( document.body.scrollLeft || document.body.scrollTop ) ) {
        //DOM compliant
        ttyOffset = document.body.scrollTop;
      } else if( document.documentElement && ( document.documentElement.scrollLeft || document.documentElement.scrollTop ) ) {
        //IE6 standards compliant mode
        ttyOffset = document.documentElement.scrollTop;
      }
  
    if (typeof(window.innerHeight)=='number') {
    windowHeight=window.innerHeight;
    }
    else {
    if (document.documentElement&&
    document.documentElement.clientHeight) {
    windowHeight=
    document.documentElement.clientHeight;
    }
    else {
    if (document.body&&document.body.clientHeight) {
    windowHeight=document.body.clientHeight;
    }
    }
}
    return windowHeight + ttyOffset;
}


function ajax_positionTooltip(inputObj,e)
{
	var leftPos = (ajaxTooltip_getLeftPos(inputObj) + inputObj.offsetWidth);
	var topPos = (ajaxTooltip_getTopPos(inputObj,e));
	var windowHeight = getWindowHeight();
	//divHeight = ajax_tooltipObj.offsetHeight; // ajax_tooltipObj.offsetHeight;
//	alert(ajax_tooltipObj.tagName);
//	alert('dh : ' + divHeight);
	
	var diff = windowHeight - divHeight - topPos;
    if(diff < 0){ topPos = topPos + diff; }

	
	var rightedge=ajax_tooltip_MSIE? document.body.clientWidth-leftPos : window.innerWidth-leftPos
	/*
	var bottomedge=ajax_tooltip_MSIE? document.body.clientHeight-topPos : window.innerHeight-topPos
	*/
	var tooltipWidth = $get('ajax_tooltip_content').offsetWidth + $get('ajax_tooltip_arrow').offsetWidth;
	// Dropping this reposition for now because of flickering
	
    var offset = tooltipWidth - rightedge;
    if (offset > 0 && diff >= 0) {
	    leftPos = Math.max(0, leftPos - offset - 5);
	    topPos = topPos + 5;  
	}
    
	ajax_tooltipObj.style.left = leftPos + 'px';
	ajax_tooltipObj.style.top = topPos + 'px';
    if (diff < 0 )
        $get('ajax_tooltip_arrow').style.top = -diff + 'px';
    else
        $get('ajax_tooltip_arrow').style.top = '0px';
   
}


function ajax_hideTooltip() {   
    ajax_tooltipObj.style.display = 'none';    
}

function ajaxTooltip_getTopPos(inputObj,e)
{		

//  var returnValue = inputObj.offsetTop;
//  while((inputObj = inputObj.offsetParent) != null){
//  	if((inputObj.tagName!='HTML'))returnValue += inputObj.offsetTop;
//  }

e = e||event;
var returnValue;
var ttyOffset = 0; 

  if( typeof( window.pageYOffset ) == 'number' ) {
    //Netscape compliant
    ttyOffset = window.pageYOffset;
  } else if( document.body && ( document.body.scrollLeft || document.body.scrollTop ) ) {
    //DOM compliant
    ttyOffset = document.body.scrollTop;
  } else if( document.documentElement && ( document.documentElement.scrollLeft || document.documentElement.scrollTop ) ) {
    //IE6 standards compliant mode
    ttyOffset = document.documentElement.scrollTop;
  }

    returnValue = e.clientY + ttyOffset;
 
  return returnValue;
}

function ajaxTooltip_getLeftPos(inputObj)
{
  var returnValue = inputObj.offsetLeft;
  while((inputObj = inputObj.offsetParent) != null){
  	if(inputObj.tagName!='HTML')returnValue += inputObj.offsetLeft;
  }
  return returnValue;
}