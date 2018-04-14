/**
 * Created by Administrator on 2017-09-01.
 */
$(function(){
    //$(".submenu-tokenManage").on('click','li a',function(){
        $(".submenu-tokenManage").find('li a').click(function(){
        var _index=$(this).parent().index();
        $('.takenNav>li').eq(_index).addClass("active");
        $('.tokenPage>li').eq(_index).addClass("active").siblings().removeClass("active");
    });

      $(".takenNav").on('click','li',function(){
          var _index=$(this).index();
          $('.tokenPage>li').eq(_index).addClass("active").siblings().removeClass("active");
      });
    $(".takenNav").on('click','li>button',function(){
        var _index=$(this).parent().index();
        $('.takenNav>li').eq(_index).removeClass("active");
        $('.tokenPage>li').eq(_index).removeClass("active");
    })

});
$(function(){

    //var deleteColumn1=function(table,column,table1){//合计，表格对象，对哪一列进行合计，第一列从0开始
    //    var trs=table.getElementsByTagName('tr');
    //    var trs1=table1.getElementsByTagName('tr');
    //    var start=1,//忽略第一行的表头
    //        end=trs.length;//忽略最后合计的一行
    //    end=$(this).parents("table").find("tr").length;
    //    var total=0;
    //    for(var i=start;i<end;i++){
    //        var td=trs[i].getElementsByTagName('td')[column];
    //        var t=parseFloat(td.innerHTML);
    //        if(t)total+=t;
    //    }
    //    trs1[0].getElementsByTagName('td')[column].innerHTML=total;
    //};


});