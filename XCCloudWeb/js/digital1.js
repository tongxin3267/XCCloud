/**
 * Created by Administrator on 2017-08-30.
 */
$(function () {
    // Create the chart
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    Highcharts.chart('moneyOrder', {
        chart: {
            type: 'column'
        },
        credits:{
            text:"武汉莘宸科技",
            href:"#",
            enabled:false
        },
        title: {
            text: '套餐销售情况分析'
        },
        //subtitle: {
        //    text: '点击查看数据详情: <a href="#">武汉莘宸科技</a>.'
        //},
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
                                case "套餐1": createTable('json/aaa.json');  //页面跳转
                                    break;
                                case "套餐2": createTable('json/aaa.json');
                                    break;
                                case "套餐3": createTable('json/aaa.json');
                                    break;
                                case "套餐4": createTable('json/aaa.json');
                                    break;
                                case "套餐5": createTable();
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
                name: '套餐1',
                y: 5600
            }, {
                name: '套餐2',
                y: 4500

            }, {
                name: '套餐3',
                y: 3450

            }, {
                name: '套餐4',
                y: 2400
            }, {
                name: '套餐5',
                y: 2300
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
                //width:1600,
                loadMsg:"正在加载，请稍等...",
                striped: true,
                fit: true,//自动大小
                fitColumns: true,
                url:url,//查看收件箱内容
                columns:[[
                    //{field:'ID',title:'编号',checkbox:true},
                    {field:'ID',title:'编号'},
                    {field:'SellTime',title:'套餐编号',width:120,sortable:true,searchtype:"number"},
                    {field:'Events',title:'套餐名称',width:120,sortable:true},
                    {field:'LastSurplus',title:'会员卡号',width:200,sortable:true},
                    {field:'digitalCount',title:'实际时间',width:200,sortable:true},
                    {field:'CurrentSurplus',title:'类型',width:200,sortable:true},
                    {field:'Operator',title:'币数',width:200,sortable:true},
                    {field:'Operator2',title:'送积分',width:200,sortable:true},
                    {field:'Operator3',title:'金额',width:200,sortable:true},
                    {field:'Operator4',title:'优惠金额',width:200,sortable:true},
                    {field:'Operator5',title:'操作员',width:200,sortable:true},
                    {field:'Operator6',title:'工作站',width:200,sortable:true},
                    {field:'Operator7',title:'营业日期',width:200,sortable:true},
                    {field:'note',title:'备注',width:200,sortable:true}

                ]],
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
        $(".panel-body").css({padding:'2px'});
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
    createTable('json/aaa.json');
});
