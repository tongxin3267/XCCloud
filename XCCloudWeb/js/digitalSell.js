
$(function () {
    // Create the chart
    Highcharts.chart('histogramOrder', {
        chart: {
            type: 'column'
        },
        credits:{
            text:"武汉莘宸科技",
            href:"",
            enabled:false
        },
        title: {
            text: '数字币销售情况分析'
        },

        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: '销售总份数'
            }
        },
        legend: {
            enabled: false
            //    verticalAlign:center
        },
        plotOptions: {
            series: {
                borderWidth: 0,
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '{point.y:.1f}</b> 份<br/>'
                },
                point:{
                    events: {
                        select: function() {
                            var text = this.name;
                            switch (text){
                                case "50元":    createTable('tokenSell.json'); //页面跳转
                                    break;
                                case "100元":   createTable('tokenSell.json');
                                    break;
                                case "200元":   createTable('tokenSell.json');
                                    break;
                                case "300元":   createTable('aaa.json');
                                    break;
                                case "500元":   createTable('aaa.json');
                                    break;
                            }
                        }
                    }
                }
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span style="color:{point.color}">{point.name}</span>: <b>{point.y:.1f}</b>'
        },
        //tooltip: {
        //    headerFormat: '{series.name}<br>',
        //    pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
        //},
        series: [{
            name: '套餐名',
            colorByPoint: true,
            data: [ {
                name: '50元',
                y: 600
            }, {
                name: '100元',
                y: 500

            }, {
                name: '200元',
                y: 450

            }, {
                name: '300元',
                y: 400
            }, {
                name: '500元',
                y: 300
            }
            ]
        }]

    });

    function  getData(url){
        $.get(
            url,{
            },function(data,state) {
                var jsonData = [];
                var arrFiled=[];
                for (var i = 0; i < data.length; i++) {
                    jsonData.push({
                        id: i,
                        name: data[i].title,
                        type: data[i].type,
                        list: data[i].list,
                        iscolume: data[i].iscolume,
                        issearch: data[i].issearch
                    });
                    arrFiled.push(data[i].field);

                }
                console.log(arrFiled);
                var ms2 = $('#ms2').magicSuggest({
                    data: jsonData,
                    maxSelection: 100,
                    editable: false
                });


                var temp1=$(".number").html();
                var temp2=$(".time1").html();

                var temp4=$(".time2").html();
                $(".number").remove();
                $(".time1").remove();
                $(".time2").remove();

                for (var j = 0; j<jsonData.length;j++){

                    var txtL1=$('<li class="bbsHidden""><em></em></li>');
                    var txtL2=jsonData[j].name;

                    $(txtL1).find("em").append(txtL2);
                    $("#bbs").append(txtL1);
                    var txtLB1 =$('<li><i class="icon iconfont icon-gouSolid-copy"></i></li>');
                    $(txtLB1).append(txtL2);
                    $("#label").find("ul").append(txtLB1);
                    if(jsonData[j].type == "number"){
                        $("#bbs").find("li").eq(j).addClass("numberS").append(temp1);
                    }

                    if(jsonData[j].type == "time") {
                        $("#bbs").find("li").eq(j).addClass("timeS").append(temp2);
                    }
                    if(jsonData[j].type == "literals") {
                        var optionList = jsonData[j].list;

                        for (var k = 0; k < optionList.length; k++) {

                            var optionVal = optionList[k];
                            var optionTxt = $('<option value=""></option>');
                            $(optionTxt).val(optionVal);
                            $(optionTxt).append(optionVal);
                            $(".optionL").append(optionTxt);
                            $("#inputL ").attr("value", optionList[0]);
                        }
                        var temp3 = $(".literals").html();

                        $("#bbs").find("li").eq(j).addClass("literalsS").append(temp3);
                    }

                    if(jsonData[j].type == "time2"){
                        $("#bbs").find("li").eq(j).addClass("time2S").append(temp4);
                    }
                    if(jsonData[j].iscolume == "1"){
                        $(".big-link").one("click",function () {
                            $("#ms-input-0").trigger("click");
                            $(".ms-res-item").trigger("click");

                        });


                    }
                    if(jsonData[j].issearch == "1"){
                        $("#label").find("li").eq(j).addClass("active");
                        console.log(j)

                    }

                }

                $(".label1 li").click(function () {

                    var _index=$(this).index();
                    if($(this).hasClass("active")){
                        $("#digitalSellTable").datagrid("hideColumn",arrFiled[_index]);
                        $(this).removeClass("active");
                    }else {
                        $(this).addClass("active");
                        $("#digitalSellTable").datagrid('showColumn', arrFiled[_index]);


                    }
                });

            });
    }

    getData("json/dataDigitalInventory.json")

});

