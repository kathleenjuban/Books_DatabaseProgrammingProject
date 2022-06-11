var apiHandler2 = {
    Calc: function () {
        //var s1 = $("#s1").val();
        //var s2 = $("#s2").val();

        var m1 = $("#m1").val();
        var m2 = $("#m2").val();
        var m3 = $("#m3").val();

        $.get("/Calc/Add/?s1=" + m1 + "&s2=" + m2 + "&s3=" + m3, function (data) {
            //$(".result").html(data);
            //alert("Load was performed.");
            //debugger;
            //alert(data);
            $("#m4").val(data);
            debugger;


        });


        var d1 = $("#d1").val();
        var i1 = $("#i1").val();

            $.get("/Calc/Multiply/?s1=" + d1 + "&s2=" + i1, function (data) {
                //$(".result").html(data);
                //alert("Load was performed.");
                //debugger;
                //alert(data);
                //if (data.isSuccess) {
                //    //alert(data);
                //}
                //else {
                //    alert(data.Message);
                //}
                $("#t1").val(data);
                debugger;

            });
    }
}
