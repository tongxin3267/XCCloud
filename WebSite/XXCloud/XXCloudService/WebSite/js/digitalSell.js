
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

});

var parasObj = {
    "pagename": "digitalSell",
    "processname": "digitalSell", //表格名
    "userid": "0",
    "token": "1",
    "signkey": "1f626576304bf5d95b72ece2222e42c3"
};

// parasObj中的processname和参数ids是同一个参数
doApi(parasObj,"#digitalSell");