(function (Formhandler) {
    var getXmlElementsFromUrl = function () {
        var url = document.getElementById("url").value;

        $.ajax({
            url: url+'&output=json',
            type: "GET",
            dataType: "jsonp",
            async: true,
            cache: false,
            contentType: "application/x-www-form-urlencoded",
            data: 'json',
            success: function (data) {

                //Check the diagnostic node for errors
                if (data.adlibJSON.diagnostic.error != undefined) {
                    alert("error: " + data.adlibJSON.diagnostic.error.message);
                }
                else {
                    callback(data.adlibJSON);
                }
            },
            error: function (xhr, msg) {
                alert("error: " + xhr.responseText + " " + msg);
            }
        });

        var callback = function (data) {
            console.log(data);
            if (!data.hasOwnProperty("recordList")) console.log("No recordList");
            if (!data.recordList.hasOwnProperty("record")) console.log("No recordslist found");
            if (!data.recordList.length === 0) console.log("No records found");
            for (var prop in data.recordList.record[0]) {
                console.log(prop);
            }
        }

        

       // Importer.Webhandler.GetXmlElementsFromUrl(url);
     }

     Formhandler.GetXmlElementsFromUrl = getXmlElementsFromUrl;
}(Importer.Formhandler = {}));