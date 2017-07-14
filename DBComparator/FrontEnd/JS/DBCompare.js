/**
 * Created by XiaJx on 7/14/2017.
 */
var connectionStr;
var response;
$(document).ready(function(){
    init();
});

function init(){

    $("#servername-one").tooltip();
    $("#servername-two").tooltip();
    $("#db-name-one").tooltip();
    $("#db-name-two").tooltip();

    // compare btn click event
    $(".btn-compare").click(function(){
        var server1 = $("#servername-one").val();
        var server2 = $("#servername-two").val();
        var db1 = $("#db-name-one").val();
        var db2 = $("#db-name-two").val();
        $(".header-db").text(server1 + "," + db1 + " ; " + server2 + "," + db2);
        connectionStr = server1 + "&" + db1 + "&" +server2 + "&" + db2;
        submitDBs(connectionStr);
        hideDBInput();
    });

    // reinput btn click event
    $(".btn-reinput").click(function(){
        hideHeader();
    })
}

// Header animate
function showHeader(){
    $("#header").animate({width:'100%'},500,function(){
        $(".header-db").animate({opacity:'1',paddingTop:'0'},1000);
        $(".header-btns").animate({opacity:'1',marginTop:'7px'},1000);
        $(".compare-title-text").animate({opacity:'1',marginTop:'0px'},1000);
    });
}
function hideHeader(){
    $(".header-db").css("opacity","0").css("paddingTop","10px");
    $(".header-btns").css("opacity","0").css("marginTop","17px");
    $(".compare-title-text").css("opacity","0").css("marginTop","10px");
    $("#header").animate({width:'0'},500);
    showDBInput();
}

// DBInput function
function submitDBs(connectStr){
    $.post("/api/DBComparator", { "": connectStr }, function (data) {
        response = data;
        console.log(response);
    });
}
function hideDBInput(){
    var dbInput = $("#db-input");

    dbInput.animate({paddingTop:'30px',opacity:0},1000,function(){
        dbInput.css("display","none");
        showHeader();
    })
}
function showDBInput(){
    var dbInput = $("#db-input");

    dbInput.css("display","block");
    dbInput.animate({paddingTop:'100px',opacity:1},1000);
}