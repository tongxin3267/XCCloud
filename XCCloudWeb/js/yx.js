/**
 * Created by yang on 2017/8/28.
 */

    $(function() {
        var Accordion = function(el, multiple) {
            this.el = el || {};
            this.multiple = multiple || false;

            // Variables privadas
            var links = this.el.find('.link');
            // Evento
            links.on('click', {el: this.el, multiple: this.multiple}, this.dropdown)
        };

        Accordion.prototype.dropdown = function(e) {
            var $el = e.data.el;
            $this = $(this),
                $next = $this.next();

            $next.slideToggle();
            $this.parent().toggleClass('open');

            if (!e.data.multiple) {
                $el.find('.submenu').not($next).slideUp().parent().removeClass('open');
            }
        };

        var accordion = new Accordion($('#accordion'), false);


    });
    $(function () {
        $(".main-menu").mouseleave(function () {
            $(".submenu").fadeOut(1000);
        })
    });
// system overview
// orderSetting,orderAnalysis,orderManage
// IntegralSetting,IntegralAnalysis,IntegralManage
//Token
//digital
//product
//lottery
//console
//ticket
//Sales promotion
//operating
//message
//system
//stores
//accessories

$(function () {
    var arrT = ["test0.html",
        "test1.html","test2.html","test3.html","test4.html","test5.html","test6.html",
        "test7.html","test8.html", "test9.html","test10.html",
        "test11.html","test12.html","test13.html","test14.html","test15.html","test16.html","test17.html","test18.html", "test19.html","test20.html",
        "test50.html",
        "test51.html","test54.html"

    ];
    $(".submenu").find("li").click(function () {
        var ids=$(this).attr("id");
        if(ids){
            //var lengths=$(this).attr("id").length;
            //console.log(lengths);
            var n = $(this).attr("id").substr(4);
            //console.log($(this).attr("id"));
            //console.log(n);
            var tjTitle =$(this).find("a").html();

            var m =arrT[n];
            var txt1=$("<a href='javascript:void(0)'></a>").text(tjTitle);
            var txt2=$('<li id="hShow'+n+'"><em><span class="icon iconfont icon-cha2"></span></em></li>');
            var idN =$("li[id='hShow"+n+"']");
            if(!idN.length>0){
                $(".titleAppend").find("ul").append(txt2);
                $(txt2).find("em").append(txt1);
                $(txt2).addClass("now").siblings().removeClass("now");
                $('iframe').attr('src',m);

            } else {
                idN.addClass("now").siblings().removeClass("now");
                $('iframe').attr('src',m);
            }
        }


    });


    $(".titleAppend ul").on("click","li em a",function () {
        $(this).parents("li").addClass("now").siblings().removeClass("now");
        var titleShow = $(this).parents("li").attr("id").split("hShow")[1];

        var mShow =arrT[titleShow];


        $('iframe').attr('src',mShow);
    });

    $(".titleAppend ul").on("click"," li em span",function () {
        var dd3 =$(this).parents("li").index();
        var dd1 =$(".titleAppend ul li").eq(dd3-1).attr("id").split("hShow")[1];
        $(this).parents("li").remove();
        $('iframe').attr('src',arrT[dd1]);
        $(".titleAppend ul li").eq(dd3).addClass("now").siblings().removeClass("now");


    });
    $(".submenu").on("click","li",function () {
    var liLength = $(".titleAppend li").length;

        if(liLength>10){
            $(".titleAppend ul li").eq(1).remove()
        }
    });

});




