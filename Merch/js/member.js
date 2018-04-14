
$(function(){
    $('.memberNav').on('click','li',function(){
        $(this).addClass("li_active").siblings().removeClass("li_active")
            .mouseover(function(){$(this).css({backgroundColor:'#eee'})})
            .mouseout(function(){$(this).css({backgroundColor:'#fff'})});
        var _index=$(this).index();
        $(".memberContent>li").eq(_index).css({'display':"block"}).siblings().css({'display':"none"});
    })
});

$(function(){
         var objLineChart={
    id:'#memberChart1',
    title: {
            text: '2017年会员增加趋势图'
        },
    xAxis: {
            categories: ['一月', '二月', '三月', '四月', '五月', '六月', '七月', '八月', '九月', '十月', '十一月', '十二月']
        },
    series:[{
            name: '2017年',
            data: [7.0, 6.9, 9.5, 14.5, 18.4, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
        }, {
            name: '2016年',
            data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
        }]
}

drawLineChart(objLineChart);

    $('.memberNav li').eq(0).click(function(){
        $(".memberChartBox1").css({display:'block'});
        $(".memberChartBox2").css({display:'none'});
        $(".memberChartBox3").css({display:'none'});

    
    });


     $('.memberNav li').eq(1).click(function(){
        $("#memberChart2").html("");
         $("#memberTable").text("");
           
        $(".memberChartBox1").css({display:'none'});
        $(".memberChartBox2").css({display:'block'});
        $(".memberChartBox3").css({display:'block'});

    var objBarChart={
    id:"#memberChart2",
     xAxis: {
            categories: ['操作'],
            title: {
                text: null
            }
        },
    series:[{
            name: '投币量',
            data: [110000]
        }, {
            name: '退币量',
            data: [40000]
        }, {
            name: '币余额',
            data: [30000]
        }, {
            name: '兑币量',
            data: [5000]
        }],
   }

   DrawBarChart(objBarChart);

var parasObj = {
        "pagename": "index",
        "processname": "test", //表格名
        "userid": "1",
        "token": "1",
        "signkey": "1f626576304bf5d95b72ece2222e42c3"
    };

    // parasObj中的processname和参数ids是同一个参数
    doApi(parasObj,"#memberTable");


    })

      $('.memberNav li').eq(2).click(function(){
 $("#memberChart2").html("");
         $("#memberTable").html("");
          
        $(".memberChartBox1").css({display:'none'});
        $(".memberChartBox2").css({display:'block'});
        $(".memberChartBox3").css({display:'block'});

        var objBarChart={
    id:"#memberChart2",
     xAxis: {
            categories: ['操作'],
            title: {
                text: null
            }
        },
    series:[{
            name: '投币量',
            data: [110000]
        }, {
            name: '退币量',
            data: [40000]
        }, {
            name: '币余额',
            data: [30000]
        }, {
            name: '兑币量',
            data: [5000]
        }],
   }

   DrawBarChart(objBarChart);
   var parasObj = {
        "pagename": "index",
        "processname": "test", //表格名
        "userid": "1",
        "token": "1",
        "signkey": "1f626576304bf5d95b72ece2222e42c3"
    };

    // parasObj中的processname和参数ids是同一个参数
    doApi(parasObj,"#memberTable");
    });


       $('.memberNav li').eq(3).click(function(){
         $("#memberChart2").html("");
         $("#memberTable").html("");
           
        $(".memberChartBox1").css({display:'none'});
        $(".memberChartBox2").css({display:'none'});
        $(".memberChartBox3").css({display:'block'});
        var parasObj = {
        "pagename": "index",
        "processname": "test", //表格名
        "userid": "1",
        "token": "1",
        "signkey": "1f626576304bf5d95b72ece2222e42c3"
    };

    // parasObj中的processname和参数ids是同一个参数
    doApi(parasObj,"#memberTable");
    });



        $('.memberNav li').eq(4).click(function(){
             $("#memberChart2").html("");
             $("#memberTable").html("");
       
    //     $(".memberChartBox1").css({display:'none'});
    //     $(".memberChartBox2").css({display:'none'});
    //     $(".memberChartBox3").css({display:'block'});
    //     var parasObj = {
    //     "pagename": "index",
    //     "processname": "test", //表格名
    //     "userid": "1",
    //     "token": "1",
    //     "signkey": "1f626576304bf5d95b72ece2222e42c3"
    // };

    // // parasObj中的processname和参数ids是同一个参数
    // doApi(parasObj,"#memberTable");
    });
})