/* 权限设置*/
function openSet(event) {
    $(".authorityList").toggle(500);
}

function authorizeSet(event) {
    $(".authorizeList").toggle(500);
}

function singleSelect(event) {
    $('.authorityList').on('click','li',function () {
        $(this).children('input').prop("checked","true");
        $(this).children('span').css({display:"block"});
        $(this).siblings('li').children('input').removeAttr("checked");
        $(this).siblings('li').children('span').css({display:"none"});
    })
}

function lotsSelect(event) {
    $('.authorizeList li').toggle(function () {
        $(this).children('input').prop("checked","true");
        $(this).children("div").children('span[class="s1"]').css({display:"inline-block"});
        $(this).children("div").children('span[class="s2"]').css({display:'none'})
    },function () {
        $(this).children('input').removeAttr("checked");
        $(this).children("div").children('span[class="s2"]').css({display:"inline-block"});
        $(this).children("div").children('span[class="s1"]').css({display:'none'})
    })
}