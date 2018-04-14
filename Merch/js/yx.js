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
        links.on('click', { el: this.el, multiple: this.multiple }, this.dropdown)
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
$(function() {
    $(".main-menu").mouseleave(function() {
        $(".submenu").hide();

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



function doApi(parasObj, id, url) {

    var parasJson = JSON.stringify(parasObj);
    $.ajax({
        type: "post",
        url: '/query?action=init',
        contentType: "application/json; charset=utf-8",
        data: { parasJson: parasJson },

        success: function(data) {
            data = JSON.parse(data);
            //console.log(1);
            //获取表结构
            var columns = [];
            for (var i = 0; i < data.result_data.length; i++) {
                if (data.result_data[i].iscolume == "1") {
                    columns.push({
                        field: data.result_data[i].field,
                        title: data.result_data[i].title,
                        width: data.result_data[i].width,
                        hidden: false
                    });
                } else {
                    columns.push({
                        field: data.result_data[i].field,
                        title: data.result_data[i].title,
                        width: data.result_data[i].width,
                        hidden: true
                    });
                }
            }
            //console.log(columns);
            //清空容器
            //function (){
            // $(id).html("");
            $(id).datagrid({
                title: '',
                align:'center',
                loadMsg: "正在加载，请稍等...",
                striped: true,
                fit: true, //自动大小
                fitColumns: false,
                //function:  console.log(url),
                url: url, //表格数值
                columns: [columns],
                onLoadSuccess: function() {
                    $(this).datagrid('freezeRow', 0).datagrid('freezeRow', 1);

                },
                rownumbers: false, //行号
                singleSelect: false, //是否单选
                showFooter: true,
                pagination: true, //分页控件
                //toolbar: ["-", {
                //    id: 'aaa',
                //    text: '更改表格',
                //    iconCls: 'icon-edit',
                //    handler: function() {
                //        $(".crudTable").css({ display: "block" });
                //    }
                //}]
            });

            $(".panel-tool").css({ display: 'none' }); //隐藏按钮

            $(".panel-body").css({ padding: '0' });
            $(".datagrid-header-row td").css({textAlign:'center'});
            //分页
            var p = $(id).datagrid('getPager');
            $(p).pagination({
                pageSize: 10, //每页显示的记录条数，默认为10
                pageList: [5, 10, 15, 20, 30, 40, 50], //可以设置每页记录条数的列表
                beforePageText: '第', //页数文本框前显示的汉字
                afterPageText: '页    共 {pages} 页',
                displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
            });
            //}


            var jsonData = [];
            var arrFiled = [];
            for (var i = 0; i < data.result_data.length; i++) {
                jsonData.push({
                    id: i,
                    name: data.result_data[i].title,
                    type: data.result_data[i].type,
                    list: data.result_data[i].list,
                    iscolume: data.result_data[i].iscolume,
                    issearch: data.result_data[i].issearch
                });
                arrFiled.push(data.result_data[i].field);
            }
            var ms2 = $('#ms2').magicSuggest({
                data: jsonData,
                maxSelection: 100,
                editable: false
            });
            var temp1 = $(".number").html();
            var temp2 = $(".time1").html();

            var temp4 = $(".time2").html();
            $(".number").remove();
            $(".time1").remove();
            $(".time2").remove();

            for (var j = 0; j < jsonData.length; j++) {

                var txtL1 = $('<li class="bbsHidden""><em></em></li>');
                var txtL2 = jsonData[j].name;

                $(txtL1).find("em").append(txtL2);
                $("#bbs").append(txtL1);
                var txtLB1 = $('<li><i class="icon iconfont icon-gouSolid-copy"></i></li>');
                $(txtLB1).append(txtL2);
                $("#label").find("ul").append(txtLB1);
                if (jsonData[j].type == "number") {
                    $("#bbs").find("li").eq(j).addClass("numberS").append(temp1);
                }

                if (jsonData[j].type == "datetime") {
                    $("#bbs").find("li").eq(j).addClass("timeS").append(temp2);
                }
                if (jsonData[j].type == "string") {
                    var optionList = jsonData[j].list;
                    var temp3 = $(".literals").html();
                    for (var k = 0; k < optionList.length; k++) {
                        var optionVal = optionList[k];
                        var optionTxt = $('<option value=""></option>');
                        $(optionTxt).val(optionVal);
                        $(optionTxt).append(optionVal);
                        $(".optionL").append(optionTxt);
                    }
                    $("#inputL ").attr("value", optionList[0]);
                    $("#bbs").find("li").eq(j).addClass("literalsS").append(temp3);



                    $('.optionL').change(function() {
                        var ssss = $(this).val();
                        $("#inputL ").attr("value", ssss);
                    });
                }

                if (jsonData[j].type == "readCard") {
                    $("#bbs").find("li").eq(j).addClass("readCard").append(temp5);
                }

                if (jsonData[j].type == "date") {
                    $("#bbs").find("li").eq(j).addClass("time2S").append(temp4);
                }
                if (jsonData[j].issearch == "1") {
                    $(".big-link").one("click", function() {
                        $("#ms-input-0").trigger("click");
                        $(".ms-res-item").trigger("click");

                    });


                }
                if (jsonData[j].iscolume == "1") {
                    $("#label").find("li").eq(j).addClass("active");
                }

            }

            //$(".label1 li").click(function () {
            //    var _index=$(this).index();
            //    console.log(_index);
            //    if($(this).hasClass("active")){
            //
            //        $(id).datagrid("hideColumn",arrFiled[_index]);
            //        console.log(id)
            //        console.log(_index);
            //        $(this).removeClass("active");
            //
            //
            //    }else {
            //        $(id).datagrid('showColumn', arrFiled[_index]);
            //        $(this).addClass("active");
            //
            //    }
            //});

            $(".label1 li").click(function() {

                var _index = $(this).index();
                if ($(this).hasClass("active")) {
                    $(id).datagrid("hideColumn", arrFiled[_index]);
                    $(this).removeClass("active");
                } else {
                    $(this).addClass("active");
                    $(id).datagrid('showColumn', arrFiled[_index]);


                }
            });

        },

        error: function(error) {


        }
    });
}







$(function() {
    var arrT = ["home.html",
        "test1.html", "test2.html", "test3.html", "test4.html", "test5.html", "test6.html",
        "gameCurrency.html", "tokenStorage.html", "tokenDestory.html", "tokenSell.html",
        "tokenInstall.html", "tokenInventory.html", "digitalCurrencyDestroy.html", "digitalCurrencyInventory.html", "digitalCurrencySell.html",
        "digitalCurrencyInstall.html", "productRecord.html", "barBusiness.html",
        "member.html", "packageSell.html","cardSales.html","transferMoney.html",
        "gameSetting.html","ticketRuleSet.html","ticketAnalysis.html","ticketUsing.html",
        "discountPackageSet.html","discountCoinOperatedRule.html","discountGiveTimes.html","discountElectronicCoupons.html",
        "discountPrizeRule.html","discountRefundRule.html"

    ];
    $(".submenu").find("li").click(function() {
        var ids = $(this).attr("id");
        if (ids) {
            //var lengths=$(this).attr("id").length;
            //console.log(lengths);
            var n = $(this).attr("id").substr(4);
            //console.log($(this).attr("id"));
            //console.log(n);
            var tjTitle = $(this).find("a").html();

            var m = arrT[n];
            var txt1 = $("<a href='javascript:void(0)'></a>").text(tjTitle);
            var txt2 = $('<li id="hShow' + n + '"><em><span class="icon iconfont icon-cha2"></span></em></li>');
            var idN = $("li[id='hShow" + n + "']");
            if (!idN.length > 0) {
                $(".titleAppend").find("ul").append(txt2);
                $(txt2).find("em").append(txt1);
                $(txt2).addClass("now").siblings().removeClass("now");
                $('.test2').find('iframe').hide();
                var iFrame = $('<iframe id="iFrame' + n + '"  width="100%" height="100%" frameborder="0" seamless></iframe>').attr('src', m);
                $('.test2').append(iFrame);
            } else {
                idN.addClass("now").siblings().removeClass("now");
                $("iframe[id='iFrame" + n + "']").show().siblings().hide()
            }
        }


    });


    $(".titleAppend ul").on("click", "li em a", function() {
        $(this).parents("li").addClass("now").siblings().removeClass("now");
        var titleShow = $(this).parents("li").attr("id").split("hShow")[1];


        $("iframe[id='iFrame" + titleShow + "']").show().siblings().hide();
    });

    $(".titleAppend ul").on("click", " li em span", function() {
        var dd3 = $(this).parents("li").index();
        var dd1 = $(".titleAppend ul li").eq(dd3 - 1).attr("id").split("hShow")[1];
        $(this).parents("li").remove();
        $("iframe[id='iFrame" + dd1 + "']").show().siblings().hide();
        $(".titleAppend ul li").eq(dd3 - 1).addClass("now").siblings().removeClass("now");


    });
    $(".submenu").on("click", "li", function() {
        var liLength = $(".titleAppend li").length;

        if (liLength > 10) {
            $(".titleAppend ul li").eq(1).remove()
        }
    });

});




$(function() {

    $(".cxBtn").click(function() {
        for (var m = 0; m < $("#bbs").find("li").length; m++) {
            var pdtL = $("#bbs").find("li").eq(m);
            if (pdtL.hasClass("cur") && pdtL.hasClass("numberS")) {
                var numeral1 = pdtL.find(".numeral1").val();

                if (numeral1 == '') {
                    numeral1 = 0;
                }
                var numeral2 = pdtL.find(".numeral2").val();
                if (numeral2 == '') {
                    numeral2 = Infinity;
                }

                if (isNaN(numeral1) || isNaN(numeral2) || parseInt(numeral1) > parseInt(numeral2) || numeral1 == 0 && numeral2 == Infinity) {

                    var txt1 = $("<p class='ts' >请输入数字或者正确的顺序</p>"); // 以 jQuery 创建新元素

                    if (!pdtL.find('p').length > 0) {
                        pdtL.append(txt1);
                        pdtL.find('span').remove();
                    }


                } else {
                    if (pdtL.find('p').length > 0) {
                        pdtL.find('p').remove();
                    }
                    var txtS = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>"); // 以 jQuery 创建新元素
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


                    var txt3 = $("<a id='condition" + m + "'><span class='icon iconfont icon-cha'></span></a>");
                    var idM = $("a[id='condition" + m + "']");

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

            if (pdtL.hasClass("cur") && pdtL.hasClass("timeS")) {
                var numeral3 = pdtL.find(".numeral3").val();
                var numeral4 = pdtL.find(".numeral4").val();

                if (numeral3 == '' || numeral4 == '') {
                    if (pdtL.find('p').length > 0) {
                        pdtL.find('p').remove();
                    }
                    var sj2 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>"); // 以 jQuery 创建新元素

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


                    var txt4 = $("<a id='condition" + m + "'><span class='icon iconfont icon-cha'></span></a>");
                    var idM = $("a[id='condition" + m + "']");

                    if (!idM.length > 0) {

                        $(".zWrite").append(txt4);

                        $(txt4).append(sj3);

                    } else {
                        idM.remove();
                        $(".zWrite").append(txt4);

                        $(txt4).append(sj3);
                    }
                } else {

                    var start = numeral3.replace("-", "").replace("-", "").replace(":", "").replace(":", "");

                    var end = numeral4.replace("-", "").replace("-", "").replace(":", "").replace(":", "");
                    console.log(start);
                    console.log(end);
                    if (start > end) {
                        var sj1 = $("<p  class='ts'>请输入正确的时间区间</p>"); // 以 jQuery 创建新元素

                        if (!pdtL.find('p').length > 0) {
                            pdtL.append(sj1);
                            pdtL.find('span').remove();
                        }
                    } else {
                        if (pdtL.find('p').length > 0) {
                            pdtL.find('p').remove();
                        }
                        var sj2 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>"); // 以 jQuery 创建新元素

                        if (!pdtL.find('span').length > 0) {
                            pdtL.append(sj2);
                        }

                        var txt4 = $("<a id='condition" + m + "'><span class='icon iconfont icon-cha'></span></a>");
                        var idM = $("a[id='condition" + m + "']");

                        var f1 = pdtL.find("em").html();
                        var sj3 = f1 + ":" + numeral3 + "-" + numeral4;

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
            if (pdtL.hasClass("cur") && pdtL.hasClass("literalsS")) {
                var numeral5 = pdtL.find(".numeral5").val();

                var f2 = pdtL.find("em").html();

                var txt5 = f2 + ":" + numeral5;

                var txtL5 = $("<a id='condition" + m + "'><span class='icon iconfont icon-cha'></span></a>");
                var idM = $("a[id='condition" + m + "']");
                if (!idM.length > 0) {

                    $(".zWrite").append(txtL5);

                    $(txtL5).append(txt5);

                } else {
                    idM.remove();
                    $(".zWrite").append(txtL5);

                    $(txtL5).append(txt5);
                }

            }
            if (pdtL.hasClass("cur") && pdtL.hasClass("time2S")) {
                var numeral6 = pdtL.find(".numeral6").val();
                var numeral7 = pdtL.find(".numeral7").val();

                if (numeral6 == '' || numeral7 == '') {
                    if (pdtL.find('p').length > 0) {
                        pdtL.find('p').remove();
                    }
                    var sj6 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>"); // 以 jQuery 创建新元素

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


                    var txt6 = $("<a id='condition" + m + "'><span class='icon iconfont icon-cha'></span></a>");
                    var idM = $("a[id='condition" + m + "']");

                    if (!idM.length > 0) {

                        $(".zWrite").append(txt6);

                        $(txt6).append(sj7);

                    } else {
                        idM.remove();
                        $(".zWrite").append(txt6);

                        $(txt6).append(sj7);
                    }
                } else {

                    var start = numeral6.replace("-", "");

                    var end = numeral7.replace("-", "");

                    if (start > end) {
                        var sj1 = $("<p  class='ts'>请输入正确的时间区间</p>"); // 以 jQuery 创建新元素

                        if (!pdtL.find('p').length > 0) {
                            pdtL.append(sj1);
                            pdtL.find('span').remove();
                        }
                    } else {
                        if (pdtL.find('p').length > 0) {
                            pdtL.find('p').remove();
                        }
                        var sj6 = $("<span class='Correct icon iconfont icon-gouSolid-copy'></span>"); // 以 jQuery 创建新元素

                        if (!pdtL.find('span').length > 0) {
                            pdtL.append(sj6);
                        }

                        var txt6 = $("<a id='condition" + m + "'><span class='icon iconfont icon-cha'></span></a>");
                        var idM = $("a[id='condition" + m + "']");

                        var f1 = pdtL.find("em").html();
                        var sj7 = f1 + ":" + numeral6 + "-" + numeral7;

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
            if (!pdtL.hasClass("cur")) {
                $("a[id='condition" + m + "']").remove();
            }
        }

    });



    $(".zWrite").on("click", "a span", function() {
        $(this).parents("a").remove();
    });
    $(".cxBtn").on("click", function() {
        //if(!$("#bbs").find("p").hasClass("ts")){
        $(".reveal-modal-bg").click()
        //}
    });


    $(".ms-res-item").live('click', function() {
        $(".reveal-modal").click();
    });
    $(".ms-close-btn").live('click', function() {
        $(".reveal-modal").click();
    });



    $(".gbBtn").on("click", function() {
        $(".reveal-modal-bg").click();
    });
});


$(function() {
    $('.label1 em').toggle(function() {
        $('.label1 ul').stop(true, true).slideDown(400);

    }, function() {
        $('.label1 ul').stop(true, true).slideUp(250);
    });
    $(document).click(function() {
        $(".stepX").find("ul").hide();
    });

});


 $(function(){

        $(".crudTable").click(function(event){
            if(event.target==this){
             
          $(this).css({display:"none"});
        }
    });
});

//数字键盘
function loadNumberKeyboard(obj) {
    if ($("#numberkeyboard").length == 0) {

        $("#numberkeyboard").find(".numbtn").bind("mousedown",
            function() {
                $(this).addClass("numbtndown");
            });
        $("#numberkeyboard").find(".numbtn").bind("mouseup",
            function() {
                $(this).removeClass("numbtndown");
            });
    }
    var containerDiv = $("#numberkeyboard").parent();
    containerDiv.show();
    containerDiv.css("z-index", 9100);
    obj = $(obj);
    if (obj.attr("id")) {
        var objpadding = parseInt(obj.css("padding-top").replace("px", "")) + parseInt(obj.css("padding-bottom").replace("px", ""));
        if (obj.offset().left + 340 >= $(window).width()) {
            containerDiv.css("left", $(window).width() - 340);
        } else {
            containerDiv.css("left", obj.offset().left);
        }
        if (obj.offset().top + 291 + obj.height() + objpadding + 2 + 5 >= $(window).height() + $(window).scrollTop()) {
            containerDiv.css("top", obj.offset().top - 291 - 5);
        } else {
            containerDiv.css("top", obj.offset().top + obj.height() + objpadding + 2 + 5);
        }
    }
    $("#numberkeyboard").find(".numbtn").unbind("click");
    $("#numberkeyboard").find(".numbtn").bind("click",
        function() {
            obj.focus();
            var key = $(this).attr("key");
            switch (key) {
                case "backspace":
                    if (obj.val().length > 0) {
                        obj.val(obj.val().substr(0, obj.val().length - 1));
                    }
                    break;
                case "clear":
                    obj.val("");
                    break;
                case "sign":
                    if (obj.val().length > 0) {
                        if (obj.val().substr(0, 1) == "-") {
                            obj.val(obj.val().substr(1, obj.val().length - 1));
                        } else {
                            obj.val("-" + obj.val());
                        }
                    }
                    break;
                case "close":
                    $("#numberkeyboard").find(".numbtn").unbind("click");
                    //$("#numberkeyboard").hide();
                    break;
                case "none":
                    $("#numberkeyboard").css({display:"none"});
                    break;
                default:
                    obj.val(obj.val() + key);
                    break;
            }
        });
}

$(function(){
    $(".changeBtn").click(function(){
        $(".crudTable").css({ display: "block" });
    });
});
