(function (Formhandler) {

    /* Makes the inputUrl on loading state and the input form 
    visible if the data is correctly received. Also will the select lists be populated 
    with the xml elements*/
    var getXmlElementsFromUrl = function () {

        /* The JSONP callback on success function which will scrape the xml elements
        from the received data. The method also checks if the returned data has the
        right properties to parse. if true then it will create a select list*/
        var callback = function (data) {
            inputFormIsVisisble(true);
            actionFormIsVisisble(true);
            inputUrlPartIsLoading(false);
            if (!data.hasOwnProperty("recordList")) console.log("No recordList");
            if (!data.recordList.hasOwnProperty("record")) console.log("No recordslist found");
            if (!data.recordList.length === 0) console.log("No records found");
            var record = data.recordList.record[0];
            for (var prop in record) {
                if (prop === "@attributes") {
                    for (var attr in record['@attributes']) {
                        appendToDropdownlist(attr);
                    }
                }
                appendToDropdownlist(prop);
            }
            setLists();
        }

        // Hide the input form
        inputFormIsVisisble(false);
        actionFormIsVisisble(false);
        var url = document.getElementById("url").value;
        url = "http://am.adlibhosting.com/ChronoZoom/wwwopac.ashx?command=search&database=collect"; // TEMPORARY FOR DEBUG

        // Call api and make url part in loading state
        inputUrlPartIsLoading(true);
        Importer.Webhandler.GetXmlElementsFromUrl(url, callback);

        var dropdownlistoptions = "";

        // Set the options on the selectlist on the html
        function setLists() {
            var list = createSelectList();
            document.getElementById("title").innerHTML = list;
            document.getElementById("description").innerHTML = list;
            document.getElementById("begindate").innerHTML = list;
            document.getElementById("enddate").innerHTML = list;
            document.getElementById("images").innerHTML = list;
            document.getElementById("id").innerHTML = list;

        }

        //Helper method to create an option per xml element
        function appendToDropdownlist(option) {
            dropdownlistoptions = dropdownlistoptions + "<option value=\"" + option + "\">" + option + "</option>";
        }

        // Append empty value and return the list with options
        function createSelectList() {
            var emptyOption = "<option value=\"null\"></option>";
            return  emptyOption + dropdownlistoptions;
        }


        function inputUrlPartIsLoading(isLoading) {
            var urlInputForm = document.getElementById("urlInputLoader");
            if (isLoading) {
                urlInputForm.classList.add('active');
            } else {
                urlInputForm.classList.remove('active');
            }
        }
        function inputFormIsVisisble(isVisible) {
            var inputForm = document.getElementById("inputForm");
            if (isVisible) {
                inputForm.style.display = "block";
            } else {
                inputForm.style.display = "none";
            }
        }
        function actionFormIsVisisble(isVisible) {
            var inputForm = document.getElementById("actionForm");
            if (isVisible) {
                inputForm.style.display = "block";
            } else {
                inputForm.style.display = "none";
            }
        }
     }

     Formhandler.GetXmlElementsFromUrl = getXmlElementsFromUrl;
}(Importer.Formhandler = {}));