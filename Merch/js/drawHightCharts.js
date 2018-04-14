//  画柱状图

function drawPieChart(objPie){
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $(objPie.id).highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            options3d: {//3D效果
                enabled: true,
                alpha: 45,
                beta:0
            }
        },
        //版权信息
        credits:{
            enabled:false
        },
        title: {
            text:objPie.titleText,
            align: 'left',
            style:{ "color": "#333333", "fontSize": "24px","fontWeight":"bold" }
        },
        legend: {
            align: 'right',
            verticalAlign: 'center',
            layout:"vertical",
            labelFormat:name,
            //x: 0,
            //y: 100,
            squareSymbol:false,
            symbolWidth:30,
            symbolHeight:18,
            lineHeight:18
        },

        tooltip: {
            headerFormat: '{series.name}<br>',
            pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
        },
        plotOptions: {
            pie: {
                size: 220,
                dataLabels: {
                    enabled: true,
                    format:'<b>{point.name}</b>: {point.y}'
                },
                slicedOffset:50, //偏移距离
                //innerSize: '80%',//设置环，内环占比
                depth:35,//显示3d效果
                showInLegend: true,
                allowPointSelect: true,
                cursor: 'pointer',
                point:{
                    events: {
                        select: function() {
                            var text = this.name;
                            console.log(text);
                            //switch (text){
                            //    case "充值":
                            //        console.log(1);
                            //        break;
                            //    case "售币":
                            //        console.log(2);
                            //        break;
                            //    case "开卡":
                            //        console.log(3);
                            //        break;
                            //    case "数字币":
                            //        console.log(4);
                            //        break;
                            //    case "礼品":
                            //        console.log(4);
                            //        break;
                            //    case "过户币":
                            //        console.log(4);
                            //        break;
                            //}
                        }
                    }
                },
                series : {
                    events : {
                        legendItemClick: function(event) {
                            alert("点击了："+this.name)
                        }
                    }
                }
            }
        },
        series: objPie.series
    });
}

//画饼状图

//var objHistogram={
//    id:"",
//    titleText:"",
//    yAxisText:"",
//    series:[{
//        name: '收银台编号',
//        colorByPoint: true,
//        data: [ {
//            name: '吧台1',
//            y: 5600
//
//        }, {
//            name: '吧台2',
//            y: 5000
//        }, {
//            name: '吧台3',
//            y: 10000
//
//        }]
//    }],
//    function:function aa(){}
//};
function  drawHistogram(objHistogram){
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    //Highcharts.chart('histogram', {
        $(objHistogram.id).highcharts({
        chart: {
            type: 'column'

        },
        credits:{

            enabled:false
        },
        title: {
            text: objHistogram.titleText,
            align: 'left',
            style:{ "color": "#333333", "fontSize": "24px","fontWeight":"bold" }
        },
        //设置饼图，柱状图，环形图的颜色
        //colors:['red','blue','yellow'],
        //subtitle: {
        //    text: '点击查看数据详情: <a href="#">武汉莘宸科技</a>.'
        //},
        xAxis: {
            type: 'category'
        },
        yAxis: {
            title: {
                text: objHistogram.yAxisText
            }
        },
        legend: {
            enabled: false
            //    verticalAlign:center
        },
        plotOptions: {
            stacking:'percent',
            series: {
                borderWidth: 0,
                allowPointSelect: true,
                cursor: 'pointer',
                dataLabels: {
                    enabled: true,
                    format: '{point.y:.1f}</b> 元<br/>'
                },
                point:{
                    events: {
                        select: function() {
                            var text = this.category;
                            console.log(text);
                            switch (text){
                                case 0:
                                    //location.href="test50.html";  //页面跳转
                                    break;
                                case 1:
                                    //location.href="test50.html";
                                    break;
                                case 2:
                                    //location.href="test50.html";
                                    break;
                            }
                        }
                    }
                }
            }
        },
        tooltip: {
            headerFormat: '<span style="font-size:11px">{series.name}</span><br>',
            pointFormat: '<span style="color:{point.color};font-size:18px">{point.name}</span> : <b style="color:{point.color};font-size:18px">{point.y}</b> 元<br/>'

        },
        //tooltip: {
        //    headerFormat: '{series.name}<br>',
        //    pointFormat: '{point.name}: <b>{point.percentage:.1f}%</b>'
        //},
        series: objHistogram.series

    });
}

// 画条形图
 // var objBarChart={
 //    id:"",
   //   xAxis: {
   //          categories: ['操作'],
   //          title: {
   //              text: null
   //          }
   //      },
 //    series:[{
 //            name: '投币量',
 //            data: [110000]
 //        }, {
 //            name: '退币量',
 //            data: [40000]
 //        }, {
 //            name: '币余额',
 //            data: [30000]
 //        }, {
 //            name: '兑币量',
 //            data: [5000]
 //        }],
 //   }


 function DrawBarChart(objBarChart){
    $(objBarChart.id).highcharts({
        chart: {
            type: 'bar'
        },
        title: {
            text: '',
             align: 'left',
            style:{ "color": "#333333", "fontSize": "24px","fontWeight":"bold" }
        },
        credits:{

            enabled:false
        },
        xAxis: objBarChart.xAxis,
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
        series: objBarChart.series
    });
    }


// var objLineChart={
//     id:'',
//     title: {
//             text: '2017年会员增加趋势图'
//         },
//     xAxis: {
//             categories: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
//         },
//     series:[{
//             name: '2017年',
//             data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
//         }, {
//             name: '2016年',
//             data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
//         }]
// }

function drawLineChart(objLineChart){
         Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $(objLineChart.id).highcharts({
        chart: {
            type: 'line'
        },
        title: { text:objLineChart.title,
                 align: 'left',
                 style:{ "color": "#333333", "fontSize": "24px","fontWeight":"bold" }
            },
        credits:{

            enabled:false
        },
        xAxis: objLineChart.xAxis,
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
        series: objLineChart.series
    });
    }