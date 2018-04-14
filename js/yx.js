
$(function () {     //请求，产品销售添加
    $.get(
        "data.json",{
        },function(data) {
            // console.log(data);
            var temp1=$(".salesStencil").html();

            for (var j = 0; j<data.length;j++){

                var txtSales = $('<li id="food'+j+'" class="salesList"></li>');
                $(txtSales).append(temp1);
                $(txtSales).find("img").attr("src","images/sales.png");
                $(txtSales).find("h3").append(data[j].foodname);
                $(txtSales).find("p").append(data[j].touristprice);
                $(txtSales).find("h5").append(data[j].title);

                $(".sales").find("ul").append(txtSales);
                if(data[j].foodtype == "recharge"){
                    $(".sales").find("li").eq(j).addClass("recharge");  //充值
                }
                if(data[j].foodtype == "gift"){
                    $(".sales").find("li").eq(j).addClass("gift");      //礼物
                }
                if(data[j].foodtype == "ticket"){
                    $(".sales").find("li").eq(j).addClass("ticket");    //门票
                }
                if(data[j].foodtype == "currency"){
                    $(".sales").find("li").eq(j).addClass("currency");  //货币
                }
                if(data[j].foodtype == "catering"){
                    $(".sales").find("li").eq(j).addClass("catering");  //餐饮
                }
                if(data[j].foodtype == "GiftBag"){
                    $(".sales").find("li").eq(j).addClass("GiftBag");   //礼物套餐
                    $(".sales").find("li").eq(j).find("h5").addClass("particulars");
                    var GiftMore =$('<div class="tag"><div class="arrow"> <em></em><span></span></div></div>');

                    GiftMore.append(data[j].fooddata);
                    $(".sales").find("li").eq(j).find("h5").append(GiftMore);
                }
            }


            //购物车添加
            var tempSales=$(".shopCartMb").html();
            $(".shopCartMb").remove();

            //销售多项弹出气泡


            $(".sales ul").on("mousedown","li .particulars",function (event) {
                $(this).find(".tag").toggle();
                    event.stopPropagation();
                    return false;
                });
            // $(".sales ul li .particulars").hover(function () {
            //     $(this).find(".tag").removeClass("hidden");
            // },function () {
            //     $(this).find(".tag").addClass("hidden");
            // });
            $(".sales ul").on("mousedown","li",function (event) {    //点击产品销售添加到购物车
                $(".sales ul").find(".tag").fadeOut();
                var foodID=$(this).attr("id").split("food")[1];
                // console.log(data[foodID].foodname);

                var shopCart = $('<li id="shopC'+foodID+'"></li>');
                shopCart.append(tempSales);
                $(shopCart).find("h4").append(data[foodID].foodname);
                // $(shopCart).find("h5").append(data[foodID].title);
                if(data[foodID].usecoin==-1){       //判断获取的数据，如果是-1则不显示
                    shopCart.find(".useCoin").remove();
                }

                if(data[foodID].uselottery==-1){
                    shopCart.find(".useLottery").remove();
                }

                if(data[foodID].usepoin==-1){
                    shopCart.find(".usePoin").remove();
                }

                $(shopCart).find(".shopD").append(data[foodID].touristprice);
                $(shopCart).find("strong").append(data[foodID].touristprice);
                var lId =$("li[id='shopC"+foodID+"']");

                if(lId .length <=0){
                    $(".shopCart").find("ul").append(shopCart);
                    shopCart.addClass("CountH").siblings().removeClass("CountH");
                }else {
                    var countc= lId.find("span").html();    //获取数值进行运算
                    countc ++;
                    var Total=parseFloat(data[foodID].touristprice) * parseInt(countc);
                    lId.find("span").html(countc);
                    lId.find("strong").html(Total);
                    // console.log(Total);
                    lId.addClass("CountH").siblings().removeClass("CountH");    //如果是现金添加￥
                }
                // console.log($(".shopCart").html());


                Statistics();   //调用函数，计算总值
            });
            $(".shopCart ul").on("change","li select",function (){  //当选择支付方式更换时候，更新各部分价值，总结价值
                var changeVal =$(this).val();
                var changeN =$(this).parents("li").attr("id").split("shopC")[1];

                if(changeVal == "Price"){
                    $(this).parents("li").find(".shopD").html(data[changeN].touristprice);
                }
                if(changeVal == "useCoin"){
                    $(this).parents("li").find(".shopD").html(data[changeN].usecoin);
                }
                if(changeVal == "useLottery"){
                    $(this).parents("li").find(".shopD").html(data[changeN].uselottery);
                }
                if(changeVal == "usePoint"){
                    $(this).parents("li").find(".shopD").html(data[changeN].usepoint);
                }
                var Total=parseFloat($(this).parents("li").find(".shopD").html()) * parseInt($(this).parents("li").find("span").html());
                    console.log(Total);
                $(this).parents("li").find("strong").html(Total);
                Statistics()
            });


        }
    );
    $(".shopCart ul ").on("click","li",function () {        //购物车 点击加背景，然后进行操作。
       $(this).addClass("CountH").siblings().removeClass("CountH");
    });
    $("#AddCount").on("click",function () {         //点加号，触发数量加一，并更新全部数据
        var countc=$(".CountH").find("span").html();
        countc ++;
        $(".CountH").find("span").html(countc);
        var Total=parseFloat($(".CountH").find(".shopD").html()) * parseInt(countc);

        $(".CountH").find("strong").html(Total);
        Statistics()
    });
    $("#reduceCount").click(function () {           //点减号，触发数量减一，并更新全部数据
        var countc=parseInt($(".CountH").find("span").html());
        countc --;
        console.log(countc);
        if(countc ==0){         //如果数量为零，就隐藏
            $(".CountH").remove();
        }else {
            $(".CountH").find("span").html(countc);
            var Total=parseFloat($(".CountH").find(".shopD").html()) *countc;
            $(".CountH").find("strong").html(Total);
        }
        Statistics()
    })

});




