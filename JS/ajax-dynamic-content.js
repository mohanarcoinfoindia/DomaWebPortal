/************************************************************************************************************
Ajax dynamic content
Copyright (C) 2006  DTHMLGoodies.com, Alf Magne Kalleland

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

Dhtmlgoodies.com., hereby disclaims all copyright interest in this script
written by Alf Magne Kalleland.

Alf Magne Kalleland, 2006
Owner of DHTMLgoodies.com
	
************************************************************************************************************/	



var enableCache = true;
var jsCache = new Array();

var dynamicContent_ajaxObjects = new Array();

function ajax_showContent(divId, ajaxIndex, fullUrl) {

    var content;
    try {
        var response = dynamicContent_ajaxObjects[ajaxIndex].response;
        var doc = (new DOMParser()).parseFromString(response, "text/html");
        if (doc) {
            content = doc.body.innerHTML;
        } else {
            content = dynamicContent_ajaxObjects[ajaxIndex].response;
        }
    }
    catch(e) {
        content = dynamicContent_ajaxObjects[ajaxIndex].response;
    }
    
    $get(divId).innerHTML = content;
   

    if (enableCache) {        
        jsCache[fullUrl] = 	content;
	}
	dynamicContent_ajaxObjects[ajaxIndex] = false;
}

function ajax_loadContent(divId,url)
{
    
	if(enableCache && jsCache[url]){
        document.getElementById(divId).innerHTML = jsCache[url];        
		return;
	}

    var fullUrl = url;

	var ajaxIndex = dynamicContent_ajaxObjects.length;
	document.getElementById(divId).innerHTML = UIMessages[5];
	dynamicContent_ajaxObjects[ajaxIndex] = new sack();

    //split querystring params and url
	if(url.indexOf('?')>=0){
		dynamicContent_ajaxObjects[ajaxIndex].method='GET';
		var string = url.substring(url.indexOf('?'));
		url = url.replace(string,'');
		string = string.replace('?','');
		var items = string.split(/&/g);
		for(var no=0;no<items.length;no++){
			var tokens = items[no].split('=');
			if(tokens.length==2){
				dynamicContent_ajaxObjects[ajaxIndex].setVar(tokens[0],tokens[1]);
			}	
		}	
		url = url.replace(string,'');
	}

  
	dynamicContent_ajaxObjects[ajaxIndex].requestFile = url;	// Specifying which file to get
    dynamicContent_ajaxObjects[ajaxIndex].onCompletion = function () { ajax_showContent(divId, ajaxIndex, fullUrl); };	// Specify function that will be executed after file has been found
	dynamicContent_ajaxObjects[ajaxIndex].runAJAX();		// Execute AJAX function	
	
	
}
