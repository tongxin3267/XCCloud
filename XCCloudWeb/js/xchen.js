
$(function () {

    //.............................................
    Highcharts.setOptions({
        colors: ['#058DC7', '#50B432', '#ED561B', '#DDDF00', '#24CBE5', '#64E572', '#FF9655', '#FFF263', '#6AF9C4']
    });
    $('#circle1').highcharts({
        chart: {
            plotBackgroundColor: null,
            plotBorderWidth: null,
            plotShadow: false,
            options3d: {//3D效果
                enabled: true,
                alpha: 45,
                beta:0
            },
            colors:['red','blue']
        },
        //版权信息
        credits:{
            text:"武汉莘宸科技",
            href:"",
            enabled:false
        },
        title: {
            text: '游戏币总览',
            style:{ "color": "#333333", "fontSize": "24px","fontWeight":"bold" }
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
                size: 220,
                dataLabels: {
                    enabled: true,
                    format:'<b>{point.name}</b>: {point.y}<b>个</b>'
                },
                slicedOffset:50, //偏移距离
                //innerSize: '80%',//设置环，内环占比
                depth:35,//显示3d效果
                showInLegend: true,
                allowPointSelect: true,
                cursor: 'pointer',
                //point:{
                //    events: {
                //        select: function() {
                //            var text = this.name;
                //            console.log(text);
                //            switch (text){
                //                case "套餐充值": location.href="digitalSell.json.html";  //页面跳转
                //                    break;
                //                case "实币": location.href="digitalAnalys.html";
                //                    break;
                //                case '发卡': location.href="#";
                //                    break;
                //                case '过户币': location.href="#";
                //                    break;
                //            }
                //        }
                //    }
                //},
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
            name: '游戏币',
            data: [
                ['剩余',   60890],
                ['已消耗',       300000]
            ]
        }]
    });
});

$(function () {

    //.............................................
    $('#circle2').highcharts({
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
            text: '游戏币消耗详情',
            style:{ "color": "#333333", "fontSize": "24px","fontWeight":"bold" }
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
                size: 220,
                dataLabels: {
                    enabled: true,
                    format:'<b>{point.name}</b>: {point.y}<b>个</b>'
                },
                slicedOffset:50, //偏移距离
                //innerSize: '80%',//设置环，内环占比
                depth:35,//显示3d效果
                showInLegend: true,
                allowPointSelect: true,
                cursor: 'pointer',
                //point:{
                //    events: {
                //        select: function() {
                //            var text = this.name;
                //            console.log(text);
                //            switch (text){
                //                case "套餐充值": location.href="digitalSell.json.html";  //页面跳转
                //                    break;
                //                case "实币": location.href="digitalAnalys.html";
                //                    break;
                //                case '发卡': location.href="#";
                //                    break;
                //                case '过户币': location.href="#";
                //                    break;
                //            }
                //        }
                //    }
                //},
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
            name: '机器编号',
            data: [
                ['1号机',   16089],
                ['2号机',   16089],
                ['3号机',       18000],
                ['4号机',    15000],
                ['5号机',     16000]
            ]
        }]
    });
});


//柱状图
$(function(){
    var $arrInDiv=document.getElementsByClassName("innerTip");
    var $arrInPercent=document.getElementsByClassName("innerPercent");
    var $arrFore=document.getElementsByClassName("foreTip");
    var barDiv=  document.getElementsByClassName("bar-inner");
    var data={
        innerNum: [80,180,70],
        allNum:[100,200,300]
    };
    var arrInner=data.innerNum;
    var arrAll=data.allNum;

    //设置警告

    function setNumber(){
        for(var i=0;i<$arrInDiv.length;i++){
            var num=(arrInner[i]/arrAll[i]).toFixed(4)*100;
            //console.log(num);
            var textFixed=num.toFixed(2)+"%";
            $arrInDiv[i].innerHTML= textFixed ;
            if(arrInner[i]/arrAll[i]>0.7){
                 $('.bar-inner').eq(i).css({backgroundColor:'#ff6666',color:'#fff'}).addClass("bar-inner1");
            }
        }
        for(var i=0;i<$arrInPercent.length;i++){
            $arrInPercent[i].innerHTML=arrInner[i];
        }
        for(var i=0;i<$arrFore.length;i++){
            $arrFore[i].innerHTML=arrAll[i];
        }
    }

    function setHeight(){
        $(".bar-inner").each(function(){
            var index=$(this).find($('.innerTip')).html();
            //console.log(index);
            //console.log(parseFloat(index)/100);
            $(this).css("height",parseFloat(index)/100*30+"em");
            //     if(parseFloat(index)>0.7){
            //     $(this).css('backgroundColor','red') ;
            //}
        });
    }
   $(document).ready(function(){
        setNumber();
        setHeight();
   });
    //鼠标覆盖

    $('.graph-container').on('mouseover','li',function(){
        $(this).find(".innerTip").css({display:'none',color:'#fff'});
        $(this).find(".innerPercent").css('display','block');
    }).on('mouseout','li',function(){
        $(this).find(".innerTip").css({display:'block',color:'blue'});
        $(this).find(".innerPercent").css('display','none');
    });
});