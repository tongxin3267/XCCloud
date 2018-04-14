
$(document).ready(function(){



        function createTable(tableId,url){
        //$(function(){
        //清空容器
        //$(tableId).html("");
        $(tableId).datagrid({
            title:'',
            loadMsg:"正在加载，请稍等...",
            striped: true,
            //fit: true,//自动大小
            fitColumns: false,
            nowarp : false,
            url:url,//查看收件箱内容
            columns:[[
                {field:'ID',title:'编号'},
                {field:'MemberId',title:'会员卡号',width:120,sortable:true,searchtype:"number"},
                {field:'CurrentTime',title:'实际时间',width:120,sortable:true},
                {field:'LastSurplus',title:'上期结余',width:200,sortable:true},
                {field:'SaveCoin',title:'存币',width:200,sortable:true},
                {field:'UseCoin',title:'用币',width:200,sortable:true},
                {field:'Kinds',title:'类别',width:200,sortable:true},
                {field:'CurrentSurplus',title:'本次结余',width:200,sortable:true},
                {field:'Note',title:'说明',width:200,sortable:true}
            ]],
            onLoadSuccess: function(){
                $(this).datagrid('freezeRow',0).datagrid('freezeRow',1);
            },
            rownumbers:false,//行号
            singleSelect:false,//是否单选
            showFooter:true,
            pagination:true//分页控件
        });
        $(".panel-tool").css({display:'none'}); //隐藏按钮
        //分页
        var p = $(tableId).datagrid('getPager');
        $(p).pagination({
            pageSize: 10,//每页显示的记录条数，默认为10
            pageList: [5,10,15,20,30,40,50],//可以设置每页记录条数的列表
            beforePageText: '第',//页数文本框前显示的汉字
            afterPageText: '页    共 {pages} 页',
            displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
        });

    }

    $(".memberNav li:nth-child(2)").click(function(){
        createTable('#memberData1-table','json/member.json');
    });
    $(".memberNav li:nth-child(3)").click(function(){
        createTable('#memberData2-table','json/member.json');
    });
    $(".memberNav li:nth-child(4)").click(function(){
        createTable('#memberData3-table','json/member.json');
    });

});
$(function(){
    $('.memberNav').on('click','li',function(){
        $(this).addClass("li_active").siblings().removeClass("li_active")
            .mouseover(function(){$(this).css({backgroundColor:'#eee'})})
            .mouseout(function(){$(this).css({backgroundColor:'#fff'})});
        var _index=$(this).index();
        $(".memberContent>li").eq(_index).css({'display':"block"}).siblings().css({'display':"none"});
    })
});
$(function () {
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $('#memberAll').highcharts({
        chart: {
            type: 'line'
        },
        title: {
            text: '2017年会员增加趋势图'
        },
        credits:{

            enabled:false
        },
        xAxis: {
            categories: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
        },
        yAxis: {
            title: {
                text: '人（k)'
            }
        },
        plotOptions: {
            line: {
                dataLabels: {
                    enabled: true          // 开启数据标签
                },
                enableMouseTracking: false // 关闭鼠标跟踪，对应的提示框、点击事件会失效
            }
        },
        series: [{
            name: '2017年',
            data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
        }, {
            name: '2016年',
            data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
        }]
    });

    function DrawLine(divId){
    $(divId).highcharts({
        chart: {
            type: 'bar'
        },
        title: {
            text: ''
        },
        credits:{

            enabled:false
        },
        xAxis: {
            categories: ['操作'],
            title: {
                text: null
            }
        },
        yAxis: {
            min: 0,
            title: {
                text: '币数 (个)',
                align: 'high'
            },
            labels: {
                overflow: 'justify'
            }
        },
        tooltip: {
            valueSuffix: ' 个'
        },
        plotOptions: {
            bar: {
                dataLabels: {
                    enabled: true,
                    allowOverlap: true
                }
            }
        },
        legend: {
            layout: 'vertical',
            align: 'right',
            verticalAlign: 'top',
            x: -40,
            y: 100,
            floating: true,
            borderWidth: 1,
            backgroundColor: ((Highcharts.theme && Highcharts.theme.legendBackgroundColor) || '#FFFFFF'),
            shadow: true
        },
        credits: {
            enabled: false
        },
        series: [{
            name: '投币量',
            //color:['red'],
            data: [110000]
        }, {
            name: '退币量',
            //color:['red'],
            data: [40000]
        }, {
            name: '币余额',
            //color:['red'],
            data: [30000]
        }, {
            name: '兑币量',
            //color:['red'],
            data: [5000]
        }]
    });
    }
    DrawLine('#memberData1-line');
    DrawLine('#memberData2-line');
    DrawLine('#memberData3-line');
});


