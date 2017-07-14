$(document).ready(function () {

    $("#submit").click(function () {
        var server1 = $("#txtserver1").val();
        var database1 = $("#txtdatabase1").val();
        var server2 = $("#txtserver2").val();
        var database2 = $("#txtdatabase2").val();
        compare(server1, database1, server2, database2);
    });
});

function compare(server1, databas1, server2, database2) {
    var twoDB = server1 + "&" + databas1 + "&" + server2 + "&" + database2;

    $.post("/api/DBComparator", { "": twoDB }, function (data) {
        console.log(data);
        
    });
}