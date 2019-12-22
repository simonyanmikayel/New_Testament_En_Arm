var cur_book_number = 0;
var cur_chapter = 1;
var sound_pos;
var curPar = -1;
var mp3_duration = 100;

var language = 0;
var font_size = 1;
var cur_font_size = 1;
var color_mode = 0;
var audioplay_mode = 2;
var autoscroll_mode = 2;
var change_sound_pos = true;
var highlight_paragraph = true;
var play_next_chapter = false;

var menu_items =
    [
        ["Matt",          "Matthew",         28, "Matt",      "The Gospel According to Matthew"],
        ["Mark",          "Mark",            16, "mark",      "The Gospel According to Mark"],
        ["Luke",          "Luke",            24, "luke",      "The Gospel According to Luke"],
        ["John",          "John",            21, "John",      "The Gospel According to John"],
        ["Acts",          "Acts",            28, "ACTS",      "Acts"],
        ["Romans",        "Romans",          16, "ROMANS",    "Paul's Letter to the Romans"],
        ["1Cor",          "1 Corinthias",    16, "1COR",      "Paul's First Letter to the Corinthians"],
        ["2Cor",          "2 Corinthias",    13, "2COR",      "Paul's Second Letter to the Corinthians"],
        ["Galatians",     "Galatians",       6,  "Galatians", "Paul's Letter to the Galatians"],
        ["Eph",           "Ephesians",       6,  "EPH",       "Paul's Letter to the Ephesians"],
        ["Philip",        "Philippians",     4,  "PHILIP",    "Paul's Letter to the Philippians"],
        ["Col",           "Colossians",      4,  "Col",       "Paul's Letter to the Colossians"],
        ["1Thess",        "1 Thessalonians", 5,  "1THESS",    "Paul's First Letter to the Thessalonians"],
        ["2Thess",        "2 Thessalonians", 3,  "2Thess",    "Paul's Second Letter to the Thessalonians"],
        ["1Tim",          "1 Timothy",       6,  "1Tim",      "Paul's First Letter to Timothy"],
        ["2Tim",          "2 Timothy",       4,  "2Tim",      "Paul's Second Letter to Timothy"],
        ["Titus",         "Titus",           3,  "Titus",     "Paul's Letter to Titus"],
        ["Philemon",      "Philemon",        1,  "Philemon",  "Paul's Letter to Philemon"],
        ["Hebrews",       "Hebrews",         13, "Hebrews",   "The Letter to the Hebrews"],
        ["James",         "James",           5,  "James",     "The Letter from James"],
        ["1Peter",        "1 Peter",         5,  "1PETER",    "Peter's First Letter"],
        ["2Peter",        "2 Peter",         3,  "2Peter",    "Peter's Second Letter"],
        ["1John",         "1 John",          5,  "1JOHN",     "John's First Letter"],
        ["2John",         "2 John",          1,  "2John",     "John's Second Letter"],
        ["3John",         "3 John",          1,  "3John",     "John's Third Letter"],
        ["Jude",          "Jude",            1,  "Jude",      "The Letter from Jude"],
        ["Rev",           "Revelation",      22, "REV",       "The Revelation to John"]
    ];

function log(message){
    parent.log(message);
}

$(document).ready(function() {
    setup_globals();
    setup_message_handler();
    setup_chapter();
});

$(document).keydown(function(e) {
    if (e.which == 13 || e.which == 32 || e.which == 'p' || e.which == 'P') {
        play_pause();
        e.preventDefault();
    }
});

function setup_globals() {
}

function setup_message_handler() {
    try {
        if (window.addEventListener) {
            // For standards-compliant web browsers
            window.addEventListener("message", dispatchMessage, false);
        }
        else {
            window.attachEvent("onmessage", dispatchMessage);
        }
    } catch (e) {
        //alert('exception: ' + e);
    }
}

function dispatchMessage(evt) {
    handleSettings(evt.data);
}

