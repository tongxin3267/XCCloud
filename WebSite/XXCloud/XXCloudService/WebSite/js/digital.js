/**
 * Created by Administrator on 2017-08-30.
 */
$(function () {
    // Create the chart
    Highcharts.chart('moneyOrder', {
        chart: {
            type: 'column'
        },
        credits:{
            text:"武汉莘宸科技",
            href:"",
            enabled:false
        },
        title: {
            text: '2018年8月28日，各站点实币排行'
        },
        subtitle: {
            text: '点击查看数据详情: <a href="https://netmarketshare.com">武汉莘宸科技</a>.'
        },
        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: '实币数'
            }
        },
        legend: {
            enabled: false
            //    verticalAlign:center
        },

        stacking:"percent",
        plotOptions: {
            series: {
                borderWidth: 0,
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    stacking:"percent",
                    format: '{point.y:.1f}</b> 元<br/>'
                },
                point:{
                    events: {
                        select: function() {
                            var text = this.name;
                            console.log(text);
                            switch (text){
                                case "1号吧台": createTable('aaa.json');  //页面跳转
                                    break;
                                case "2号吧台": createTable('aaa1.json');
                                    break;
                                case "3号吧台": createTable('aaa2.json');
                                    break;
                                case "4号吧台": createTable('aaa.json');
                                    break;
                                case "5号吧台": createTable();
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
            name: '站点名',
            colorByPoint: true,
            data: [ {
                name: '1号吧台',
                y: 2600
            }, {
                name: '2号吧台',
                y: 2500

            }, {
                name: '3号吧台',
                y: 2450

            }, {
                name: '4号吧台',
                y: 2400
            }, {
                name: '5号吧台',
                y: 2300
            },
                {
                    name: '合计',
                    y: 10000
                }
            ]
        }]

    });


    function createTable(url){
        //$(function(){
            //清空容器
            $('#newTable').html("");
            $('#newTable').datagrid({
                title:'',
                loadMsg:"正在加载，请稍等...",
                striped: true,
                fit: true,//自动大小
                fitColumns: true,
                url:url,//查看收件箱内容
                columns:[[
                    //{field:'ID',title:'编号',checkbox:true},
                    {field:'ID',title:'编号'},
                    {field:'SellTime',title:'时间',width:120,sortable:true,searchtype:"number"},
                    {field:'Events',title:'事件',width:120,sortable:true},
                    {field:'LastSurplus',title:'上期结余',width:200,sortable:true},
                    {field:'digitalCount',title:'数量',width:200,sortable:true},
                    {field:'CurrentSurplus',title:'本期结余',width:200,sortable:true},
                    {field:'Operator',title:'经办人',width:200,sortable:true},
                    {field:'note',title:'备注',width:200,sortable:true}

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
            var p = $('#newTable').datagrid('getPager');
            $(p).pagination({
                pageSize: 10,//每页显示的记录条数，默认为10
                pageList: [5,10,15,20,30,40,50],//可以设置每页记录条数的列表
                beforePageText: '第',//页数文本框前显示的汉字
                afterPageText: '页    共 {pages} 页',
                displayMsg: '当前显示 {from} - {to} 条记录   共 {total} 条记录'
            });

    //})
}
    createTable('aaa.json');
});
