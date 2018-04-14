/**
 * Created by Administrator on 2017-08-29.
 */





$(function () {
        var   data1= [
        ['2017年9月5号', 85311],
        ['2017年9月13号', 61750]
    ];
 var percentNum=((data1[1][1]/data1[0][1])*100).toFixed(0);
   for(var i=0;i<data1.length;i++){
       $("#news li").eq(i).find("span:first-child"). text(data1[i][0]+':');
       $("#news li").eq(i).find("span:last-child"). text(data1[i][1]);
       $("#news li:last-child .sellValue").text(percentNum+"%");
   }

    function percentChart(a){
        Highcharts.setOptions({
            colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
        });
        Highcharts.chart('loop', {
            chart: {
                type: 'solidgauge',
                marginTop: 50,
                backgroundColor: 'white',
                colors: [ '#0CCDD6'],
                title: {
                    style: {
                        color: 'silver'
                    }
                },
                tooltip: {
                    style: {
                        color: 'silver'
                    }
                }
            },
            //legend: {
            //    layout: 'vertical',
            //    backgroundColor: '#FFFFFF',
            //    floating: true,
            //    align: 'left',
            //    verticalAlign: 'top'
            //    //x: 90,
            //y: 45,
            //labelFormatter: function () {
            //    return this.name + ' (click to hide)';
            //}
            //},
            credits: {
                text: '莘宸科技',
                href: '#',
                enabled:false
            },
            title: {
                text: '今日营收总览',
                align:'left',
                style: {
                    fontSize: '24px'
                }
            },
            //tooltip: {
            //    borderWidth: 0,
            //    backgroundColor: 'none',
            //    shadow: false,
            //    style: {
            //        fontSize: '16px'
            //    }
            //    //pointFormat: '{series.name}<br><span style="font-size:2em; color: {point.color}; font-weight: bold">{point.y}%</span>',
            //    //positioner: function (labelWidth, labelHeight) {
            //    //    return {
            //    //        x: 450 - labelWidth / 2,
            //    //        y: 150
            //    //    };
            //    //}
            //},
            pane: {
                startAngle: 0,
                endAngle: -360,
                background: [{ // Track for Move
                    outerRadius: '88%',
                    innerRadius: '112%',
                    backgroundColor: '#f7a35c'
                        //Highcharts.Color(Highcharts.getOptions().colors[0]).setOpacity(0.3).get()
                    ,
                    borderWidth: 0
                }

                ]
            },
            yAxis: {
                min: 0,
                max: 100,
                lineWidth: 0,
                tickPositions: []
            },
            plotOptions: {
                solidgauge: {
                    borderWidth: '34px',
                    dataLabels: {
                        enabled: false
                    },
                    linecap: 'round',
                    stickyTracking: false
                }
            },
            series: [{
                name: '当前已完成',
                borderColor: Highcharts.getOptions().colors[0],
                data: [{
                    color: Highcharts.getOptions().colors[0],
                    radius: '100%',
                    innerRadius: '100%',
                    y:a
                }]
            }

            ]
        },
        /**
         * In the chart load callback, add icons on top of the circular shapes
         */
        function callback() {
            // Move icon
            this.renderer.path(['M', -8, 0, 'L', 8, 0, 'M', 0, -8, 'L', 8, 0, 0, 8])
                .attr({
                    'stroke': '#303030',
                    'stroke-linecap': 'round',
                    'stroke-linejoin': 'round',
                    'stroke-width': 2,
                    'zIndex': 10
                })
                .translate(190, 26)
                //.add(this.series[2].group)
            ;
            // Exercise icon
            this.renderer.path(['M', -8, 0, 'L', 8, 0, 'M', 0, -8, 'L', 8, 0, 0, 8, 'M', 8, -8, 'L', 16, 0, 8, 8])
                .attr({
                    'stroke': '#fff',
                    'stroke-linecap': 'round',
                    'stroke-linejoin': 'round',
                    'stroke-width': 2,
                    'zIndex': 10
                })
                .translate(190, 61)
                //.add(this.series[2].group)
            ;
            // Stand icon
            this.renderer.path(['M', 0, 8, 'L', 0, -8, 'M', -8, 0, 'L', 0, -8, 8, 0])
                .attr({
                    'stroke': '#303030',
                    'stroke-linecap': 'round',
                    'stroke-linejoin': 'round',
                    'stroke-width': 2,
                    'zIndex': 10
                })
                .translate(190, 96)
                //.add(this.series[2].group)
            ;
        }
    );}
     percentChart(parseInt(percentNum));
});