function handleSettings(settings) {
    settings = settings.split(';');
    language = parseInt(settings[0]);
    font_size = parseInt(settings[1]);
    color_mode = parseInt(settings[2]);
    audioplay_mode = parseInt(settings[3]);
    autoscroll_mode = parseInt(settings[4]);
    change_sound_pos = (settings[5] != 'false');
    highlight_paragraph = (settings[6] != 'false');
    play_next_chapter = (settings[7] != 'false');

    if (language == 0) {
        $(".book_paragraph_engl").removeClass("book_paragraph_single");
        $(".book_paragraph_engl").removeClass("book_paragraph_hidden");
        $(".book_paragraph_arm").removeClass("book_paragraph_single");
        $(".book_paragraph_arm").removeClass("book_paragraph_hidden");
    } else if (language == 1) {
        $(".book_paragraph_engl").addClass("book_paragraph_single");
        $(".book_paragraph_engl").removeClass("book_paragraph_hidden");
        $(".book_paragraph_arm").removeClass("book_paragraph_single");
        $(".book_paragraph_arm").addClass("book_paragraph_hidden");
    } else {
        $(".book_paragraph_arm").addClass("book_paragraph_single");
        $(".book_paragraph_arm").removeClass("book_paragraph_hidden");
        $(".book_paragraph_engl").removeClass("book_paragraph_single");
        $(".book_paragraph_engl").addClass("book_paragraph_hidden");
    }

    if (!highlight_paragraph) {
        removeCurHighlight();
    }

    if (font_size != cur_font_size) {
        var abs_font_size = 'medium';
        if (font_size == 0) {
            abs_font_size = 'small';
        } else if (font_size == 1) {
            abs_font_size = 'medium';
        } else if (font_size == 2) {
            abs_font_size = 'large';
        } else if (font_size == 3) {
            abs_font_size = 'x-large';
        } else if (font_size == 4) {
            abs_font_size = 'xx-large';
        }

        $("#content_div").css('font-size', abs_font_size);
        cur_font_size = font_size;
    }

    if (color_mode == 0) {
        $("body").removeClass("body-color-white");
        $("#header-id").removeClass("header-color-white");
        $(".header-button").removeClass("header-button-color-white");
    } else {
        $("body").addClass("body-color-white");
        $("#header-id").addClass("header-color-white");
        $(".header-button").addClass("header-button-color-white");
    }
}

function get_par_color(sel) {
    var color;
    if (sel) {
        if (color_mode == 0) {
            color = '#1033FF';
        } else {
            color = '#0000FF';
        }
    } else {
        if (color_mode == 0) {
            color = '#4C3B26';
        } else {
            color = '#000000';
        }
    }
    return color;
}

function smooth_scroll_exec(element, start_top, cur_top, delta, distance, start_time, end_time) {
    //log('scrollTop '+element.scrollTop +' start_top ' + start_top+' distance '+distance);
    if (distance == 0) {
        //log('nothing to scroll');
        return;
    }

    if (element.scrollTop != cur_top) {
        //log('interupted '+element.scrollTop +' ' + cur_top);
        return;
    }

    cur_top = start_top + delta;
    element.scrollTop = cur_top;

    //calculate next step and time;
    // based on http://en.wikipedia.org/wiki/Smoothstep
    var now = Date.now();
    var next_time = now + 10; //milliseconds
    var x;
    while (true) {

        if(next_time <= start_time) {
            next_time = start_time;
            x = 0;
        } else  if(next_time >= end_time) {
            next_time = end_time;
            x = 1;
        } else {
            if (0) {
                //smoth
                x = (next_time - start_time) / (end_time - start_time); // interpolation
                x = x*x*(3 - 2*x);
            } else {
                //linier
                x = (next_time - start_time) / (end_time - start_time);
            }
        }
        delta =  Math.round(distance * x);
        if (delta != 0 || x == 1) {
            break;
        }
        next_time += 10;
    }

    if (cur_top == (start_top + distance)) {
        //log('riched');
        return;
    }

    setTimeout(smooth_scroll_exec, next_time - now, element, start_top, cur_top, delta, distance, start_time, end_time);

}