$(function () {

    $(".cxBtn").click(function () {
        for (var m = 0; m < $("#bbs").find("li").length; m++) {
            var pdtL = $("#bbs").find("li").eq(m);
            if (pdtL.hasClass("cur") && pdtL.hasClass("numberS")) {
                var numeral1 =pdtL.find(".numeral1").val();

                if (numeral1 == '') {
                    numeral1 = 0;
                }
                var numeral2 =pdtL.find(".numeral2").val();
                if (numeral2 == '') {
                    numeral2 = Infinity;
                }

                if (isNaN(numeral1) || isNaN(numeral2) || parseInt(numeral1) > parseInt(numeral2) || numeral1 == 0 && numeral2 == Infinity) {

                    var txt1 = $("<p class='ts' >请输入数字或者正确的顺序</p>");  // 以 jQuery 创建新元素

                    if (!pdtL.find('p').length > 0) {
                        pdtL.append(txt1);
                        pdtL.find('span').remove();
                    }


                } else {
                    if (pdtL.find('p').length > 0) {
                        pdtL.find('p').remove();
                    }
                    var txtS = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>");  // 以 jQuery 创建新元素
                    var f0 = pdtL.find("em").html();
                    if (!pdtL.find('span').length > 0) {
                        pdtL.append(txtS);
                    }

                    if (numeral2 == Infinity) {
                        numeral2 = "≥";
                        var tx4 = f0 + ":" + numeral2 + numeral1;
                    } else if (numeral1 == 0) {
                        numeral1 = "≤";
                        var tx4 = f0 + ":" + numeral1 + numeral2;
                    } else {
                        var tx4 = f0 + ":" + numeral1 + "-" + numeral2;
                    }


                    var txt3 = $("<a id='condition"+m+"'><span class='icon iconfont icon-cha'></span></a>");
                    var idM =$("a[id='condition"+m+"']");

                    if (!idM.length > 0) {

                        $(".zWrite").append(txt3);

                        $(txt3).append(tx4);

                    } else {
                        idM.remove();
                        $(".zWrite").append(txt3);

                        $(txt3).append(tx4);
                    }
                }






            }

            if (pdtL.hasClass("cur") && pdtL.hasClass("timeS")){
                var numeral3 =pdtL.find(".numeral3").val();
                var numeral4 =pdtL.find(".numeral4").val();

                if(numeral3 == ''|| numeral4 == '') {
                    if (pdtL.find('p').length > 0) {
                        pdtL.find('p').remove();
                    }
                    var sj2 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>");  // 以 jQuery 创建新元素

                    if (!pdtL.find('span').length > 0) {
                        pdtL.append(sj2);
                    }

                    var f1 = pdtL.find("em").html();
                    if (numeral3 == "" && numeral4 == "") {
                        var sj3 = "今天"
                    } else if (numeral3 == "") {
                        numeral3 = "以前";
                        var sj3 = f1 + ":" + numeral4 + numeral3;
                    } else if (numeral4 == "") {
                        numeral4 = "以后";
                        var sj3 = f1 + ":" + numeral3 + numeral4;
                    }


                    var txt4 = $("<a id='condition"+m+"'><span class='icon iconfont icon-cha'></span></a>");
                    var idM =$("a[id='condition"+m+"']");

                    if (!idM.length > 0) {

                        $(".zWrite").append(txt4);

                        $(txt4).append(sj3);

                    } else {
                        idM.remove();
                        $(".zWrite").append(txt4);

                        $(txt4).append(sj3);
                    }
                }else {

                    var start=numeral3.replace("-", "").replace("-", "").replace(":", "").replace(":", "");

                    var end=numeral4.replace("-", "").replace("-", "").replace(":", "").replace(":", "");
                    console.log(start);
                    console.log(end);
                    if(start>end ){
                        var sj1=$("<p  class='ts'>请输入正确的时间区间</p>");  // 以 jQuery 创建新元素

                        if (!pdtL.find('p').length > 0) {
                            pdtL.append(sj1);
                            pdtL.find('span').remove();
                        }
                    }else {
                        if (pdtL.find('p').length > 0) {
                            pdtL.find('p').remove();
                        }
                        var sj2 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>");  // 以 jQuery 创建新元素

                        if (!pdtL.find('span').length > 0) {
                            pdtL.append(sj2 );
                        }

                        var txt4 = $("<a id='condition"+m+"'><span class='icon iconfont icon-cha'></span></a>");
                        var idM =$("a[id='condition"+m+"']");

                        var f1 = pdtL.find("em").html();
                        var sj3 = f1 + ":" + numeral3+"-" + numeral4;

                        if (!idM.length > 0) {

                            $(".zWrite").append(txt4);

                            $(txt4).append(sj3);

                        } else {
                            idM.remove();
                            $(".zWrite").append(txt4);

                            $(txt4).append(sj3);
                        }
                    }
                }
            }
            if(pdtL.hasClass("cur") && pdtL.hasClass("literalsS")){
                var numeral5 =pdtL.find(".numeral5").val();

                var f2 = pdtL.find("em").html();

                var txt5=f2+":"+numeral5;

                var txtL5 = $("<a id='condition"+m+"'><span class='icon iconfont icon-cha'></span></a>");
                var idM =$("a[id='condition"+m+"']");
                if (!idM.length > 0) {

                    $(".zWrite").append(txtL5);

                    $(txtL5).append(txt5);

                } else {
                    idM.remove();
                    $(".zWrite").append(txtL5);

                    $(txtL5).append(txt5);
                }

            }
            if (pdtL.hasClass("cur") && pdtL.hasClass("time2S")){
                var numeral6 =pdtL.find(".numeral6").val();
                var numeral7 =pdtL.find(".numeral7").val();

                if(numeral6 == ''|| numeral7 == '') {
                    if (pdtL.find('p').length > 0) {
                        pdtL.find('p').remove();
                    }
                    var sj6 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>");  // 以 jQuery 创建新元素

                    if (!pdtL.find('span').length > 0) {
                        pdtL.append(sj6);
                    }

                    var f1 = pdtL.find("em").html();
                    if (numeral6 == "" && numeral7 == "") {
                        var sj7 = "今天"
                    } else if (numeral6 == "") {
                        numeral6 = "以前";
                        var sj7 = f1 + ":" + numeral7 + numeral6;
                    } else if (numeral7 == "") {
                        numeral7 = "以后";
                        var sj7 = f1 + ":" + numeral6 + numeral7;
                    }


                    var txt6 = $("<a id='condition"+m+"'><span class='icon iconfont icon-cha'></span></a>");
                    var idM =$("a[id='condition"+m+"']");

                    if (!idM.length > 0) {

                        $(".zWrite").append(txt6);

                        $(txt6).append(sj7);

                    } else {
                        idM.remove();
                        $(".zWrite").append(txt6);

                        $(txt6).append(sj7);
                    }
                }else {

                    var start=numeral6.replace("-", "");

                    var end=numeral7.replace("-", "");

                    if(start>end ){
                        var sj1=$("<p  class='ts'>请输入正确的时间区间</p>");  // 以 jQuery 创建新元素

                        if (!pdtL.find('p').length > 0) {
                            pdtL.append(sj1);
                            pdtL.find('span').remove();
                        }
                    }else {
                        if (pdtL.find('p').length > 0) {
                            pdtL.find('p').remove();
                        }
                        var sj6 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>");  // 以 jQuery 创建新元素

                        if (!pdtL.find('span').length > 0) {
                            pdtL.append(sj6 );
                        }

                        var txt6 = $("<a id='condition"+m+"'><span class='icon iconfont icon-cha'></span></a>");
                        var idM =$("a[id='condition"+m+"']");

                        var f1 = pdtL.find("em").html();
                        var sj7 = f1 + ":" + numeral6+"-" + numeral7;

                        if (!idM.length > 0) {

                            $(".zWrite").append(txt6);

                            $(txt6).append(sj7);

                        } else {
                            idM.remove();
                            $(".zWrite").append(txt6);

                            $(txt6).append(sj7);
                        }
                    }
                }
            }
            if(!pdtL.hasClass("cur")){
                $("a[id='condition"+m+"']").remove();
            }
        }

    });



    $(".zWrite").on("click","a span",function(){
        $(this).parents("a").remove();
    });
    $(".cxBtn").on("click", function () {

        if(!$("#bbs").find("p").hasClass("ts")){

            $(".reveal-modal-bg").click()
        }
    });


    $(".ms-res-item").live('click',function(){
        $(".reveal-modal").click();
    });
    $(".ms-close-btn").live('click',function(){
        $(".reveal-modal").click();
    });



    $(".gbBtn").on("click", function () {
        $(".reveal-modal-bg").click();
    });
});


$(function () {
        $('.label1 em').toggle(function () {
            $('.label1 ul').stop(true,true).slideDown(400);

        },function () {
            $('.label1 ul').stop(true,true).slideUp(250);
        });
        $(document).click(function(){
            $(".stepX").find("ul").hide();
        });

});