function Statistics() {     //更新购物车数据。获取上面的值和类型

    var shopCartL = $(".shopCart ul").find("li");
    var sum =0;var sumCoin =0;var sumLottery =0;var sumPoint =0;
    for(var i= 0;i<shopCartL.length;i++){
        var shopCartVl= shopCartL.eq(i).find(".useSelect").val();

        if(shopCartVl =="Price"){
            shopCartL.eq(i).find(".fhRMB").removeClass("hidden");
            sum +=parseInt(shopCartL.eq(i).find("strong").html());
        } else {
            shopCartL.eq(i).find(".fhRMB").addClass("hidden");
        }
        if(shopCartVl =="useCoin"){
            sumCoin +=parseInt(shopCartL.eq(i).find("strong").html());
        }
        if(shopCartVl =="useLottery"){
            sumLottery +=parseInt(shopCartL.eq(i).find("strong").html());
        }
        if(shopCartVl =="usePoint"){
            sumPoint +=parseInt(shopCartL.eq(i).find("strong").html());
        }

    }

    $(".sumPrice").html("￥"+sum.toFixed(2));
    $(".sumCoin").html(sumCoin);
    $(".sumLottery").html(sumLottery);
    $(".sumPoint").html(sumPoint);


}




function member() {     //获取ajax 来生成会员等级
    $.get(
        "data2.json",{
        },function(data) {
            var temp2=$(".Membership").html();
            for (var i = 0; i<data.length;i++){
                var txtMember = $('<li class="memberL swiper-slide"></li>');
                $(txtMember).append(temp2);

                $(txtMember).find("img").attr("src","images/Member.png");
                $(txtMember).find(".MemberJb").append(data[i].membertitle);
                $(txtMember).find(".MemberYj").append(data[i].memberdeposit);
                $(".memberList").find("ul").append(txtMember);

            }
        })
}
member();