function smooth_scroll_to(element, end_top, duration) {
    var cur_top = element.scrollTop;
    var start_time = Date.now();
    var end_time = start_time + duration;
    //log('duration = '+duration);
    smooth_scroll_exec(element, cur_top, cur_top, 0, end_top - cur_top, start_time, end_time);
}

function removeCurHighlight() {
    if (curPar >= 0) {
        $('#book_paragraph_' + curPar).css('color', get_par_color(false));
    }
}

function set_cur_par(par) {
    if (highlight_paragraph) {
        removeCurHighlight();
        $('#book_paragraph_' + par).css('color', get_par_color(true));
    }
    curPar = par;
}

function scrol_to_par(par) {
    if (autoscroll_mode > 0){
        var pos = get_scroll_pos(par);
        if (document.getElementById('content_div').scrollTop != pos) {
            if (autoscroll_mode == 1) {
                document.getElementById('content_div').scrollTop = pos;
            } else {
                var delta = 1000;
                var par_duration = (((sound_pos[par+1] - sound_pos[par]) * mp3_duration ) * 10) - delta;
                var time = 1400;
                if (autoscroll_mode == 3) {
                    time = par_duration;
                }
                if (time < delta ||  par_duration < delta) {
                    time = 0;
                }
                if (1) {
                    smooth_scroll_to(document.getElementById('content_div'), pos, time);
                } else {
                    var scrollTop = $('#content_div').scrollTop();
                    $('#content_div').animate({ scrollTop: pos}, time); // swing slow fast
                }
            }
        }
    }
}

function get_scroll_pos(par) {
    var paragraph = '#book_paragraph_' + par;
    var par_top = $(paragraph).position(paragraph).top;
    var scroll_top = $('#content_div').scrollTop();
    var par_heght = $(paragraph).outerHeight();
    var vis_heght = $("#content_div").visibleHeight();
    //log(par+' par_top '+ par_top + ' scroll_top '+scroll_top+' par_heght '+par_heght+' vis_heght '+vis_heght);
    if (par_heght >= vis_heght) {
        return scroll_top + par_top;
    } else {
        return scroll_top + par_top - ((vis_heght - par_heght) /2);
    }
}

function play_paragraph(par) {
    if (change_sound_pos) {
        $("#jquery_jplayer").jPlayer("playHead", sound_pos[par] + 0.001);
        if (($("#jquery_jplayer").data("jPlayer").status.paused)) {
            $("#jquery_jplayer").jPlayer("play");
        }
    }
}

function par_by_sound_pos(pos) {
    var par = -1;
    if (curPar >= 0 && pos >= sound_pos[curPar] && pos < sound_pos[curPar + 1]) {
        par = curPar;
    } else {
        if (pos <= sound_pos[0]) {
            par = 0;
        } else {
            //check next
            if (curPar < sound_pos.length - 2 && pos >= sound_pos[curPar + 1] && pos < sound_pos[curPar + 2]) {
                par = curPar + 1;
            } else {
                for (i = 0; i < sound_pos.length - 1; i++) {
                    if (pos >= sound_pos[i] && pos < sound_pos[i + 1]) {
                        par = i;
                        break;
                    }
                }
            }
        }
    }
    return par;
}

function set_cur_pos(pos) {
    var par = par_by_sound_pos(pos);
    if (par >= 0) {
        set_cur_par(par);
        scrol_to_par(par);
    }
    return par;
}

