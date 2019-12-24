var curPar = 0;

//logging
var log_nn = 1; var log_str = '';
function log(s) {
	//return;
	try {
		var inner_div = document.getElementById('debuginfo');
		if (inner_div === null) {
			var out_div = document.createElement('div');out_div.style.background = '#dedede';out_div.style.border = '1px solid silver';out_div.style.padding = '5px';out_div.style.width = '300px';out_div.style.position = 'fixed';out_div.style.zIndex = '9999999';out_div.style.right = '50px';out_div.style.top = '5px';out_div.style.bottom = '5px';
			var inner_div = document.createElement('div');inner_div.id = 'debuginfo';inner_div.style.background = '#fff';inner_div.style.position = 'absolute';inner_div.style.right = '0';inner_div.style.left = '0';inner_div.style.top = '1.5em';inner_div.style.bottom = '0';inner_div.style.fontSize='0.7em';inner_div.style.overflow='auto';out_div.appendChild(inner_div);
			var clear_btn = document.createElement('button');
			clear_btn.style.border = 'none';clear_btn.style.position = 'absolute';clear_btn.style.background = '#ddd';clear_btn.style.right = '10px';clear_btn.style.left = '10px';clear_btn.style.top = '0';clear_btn.innerHTML = ' Clear ';
			clear_btn.onclick = function(e) { document.getElementById('debuginfo').innerHTML = ''; log_nn=1; log_str = '';};
			out_div.appendChild(clear_btn);
			document.body.appendChild(out_div);
		}
		log_str = '<p>' + '<span style="color: #000000; font-size: .7em">' + log_nn + '</span>' + '. ' + s + '</p>' + log_str;
		inner_div.innerHTML = log_str;
		log_nn++;
	} catch (e) {
		alert(e.message);
	}
}

/*
Element.prototype.documentOffsetTop = function () {
    return this.offsetTop + ( this.offsetParent ? this.offsetParent.documentOffsetTop() : 0 );
};
*/

// select a list of matching elements, context is optional
function $(selector, context) {
    return (context || document).querySelectorAll(selector);
}
// select the first match only, context is optional
function $1(selector, context) {
    return (context || document).querySelector(selector);
}
function removeElemClass(elsem, classToBeRemoved) {
	if (elsem.classList.contains(classToBeRemoved)) {
		elsem.classList.remove(classToBeRemoved);
	}
}
function addElemClass(elsem, classToBeAdded) {
	if (!elsem.classList.contains(classToBeAdded)) {
		elsem.classList.add(classToBeAdded);
	}
}
function removeRangeClass(range, classToBeRemoved) {
    var i = range.length
	while (i--) {
		removeElemClass(range[i], classToBeRemoved);
	}
}
function addRangeClass(range, classToBeAdded) {
    var i = range.length
	while (i--) {
		addElemClass(range[i], classToBeAdded);
	}
}

function onLoaded() {
	try {
		var elem  = document.getElementById('sound_pos');
		var sound_pos_text = elem.value;
		nativeObject.onHtmlLoaded(sound_pos_text); // Call the projected WinRT method.
		var settings = nativeObject.getSettings();
		handle_settings(settings);
	} catch (e) {
		log("onLoaded " + e.message);
	}
}

//settings
var language = 0;
var font_size = 3;
var cur_font_size = 3;
var color_mode = 0;
var highlight_paragraph = 1;
var audioplay_mode = 2;
var autoscroll_mode = 0;
function handle_settings(settings) {
	try {
		//log(settings);
		settings = settings.split(';');
		language = parseInt(settings[0]);
		font_size = parseInt(settings[1]);
		color_mode = parseInt(settings[2]);
		highlight_paragraph = parseInt(settings[3]);
		audioplay_mode = parseInt(settings[4]);
		autoscroll_mode = parseInt(settings[5]);
		
		if (language == 0) {
			removeRangeClass($(".book_paragraph_engl"), "book_paragraph_single");
			removeRangeClass($(".book_paragraph_engl"), "book_paragraph_hidden");
			removeRangeClass($(".book_paragraph_arm"), "book_paragraph_single");
			removeRangeClass($(".book_paragraph_arm"), "book_paragraph_hidden");
		} else if (language == 1) {
			addRangeClass($(".book_paragraph_engl"), "book_paragraph_single");
			removeRangeClass($(".book_paragraph_engl"), "book_paragraph_hidden");
			removeRangeClass($(".book_paragraph_arm"), "book_paragraph_single");
			addRangeClass($(".book_paragraph_arm"), "book_paragraph_hidden");
		} else {
			addRangeClass($(".book_paragraph_arm"), "book_paragraph_single");
			removeRangeClass($(".book_paragraph_arm"), "book_paragraph_hidden");
			removeRangeClass($(".book_paragraph_engl"), "book_paragraph_single");
			addRangeClass($(".book_paragraph_engl"), "book_paragraph_hidden");
		}

		if (font_size != cur_font_size) {
			var abs_font_size = 'medium';
			if (font_size == 0) {
				abs_font_size = 'xx-small';
			} else if (font_size == 1) {
				abs_font_size = 'x-small';
			} else if (font_size == 2) {
				abs_font_size = 'small';
			} else if (font_size == 3) {
				abs_font_size = 'medium';
			} else if (font_size == 4) {
				abs_font_size = 'large';
			} else if (font_size == 5) {
				abs_font_size = 'x-large';
			} else if (font_size == 6) {
				abs_font_size = 'xx-large';
			}
			var elem = document.getElementById('content_div');
			elem.style.fontSize = abs_font_size;
			cur_font_size = font_size;
		}
		if (color_mode == 0) {
			removeElemClass(document.body, "body-color-white");
		} else {
			addElemClass(document.body, "body-color-white");
		}
		
		if (highlight_paragraph == 0) {
			removeCurHighlight();
		} else {
			set_cur_par(curPar);
		}	
		
	} catch (e) {
		log("handle_settings " + e.message);
	}
}

