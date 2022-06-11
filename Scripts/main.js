//CREATE A STATIC OBJECT
//CAN BE ACCESSED WITHOUT THE NEED FOR INITIALIZATION.
var Sortable = {
    baseUrl: '',
    sortBy: 0,
    searchTerm: '',
    Search() {
        var searchKey = $('#txtSearch').val();
        window.location.href = Sortable.baseUrl + "?id=" + searchKey;
    },
    Sort(sortBy) {
        var isDesc = true;

        const urlParams = new URLSearchParams(window.location.search);

        var isDescOriginal = urlParams.get('isDesc');
        const sortByOriginal = urlParams.get('sortBy');

        if (sortByOriginal == sortBy) {
            if (isDescOriginal == 'true') {
                isDesc = false;
            }
        }

        window.location.href = Sortable.baseUrl + "?sortBy=" + sortBy + "&isDesc=" + isDesc;
    }
};


var apiHandler = {
    GET(url) {
        $.ajax({
            url: url,
            type: 'GET',
            success: function (res) {
                debugger;
            }
        });
    },
    POST(url, object) {
        object = {
            Id: 5,
            Name: "asd",
            // ....
        }

        $.ajax({
            url: url,
            type: 'GET',
            data: object,
            success: function (res) {
                debugger;
            }
        });
    },
    DELETE(url) {
        if (confirm("Are you sure you want to delete?")) {
            $.ajax({
                url: url,
                type: 'GET',
                success: function (res) {
                    // we can redirect
                    // we can throw errors
                    // we can show a message
                    //document.location.href = "/somewhere"
                    //confirm("Deletion has been successful. You may go back to the list to recheck.")
                    if (res.Success == true) {
                        debugger;
                        location.href = res.returnUrl;
                    }
                    else {
                        alert(res.Message);
                    }
                }
            });
        } else {
            alert("Thank you for confirming not to delete.");
        }
    },

    //Calc: function () {
    //    //var s1 = $("#s1").val();
    //    //var s2 = $("#s2").val();

    //    var d1 = $("#d1").val();
    //    var i1 = $("#i1").val();

    //    $.get("/Calc/Multiply/?s1=" + d1 + "&s2=" + i1, function (data) {
    //        //$(".result").html(data);
    //        //alert("Load was performed.");
    //        //debugger;
    //        //alert(data);
    //        //if (data.isSuccess) {
    //        //    //alert(data);
    //        //}
    //        //else {
    //        //    alert(data.Message);
    //        //}
    //        $("#t1").val(data);
    //        debugger;

    //    });
    //}

    


}