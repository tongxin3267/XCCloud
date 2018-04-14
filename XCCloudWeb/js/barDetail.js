/**
 * Created by Administrator on 2017-08-30.
 */
$(function () {

    //.............................................
    var sss=[{
        type: 'pie',
        name: '营业额占比',
        data: [
            ['套餐销售',   16089],
            ['售数字币',       8000],
            ['入会押金',    5000],
            ['商品销售',     6000],
            ['会员退钱',     2000],
            ['退会押金',     2000],
            ['条码退钱',     2000]
        ]
    }];
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $('#circle-bar').highcharts({
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
            text: '吧台明细'
        },
        legend: {
            align: 'right',
            verticalAlign: 'center',
            layout:"vertical",
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

                dataLabels: {
                    enabled: true,
                    format:'<b>{point.name}</b>: {point.y}<b>元</b>'
                },
                slicedOffset:30, //偏移距离
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
                                case "套餐充值": location.href="digitalSell.json.html";  //页面跳转
                                    break;
                                case "实币": location.href="digitalAnalys.html";
                                    break;
                                case '发卡': location.href="#";
                                    break;
                                case '过户币': location.href="#";
                                    break;
                            }
                        }
                    }
                }

            }
        },
        series: sss

    });


    //var calcTotal=function(table,column,table1){//合计，表格对象，对哪一列进行合计，第一列从0开始
    //    var trs=table.getElementsByTagName('tr');
    //    var trs1=table1.getElementsByTagName('tr');
    //    var start=1,//忽略第一行的表头
    //        end=trs.length;//忽略最后合计的一行
    //    var total=0;
    //    for(var i=start;i<end;i++){
    //        var td=trs[i].getElementsByTagName('td')[column];
    //        var t=parseFloat(td.innerHTML);
    //        if(t)total+=t;
    //    }
    //    trs1[0].getElementsByTagName('td')[column].innerHTML=total;
    //};
    //calcTotal(document.getElementById('tab1'),5,document.getElementById('tab2'));
    //calcTotal(document.getElementById('tab1'),6,document.getElementById('tab2'));
    //calcTotal(document.getElementById('tab1'),7,document.getElementById('tab2'));
    //calcTotal(document.getElementById('tab1'),8,document.getElementById('tab2'));
});