function get_par_color(sel) {
    var color;
    if (sel) {
        if (color_mode == 0) {
//            color = '#1033FF';
            color = '#0B24B5';
        } else {
//            color = '#0000FF';
            color = '#0000D0';
        }
    } else {
        if (color_mode == 0) {
            color = '#4C3B26';
//            color = '#7A5F3D';
        } else {
//            color = '#000000';
            color = '#000000';
        }
    }
    return color;
}

function removeCurHighlight() {
    if (curPar >= 0) {
		var elem = document.getElementById('book_paragraph_'+curPar);
        elem.style.color = get_par_color(false);
    }
}

function set_cur_par(par) {
    if (highlight_paragraph != 0 && par >= 0) {
        removeCurHighlight();
		var elem = document.getElementById('book_paragraph_'+par);
        elem.style.color = get_par_color(true);
    }
    curPar = par;
}

function elem_pos(id, elem) {
	return  id
			+ ' offsetTop: ' + elem.offsetTop
			+ ' offsetHeight: ' + elem.offsetHeight
			+ ' scrollTop: ' + elem.scrollTop
			+ ' scrollHeight ' + elem.scrollHeight
			"\r\n";
}

function paragraph_onclick(par) {
	try {
		var dbg = ""
		//+ par
//		+  ' innerHeight ' + window.innerHeight
//		+ elem_pos("par", document.getElementById('book_paragraph_'+par))
//		+ elem_pos(' div', document.getElementById('content_div'))
		;
		//log(dbg);
		set_cur_par(par);
		nativeObject.onParagraphClick(par, dbg);
	} catch (e) {
		log("paragraph_onclick " + e.message);
	}
}

var scrolNN = 0;
function scrollToSmoothly(pos, time, cur_scrolNN){
	pos = Math.floor(pos);
	var currentPos = window.scrollY;
	//log("========== pos " + pos + " currentPos " + currentPos);
	var start = null;
	time = time || 500;
	window.requestAnimationFrame(function step(currentTime) {
		if(cur_scrolNN != scrolNN) {
			//log("========== cur_scrolNN " + cur_scrolNN + " scrolNN " + scrolNN);
			return;
		}
		start = !start? currentTime: start;
		var progress = currentTime - start;
		var scrollPos;
		if(currentPos < pos){
			scrollPos = Math.floor(((pos-currentPos)*progress/time)+currentPos);
		} else {
			scrollPos = Math.floor(currentPos-((currentPos-pos)*progress/time));
		}
		/*log(""
		+ " scrollPos " + scrollPos
		+ " start " + Math.floor(start)
		+ " progress " + Math.floor(progress)
		+ " curTime " + Math.floor(currentTime)
		+ " pos " + pos
		+ " curPos" + Math.floor(currentPos)
		);*/
		if(progress < time && pos != scrollPos){
			window.scrollTo(0, scrollPos);
			var newPos = window.scrollY;
			if (newPos == scrollPos)
				window.requestAnimationFrame(step);
			//log("....... newPos " + newPos);
		} else {
			//log("----------- pos " + (pos == scrollPos) + " time " + (progress >= time));
			window.scrollTo(0, pos);
		}
	});
}

function get_scroll_pos(par) {
	try {
		var elem = document.getElementById('book_paragraph_'+par);
		var pos = elem.offsetTop;
		var viewportHeight = window.innerHeight;
		var elemHeight = elem.offsetHeight;
		//log("offsetTop" + elem.offsetTop + " scrollHeight " + elem.scrollHeight + " offsetHeight " + elem.offsetHeight);
		if (elemHeight < viewportHeight)
			pos -= (viewportHeight - elemHeight)/2;
		return pos;
	} catch (e) {
		log('scroll_pos '+ e.message);
	}
}

function scroll_to_par(par) {
	try {
		set_cur_par(par);
		if (autoscroll_mode < 2) {
			var pos = get_scroll_pos(par);
			if (autoscroll_mode == 0) {
				scrollToSmoothly(pos, 500, ++scrolNN);
            } else {
				window.scrollTo(0, pos);
            }
		}
	} catch (e) {
		log('scroll_to_par '+ e.message);
	}
}

function restore_pos(par) {
	try {
		set_cur_par(par);
		var pos = get_scroll_pos(par);
		window.scrollTo(0, pos);
	} catch (e) {
		log('restore_pos '+ e.message);
	}
}