$(function(){

    $.get('json/dataDigitalInventory.json',{},function(data,state){
        var columns=[];
        for (var i = 0; i < data.length; i++) {
            if(data[i].issearch=="1"){
                columns.push({
                    field: data[i].field,
                    title: data[i].title,
                    width: data[i].width,
                    hidden:false
                });
            }else {
                columns.push({
                    field: data[i].field,
                    title: data[i].title,
                    width: data[i].width,
                    hidden:true
                });
            }
        }
//            columns.push({
//                field: data[data.length-1].field,
//                title: data[data.length-1].title,
//                width: data[data.length-1].width,
//                hidden:true,
//                fitColumns: true
//            });
        console.log(columns);
        createTable('json/digitalSell1.json',columns);

    });



    function createTable(url,columns){
        //$(function(){
        //清空容器
        $('#digitalSellTable').html("");
        $('#digitalSellTable').datagrid({
            title:'',
            loadMsg:"正在加载，请稍等...",
            striped: true,
            //resizable:true,
            fit: true,//自动大小
            fitColumns: false,
            url:url,//查看收件箱内容
            columns:[columns],
            onLoadSuccess: function(){
                $(this).datagrid('freezeRow',0).datagrid('freezeRow',1);
                var table = $(this).prev().find('table'), posDivs = table.eq(0).find('div.datagrid-cell')//表头用来定位用的div
                    , bodyFirstDivs1 = table.eq(1).find('tr:eq(0) div')//内容第一行用来设置宽度的div，以便设置冻结列和表头一样的宽度
                    ,bodyFirstDivs2 = table.eq(2).find('tr:eq(0) div')    //          设置没冻结列的宽度
                    ,footerDivs=$(".datagrid-footer").find("table").eq(1).find('tr:eq(0) div')     //设置footer的宽度
                    , orderHeader = posDivs.map(function (index) { return { index: index, left: $(this).position().left} }); //计算表头的左边位置，以便重新排序和内容行单元格循序一致
                orderHeader.sort(function (a, b) { return a.left - b.left; });
                console.log(footerDivs);
                //console.log(bodyFirstDivs);
                //对表头位置排序
                setTimeout(function () {//延时设置宽度，因为easyui执行完毕回调后有后续的处理，会去掉内容行用来设置宽度的div的css width属性
                    for (var i = 0; i < orderHeader.length; i++) {
                        bodyFirstDivs1.eq(i).css('width', posDivs.eq(orderHeader[i].index).css('width'));
                        bodyFirstDivs2.eq(i).css('width', posDivs.eq(orderHeader[i].index).css('width'));
                        footerDivs.eq(i).css('width', posDivs.eq(orderHeader[i].index).css('width'));
                    }
                }, 50);
            },

            rownumbers:false,//行号
            singleSelect:false,//是否单选
            showFooter:true,
            pagination:true//分页控件
        });
        $(".panel-tool").css({display:'none'}); //隐藏按钮
        //分页
        var p = $('#digitalSellTable').datagrid('getPager');
        $(p).pagination({
            pageSize: 10,//每页显示的记录条数，默认为10
            pageList: [5,10,15,20,30,40,50],//可以设置每页记录条数的列表
            beforePageText: '第',//页数文本框前显示的汉字
            afterPageText: '页    共 {pages} 页',
            displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
        });
    }
    //createTable('json/digitalSell1.json');

});