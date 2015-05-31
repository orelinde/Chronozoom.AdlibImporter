(function (Formhandler) {
    var getXmlElementsFromUrl = function () {
        var url = document.getElementById("url").value;
        url = "http://am.adlibhosting.com/ChronoZoom/wwwopac.ashx?command=search&database=collect";
        var dropdownlistoptions = "";
        $.ajax({
            url: url + '&output=json&limit=1&search=all&xmltype=unstructured',
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
            var record = data.recordList.record[0];
            for (var prop in record) {
                if (prop === "@attributes") {
                    for (var attr in record['@attributes']) {
                        appendToDropdownlis(attr);
                    }
                }
                appendToDropdownlis(prop);
            }
            setLists();
        }

        function setLists() {
            var list = createSelectList();
            document.getElementById("title").innerHTML = list;
            document.getElementById("description").innerHTML = list;
            document.getElementById("begindate").innerHTML = list;
            document.getElementById("enddate").innerHTML = list;
            document.getElementById("images").innerHTML = list;
            document.getElementById("id").innerHTML = list;

        }

        function appendToDropdownlis(option) {
            dropdownlistoptions = dropdownlistoptions + "<option value=\"" + option + "\">" + option + "</option>";
        }

        function createSelectList() {
            var emptyOption = "<option value=\"null\"></option>";
            return "<select>"+ emptyOption + dropdownlistoptions + "</select>";
        }
     }

     Formhandler.GetXmlElementsFromUrl = getXmlElementsFromUrl;
}(Importer.Formhandler = {}));