function setup_jplayer() {
    $("#jquery_jplayer").jPlayer({
        timeupdate : function(event) {
            if (event.jPlayer.status.paused)
                return;
            var pos = event.jPlayer.status.currentPercentAbsolute;
            var par = par_by_sound_pos(pos);
            //log('timeupdate curPar = '+curPar+' par '+par);
            if (curPar != par && par >= 0) {
                if (audioplay_mode == 0 && curPar >=0 ) {
                    $("#jquery_jplayer").jPlayer("pause");
                } else {
                    set_cur_par(par);
                    scrol_to_par(par);
                }
            }
        },
        seeked : function(event) {
            var pos = event.jPlayer.status.currentPercentAbsolute;
            var par = set_cur_pos(pos);
            //log('seeked curPar = '+curPar+' par '+par + ' pos ' + pos);
            if (par >= 0) {
                $("#jquery_jplayer").jPlayer("play");
            }
        },
        play : function(event) {
            //log('play');
            var pos = event.jPlayer.status.currentPercentAbsolute;
            set_cur_pos(pos);
        },
        ended : function(event) {
            if (audioplay_mode == 2) {
                setTimeout(function() {
                        try {
                            parent.postMessage("ended",'*');
                        } catch (e) {
                            //alert('exception1: ' + e);
                            try {
                                parent.show_next_chapter();
                            } catch (e) {
                                //alert('exception2: ' + e);
                            }
                        }
                    }
                    , 100);
            }
        },
        ready : function(event) {
            $(this).jPlayer("setMedia", {
                mp3 : get_sound_path(cur_book_number, cur_chapter),
            });
        },
        durationchange : function(event) {
            if (event.jPlayer.status.duration) {
                mp3_duration = event.jPlayer.status.duration;

                if (play_next_chapter) {
                    play_next_chapter = false;
                    setTimeout(function() {
                        try {
                            $("#jquery_jplayer").jPlayer("play");
                        } catch (e) {
                            //alert('exception: ' + e);
                        }
                    }, 100);
                }
            }
        },
        swfPath: "sound",
        supplied: "mp3",
        wmode: "window",
        volume: 1,
        useStateClassSkin: true,
        autoBlur: false,
        smoothPlayBar: false,
        keyEnabled: true,
        remainingDuration: true,
        toggleDuration: true
    });
};

function get_caption(book_number, chapter) {
    var caption = menu_items[book_number][1] + ' ';
    caption = caption + chapter;
    return caption;
}

function get_sound_path(book_number, chapter) {
    //    ../../sound/Matt/Matt1_e.mp3
    var path = '../../sound/' + menu_items[book_number][0] + "/" + menu_items[book_number][3];
    if (menu_items[book_number][2] > 1) {
        path = path + chapter;
    }
    path = path + "_e.mp3";
    return path;
}

function setup_chapter() {
    var sound_pos_text = $('#sound_pos').val();
    sound_pos = sound_pos_text.split(',');
    for (var i = 0; i < sound_pos.length; i++)
    {
        sound_pos[i] = parseInt(sound_pos[i]);
    }
    var sound_max = sound_pos[sound_pos.length - 1];
    for (var i = 0; i < sound_pos.length; i++)
    {
        sound_pos[i] = (sound_pos[i]*100)/sound_max;
    }
    var book_index = $('#book_index').val();
    book_index = book_index.split(',');
    cur_book_number = book_index[0];
    cur_chapter = book_index[1];

    $("#jp-book-caption").text(get_caption(book_index[0], book_index[1]));
    setup_jplayer();
}

function play_pause() {
    if (($("#jquery_jplayer").data("jPlayer").status.paused)) {
        $("#jquery_jplayer").jPlayer("play");
    } else {
        $("#jquery_jplayer").jPlayer("pause");
    }
}

// jQuery plugins
$.fn.visibleHeight = function() {
    var elBottom, elTop, scrollBot, scrollTop, visibleBottom, visibleTop;
    scrollTop = $(window).scrollTop();
    scrollBot = scrollTop + $(window).height();
    elTop = this.offset().top;
    elBottom = elTop + this.outerHeight();
    visibleTop = elTop < scrollTop ? scrollTop : elTop;
    visibleBottom = elBottom > scrollBot ? scrollBot : elBottom;
    return visibleBottom - visibleTop
}

function TestFunc(a1, a2, a3)
{
		$(".book_paragraph_engl").addClass("book_paragraph_single");
        $(".book_paragraph_engl").removeClass("book_paragraph_hidden");
        $(".book_paragraph_arm").removeClass("book_paragraph_single");
        $(".book_paragraph_arm").addClass("book_paragraph_hidden");
}