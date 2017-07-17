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
        submitDBs(server1,db1,server2,db2);
        hideDBInput();
    });

    // reinput btn click event
    $(".btn-reinput").click(function(){
        hideHeader();
        hideTable();
    })
}

// Header animate
function showHeader(){
    $("#header").animate({width:'100%'},500,function(){
        $(".header-db").animate({opacity:'1',paddingTop:'0'},1000);
        $(".header-btns").animate({opacity:'1',marginTop:'7px'},1000);
        $(".compare-title-text").animate({opacity:'1',marginTop:'0px'},1000);
        showTable();
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
function submitDBs(server1,dbname1,server2,dbname2){
    $(".compare-title-text").text("COMPARING...").css("color","black");
    var url = "/api/DBComparator?server1=" + server1 + "&dbname1=" + dbname1 + "&server2=" + server2 + "&dbname2=" + dbname2;
    $.get(url, function (data) {
        console.log(url,data);
        if(data.code > 1000){
            $(".compare-title-text").text("ERROR:" + data.msg).css("color","red");
        }
        else if(data.code == 1000){
            $(".compare-title-text").text("TABLES");
            console.log(data.tables);
            initTableDiff(data.tables);
        }
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

// Table function
function showTable(){
    $("#main").animate({paddingLeft:'50px',opacity:'1'},1000);
}
function hideTable(){
    $("#main").animate({paddingLeft:'100px',opacity:'0'},1000);
}
function initTable(columnDefs,rowData){
    // let the grid know which columns and what data to use
    var gridOptions = {
        columnDefs: columnDefs,
        rowData: rowData,
        onGridReady: function () {
            gridOptions.api.sizeColumnsToFit();
        },
        enableSorting:true,
        enableFilter:true
    };

    // lookup the container we want the Grid to use
    var eGridDiv = document.querySelector('#main');

    // create the grid passing in the div to use together with the columns & data we want to use
    new agGrid.Grid(eGridDiv, gridOptions);
}
function initTableDiff(tables){
    var columnDefs = [
        {headerName: "NAME", field: "name"},
        {headerName: "COEXIST", field: "coexist"},
        {headerName: "BELONG", field: "belong"},
        {headerName: "COLUMNS", field: "columns"},
        {headerName: "INDEXES", field: "indexes"},
        {headerName: "KEYS", field: "keys"}
    ];
    var rowData = [];
    for(var i = 0; i < tables.length;i++){
        var item = tables[i];
        rowData.push({
            name:item.name,
            coexist:item.coexist,
            belong:item.dbnameIfNotCoexit,
            columns:item.columns.length,
            indexes:item.indexes.length,
            keys:item.keys.length
        })
    }
    initTable(columnDefs,rowData)
}