function screening() {      //产品销售标题对下面进行筛选
    $(".salesTitle").find("li").click(function (){
        $(this).addClass("on").siblings().removeClass("on");

        if ($(this).index() ==0){
            $(".sales ul").find("li").removeClass("hide");
        } else {
            $(".sales ul").find("li").addClass("hide");
        }
        for (i=0;i<$(".sales ul").find("li").length;i++){

            if($(this).index() ==1 && $(".sales ul").find("li").eq(i).hasClass("recharge")){
                $(".sales ul").find("li").eq(i).removeClass("hide")
            }
            if($(this).index() ==2 && $(".sales ul").find("li").eq(i).hasClass("gift")){
                $(".sales ul").find("li").eq(i).removeClass("hide")
            }
            if($(this).index() ==3 && $(".sales ul").find("li").eq(i).hasClass("ticket")){
                $(".sales ul").find("li").eq(i).removeClass("hide")
            }
            if($(this).index() ==4 && $(".sales ul").find("li").eq(i).hasClass("currency")){
                $(".sales ul").find("li").eq(i).removeClass("hide")
            }
            if($(this).index() ==5 && $(".sales ul").find("li").eq(i).hasClass("catering")){
                $(".sales ul").find("li").eq(i).removeClass("hide")
            }
            if($(this).index() ==6 && $(".sales ul").find("li").eq(i).hasClass("GiftBag")){
                $(".sales ul").find("li").eq(i).removeClass("hide")
            }

        }
    });
}
screening();
$(function () {//所有弹出层
    var topHeight = $(".collapse").height();
    var heightKz = $(window).height()-topHeight-50;
    $(".heightKz").height(heightKz);
    $(".sales").height(heightKz-30);
     $(".shopCart").height(heightKz-120);


    $("#saveManual").click(function () {     //手工存币
        $(".ManualS").removeClass("hidden");
    });
    $("#CurrencyM").click(function () {      //提币

        $(".CurrencyS").removeClass("hidden");
    });
    $("#SendM").click(function () {         //送币

        $(".SendS").removeClass("hidden");
    });
    $("#backCardM").click(function () {     //退卡

        $(".backCardS").removeClass("hidden");
    });
    $(".PopupK").mousedown(function(event){  //防止冒泡
        event.stopPropagation();

    });
    $(".ManualS").mousedown(function(){    //蒙板层背景点击取消
        $(".ManualS").addClass("hidden");
    });
    $(".CurrencyS").mousedown(function(){  //蒙板层背景点击取消
        $(".CurrencyS").addClass("hidden");
    });
    $(".SendS").mousedown(function(){
        $(".SendS").addClass("hidden");
    }); //蒙板层背景点击取消
    $(".backCardS").mousedown(function(){
        $(".backCardS").addClass("hidden");
    });





    $("#FastSell").click(function () {//快速充币
        $(".FastSell").removeClass("hidden");
    });
    $(".FastSellK").mousedown(function(event){
        event.stopPropagation();
    });
    $(".FastSell").mousedown(function(event){
        $(".FastSell").addClass("hidden");
    });

    $("#sales").click(function () {         //点击套餐销售取消别的
        $("iframe").addClass("hidden");
        $(".salesAll").removeClass("hidden");

    });

    $(".CheckoutBtn2").click(function () {  //结账弹出
        if($(".shopCart").find("li").length ===0){
            // alert("请先购物再结算")
            $(".conF").fadeIn().fadeOut(2500);

        }
        else {
            $(".CheckoutMB").removeClass("hidden");
            var inputIN= $(".sumPrice").html().split("￥")[1];
            $("#input1").val(inputIN);
            $("#input3").val(inputIN);
            $("#input2").val(0.00)
        }
    });

    $("#input1").focus(function () {//结账现金聚焦
        var inputIN= $(".sumPrice").html().split("￥")[1];
       if($(this).val()===inputIN){
           $(this).val("")
       } else {
           var inputIN2=  $(this).val()-inputIN;
           $("#input2").val(inputIN2)
       }
    });



    $(".FastSellK").mousedown(function(event){  //快速售币弹出层
        event.stopPropagation();
    });
    $(".CheckoutMB").mousedown(function(event){
        $(".CheckoutMB").addClass("hidden");
    });
    $(".boxBtn").click(function () {//结账弹出
        $(".boxBg").removeClass("hidden");
    });
    $(".box").mousedown(function(event){    //饼状图弹出层
        event.stopPropagation();
    });
    $(".boxBg").mousedown(function(event){
        $(".boxBg").addClass("hidden");
    });



/*开关效果*/
    $(".switchZdy").find("div").click(function () {
        $(this).addClass("onSwitch").siblings().removeClass("onSwitch");
    })

});


function createTable(id,url,columns){       //表格
    $(id).bootstrapTable({ // 对应table标签的id
        url: url, // 获取表格数据的url
        method: 'get',                      //请求方式（*）
        toolbar: '.toolbar',                //工具按钮用哪个容器
        striped: true,                      //是否显示行间隔色
        cache: false,                       //是否使用缓存，默认为true，所以一般情况下需要设置一下这个属性（*）
        pagination: true,                   //是否显示分页（*）

        sortable: false,                     //是否启用排序
//        sortOrder: "asc",                   //排序方式
//        queryParams: oTableInit.queryParams,//传递参数（*）
        sidePagination: "server",           //分页方式：client客户端分页，server服务端分页（*）
        pageNumber:1,                       //初始化加载第一页，默认第一页
        pageSize: 5,                       //每页的记录行数（*）
        pageList: [5,10, 25, 50, 100],        //可供选择的每页的行数（*）
        search: false,                       //是否显示表格搜索，此搜索是客户端搜索，不会进服务端，所以，个人感觉意义不大
        strictSearch: true,
        showColumns: true,                  //是否显示所有的列
        showRefresh: true,                  //是否显示刷新按钮
        minimumCountColumns: 2,             //最少允许的列数
        clickToSelect: true,                //是否启用点击选中行
        // height: $(window.top).height()-200,                        //行高，如果没有设置height属性，表格自动根据记录条数觉得表格高度
        uniqueId: "ID",                     //每一行的唯一标识，一般为主键列
        showToggle:true,
        showFooter:true,                 //是否显示详细视图和列表视图的切换按钮
        cardView: false,                    //是否显示详细视图
        detailView: false,
        queryParams: function (params) { // 请求服务器数据时发送的参数，可以在这里添加额外的查询参数，返回false则终止请求
            return {
                pageSize: 10, // 每页要显示的数据条数
                offset: params.offset, // 每页显示数据的开始行号
                sort: params.sort, // 要排序的字段
                sortOrder: params.order, // 排序规则
                dataId: $("#dataId").val() // 额外添加的参数
            }
        },
        sortName: 'id', // 要排序的字段
        sortOrder: 'desc', // 排序规则
        columns: columns,
        onLoadSuccess: function(){  //加载成功时执行
            console.info("加载成功");
        },
        onLoadError: function(){  //加载失败时执行
            console.info("加载数据失败");
        }
    })
}





























































































