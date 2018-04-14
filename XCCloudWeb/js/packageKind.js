/**
 * Created by Administrator on 2017-08-30.
 */
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
            text: '2018年8月28日，套餐销售Top5'
        },
        subtitle: {
            text: '点击查看数据详情: <a href="https://netmarketshare.com">武汉莘宸科技</a>.'
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
                //point:{
                //    events: {
                //        select: function() {
                //            var text = this.category;
                //            console.log(text);
                //            switch (text){
                //                case 0: location.href="#";  //页面跳转
                //                    break;
                //                case 1: location.href="#";
                //                    break;
                //                case 2: location.href="#";
                //                    break;
                //            }
                //        }
                //    }
                //}

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
            name: '套餐编号',
            colorByPoint: true,
            data: [ {
                name: '特色套餐1',
                y: 600
            }, {
                name: '特色套餐2',
                y: 500

            }, {
                name: '特色套餐3',
                y: 450

            }, {
                name: '特色套餐4',
                y: 400
            }, {
                name: '特色套餐5',
                y: 300
            }
            ]
        }]

    });

});