$(function () {

    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $('#circle').highcharts({
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
            text:"武汉莘宸科技",
            href:"",
            enabled:false
        },
        title: {
            text: '今日营收分析',
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
                    format:'<b>{point.name}</b>: {point.y}<b>元</b>'
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
                            switch (text){
                                case "套餐充值":
                                    var txt1=$("<a href='javascript:void(0)'></a>").text("数字币销售分析");

                                    var txt2=$('<li id="hShow15"><em><span class="icon iconfont icon-cha2"></span></em></li>');
                                    var idN =$("li[id='hShow15']", window.parent.document);
                                    if(!idN.length>0){
                                        $(".titleAppend", window.parent.document).find("ul").append(txt2);
                                        $(txt2).find("em").append(txt1);
                                        $(txt2).addClass("now").siblings().removeClass("now");
                                        $('.test2', window.parent.document).find('iframe').hide();
                                        var iFrame = $('<iframe id="iFrame15"  width="100%" height="100%" frameborder="0" seamless></iframe>').attr('src',"digitalSell.html");
                                        $('.test2', window.parent.document).append(iFrame);
                                    } else {
                                        idN.addClass("now").siblings().removeClass("now");
                                        $("iframe[id='iFrame15']", window.parent.document).show().siblings().hide()
                                    }
                                    break;
                                case "实币":
                                    var txt1=$("<a href='javascript:void(0)'></a>").text("数字币销售分析");

                                    var txt2=$('<li id="hShow15"><em><span class="icon iconfont icon-cha2"></span></em></li>');
                                    var idN =$("li[id='hShow15']", window.parent.document);
                                    if(!idN.length>0){
                                        $(".titleAppend", window.parent.document).find("ul").append(txt2);
                                        $(txt2).find("em").append(txt1);
                                        $(txt2).addClass("now").siblings().removeClass("now");
                                        $('.test2', window.parent.document).find('iframe').hide();
                                        var iFrame = $('<iframe id="iFrame15"  width="100%" height="100%" frameborder="0" seamless></iframe>').attr('src',"digitalSell.html");
                                        $('.test2', window.parent.document).append(iFrame);
                                    } else {
                                        idN.addClass("now").siblings().removeClass("now");
                                        $("iframe[id='iFrame15']", window.parent.document).show().siblings().hide()
                                    }
                                    break;
                                case '发卡': location.href="#";
                                    break;
                                case '过户币': location.href="#";
                                    break;
                            }
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
        series: [{
            type: 'pie',
            name: '营业额占比',
            data: [
                ['套餐充值',   16089],
                ['实币',       8000],
                ['发卡',    5000],
                ['过户币',     6000]
            ]
        }]
    });
});
$(function () {
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    Highcharts.chart('histogram', {
        chart: {
            type: 'column'
            //options3d: {//3D效果
            //    enabled: true,
            //    alpha: 15,
            //    beta:15,
            //    viewDistance:25,
            //    depth:30
            //}

        },
        credits:{

            enabled:false
        },
        title: {
            text: '今日吧台营收',
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
                text: '总的市场份额'
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
        series: [{
            name: '收银台编号',
            colorByPoint: true,
            data: [ {
                name: '吧台1',
                y: 5600

            }, {
                name: '吧台2',
                y: 5000
            }, {
                name: '吧台3',
                y: 10000

            }]
        }]

    });

});

$(function () {
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $('#homeLine').highcharts({
        title: {
            text: '客流量分析'
        },
        xAxis: {
            categories: ['8', ' 9', '10', '11', '12']
        },
        plotOptions: {
            series: {
                stacking: 'normal'
            }
        },
        credits:{

            enabled:false
        },
        labels: {
            items: [{
                html: '客流量分析',
                style: {
                    left: '100px',
                    top: '18px',
                    color: (Highcharts.theme && Highcharts.theme.textColor) || 'black'
                }
            }]
        },
        series: [{
            type: 'column',
            name: '客流量',
            data: [300, 200, 100, 300, 400]
        }, {
            type: 'spline',
            name: '客流量平均值',
            data: [300, 267, 300, 633, 333],
            marker: {
                lineWidth: 2,
                lineColor: Highcharts.getOptions().colors[3],
                fillColor: 'white'
            }
        }
            //          , {
            //     type: 'pie',
            //     name: '总的消耗',
            //     data: [{
            //         name: '小张',
            //         y: 13,
            //         color: Highcharts.getOptions().colors[0] // Jane's color
            //     }, {
            //         name: '小潘',
            //         y: 23,
            //         color: Highcharts.getOptions().colors[1] // John's color
            //     }, {
            //         name: '小王',
            //         y: 19,
            //         color: Highcharts.getOptions().colors[2] // Joe's color
            //     }],
            //     center: [100, 80],
            //     size: 100,
            //     showInLegend: false,
            //     dataLabels: {
            //         enabled: false
            //     }
            // }
        ]
    });
});
$(function(){
    $('#profitLoss').highcharts({
        chart: {
            type: 'column'
},
    title: {
        text: "游戏机台盈亏排行榜"
    },
    xAxis: {
        categories: ['二战风云', '疯狂赛车', '魔幻钓鱼', '超级跳杆', '风火轮'],
        labels: {
            step: 0 //设置X轴间隔多少显示
        },
        title: {
            text: "游戏机台"
        }
    },
    yAxis: {
        title: {
            text: "盈亏额"
        }
    },
    series: [{
        color:'red',
    negativeColor: 'green',//就是这个属性设置负值的颜色
    data: [5,4,2,-2,-3]
}],

    legend:{
        enabled: false
    },
    credits:{//版权信息
        enabled:false
    },
    tooltip:{//数据点提示框
        enabled:false
    }
});


});

//
