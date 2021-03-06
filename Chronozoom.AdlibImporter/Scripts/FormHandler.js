﻿(function (Formhandler) {

    /* Makes the inputUrl on loading state and the input form 
    visible if the data is correctly received. Also will the select lists be populated 
    with the xml elements*/
    var whichurl = 0;
    var getXmlElementsFromUrl = function (todourl) {
        whichurl = todourl;
        // Hide the input form
        inputFormIsVisisble(false);
        actionFormIsVisisble(false);
        inputUrlPartIsLoading(true);
        errorsIsVisisble(false);
        /* The JSONP callback on success function which will scrape the xml elements
        from the received data. The method also checks if the returned data has the
        right properties to parse. if true then it will create a select list*/
        var callback = function (data) {
            if (!data.hasOwnProperty("recordList")) console.log("No recordList");
            if (!data.recordList.hasOwnProperty("record")) console.log("No recordslist found");
            if (!data.recordList.length === 0) console.log("No records found");
            var record = data.recordList.record[0];
            for (var prop in record) {
                if (prop === "@attributes") {
                    for (var attr in record['@attributes']) {
                        appendToDropdownlist(attr);
                    }
                } else {
                    appendToDropdownlist(prop);
                }
            }
            setLists();
            resetVisuals();
        }

        var callbackMetadata = function (data) {
            inputFormIsVisisble(true);
            actionFormIsVisisble(true);
            inputUrlPartIsLoading(false);
            if (!data.hasOwnProperty("recordList")) console.log("No recordList");
            if (!data.recordList.hasOwnProperty("record")) console.log("No recordslist found");
            if (!data.recordList.length === 0) console.log("No records found");
            var records = data.recordList.record;
            for (var i = 0; i < records.length; i++) {
                var obj = records[i];
                appendToDropdownlist(obj.displayName[0].value[0]);
            }
            setLists();
            resetVisuals();
        }

        var url = htmlElementValue("url");
        var database = htmlElementValue("database");


        // TODO: remove -- debug only
        if (whichurl === 1) {
            url = "http://amdata.adlibsoft.com/";
            database = "AMCollect";
        }
        // Call api and make url part in loading state
        Importer.Webhandler.GetXmlElementsFromUrl(url, database, callback);
        Importer.Webhandler.GetMetadata(url, database, callbackMetadata);


        // Set the options on the selectlist on the html
        function setLists() {
            var list = createSelectList();
            document.getElementById("title").innerHTML = list;
            document.getElementById("description").innerHTML = list;
            document.getElementById("begindate").innerHTML = list;
            document.getElementById("enddate").innerHTML = list;
            document.getElementById("images").innerHTML = list;
            document.getElementById("id").innerHTML = list;
            document.getElementById("groupby").innerHTML = list;

        }

        /*Wait for all callbacks to complete before making the form visible again*/
        var reset = 0;
        function resetVisuals() {
            reset = reset + 1;
            if (reset == 2) {
                inputFormIsVisisble(true);
                actionFormIsVisisble(true);
                inputUrlPartIsLoading(false);
                reset = 0;
            }
        }

        //Helper method to create an option per xml element
        var dropdownlistoptions = "";
        function appendToDropdownlist(option) {
            dropdownlistoptions = dropdownlistoptions + "<option value=\"" + option + "\">" + option + "</option>";
        }

        // Append empty value and return the list with options
        function createSelectList() {
            var emptyOption = "<option value=\"null\"></option>";
            return emptyOption + dropdownlistoptions;
        }

        
    }

    var createBatchCallback = function(errors) {
        if (errors !== undefined) {
            errorsIsVisisble(true, errors);
        }
    }

    var actions = [];

    function createBatchCommand() {
        errorsIsVisisble(false);

        // Data source
        var url = htmlElementValue("url");
        var database = htmlElementValue("database");
        var imageslocation = htmlElementValue("imageslocation");

        // TODO: remove -- debug only
        if (whichurl === 1) {
            url = "http://amdata.adlibsoft.com/";
            database = "AMCollect";
        }

        // Timeline elements
        var timelineTitle = htmlElementValue("timelinetitle");
        var timelineDescription = htmlElementValue("timelinedescription");

        // Mapper elements
        var title = htmlDropDownValue("title");
        var description = htmlDropDownValue("description");
        var begindate = htmlDropDownValue("begindate");
        var enddate = htmlDropDownValue("enddate");
        var images = htmlDropDownValue("images");
        var id = htmlDropDownValue("id");

        var batch = {
            BaseUrl: url,
            Database : database,
            Mappings: {
                Title: title,
                Description: description,
                Begindate: begindate,
                Enddate: enddate,
                Images: images,
                Id: id
            },
            ImagesLocation: imageslocation,
            Actions:actions,
            Title: timelineTitle,
            Description : timelineDescription
        }

        Importer.Webhandler.CreateBatchCommand(batch,createBatchCallback);
    }

    
    function removeAction(index) {
        actions.splice(index, 1);
        rebuildActionList();
    }

    function reset() {
       
    }

    function addAction() {
        var groupBy = htmlDropDownValue("groupby");
        var categoryName = htmlElementValue("categoryname");
        var action = { GroupBy : groupBy, CategoryName : categoryName };
        actions.push(action);
        rebuildActionList();
    }

    function rebuildActionList() {
        var list = document.getElementById("actionsList");
        list.innerHTML = "";
        for (var i = 0; i < actions.length; i++) {
            list.innerHTML = list.innerHTML + "<div class=\"item\">" +
                "<div onClick=\"Importer.Formhandler.RemoveAction(" + i + ")\" class=\"right floated compact ui button\">Remove</div>" +
                "<div class=\"content\">" +
                "<div class=\"header\">" + actions[i].GroupBy + "</div>" + actions[i].CategoryName + "</div>" +
                "</div>";
        }
         
        
    }

    function htmlElementValue(value) {
        return document.getElementById(value).value;
    }
    function htmlDropDownValue(element) {
        var element = document.getElementById(element);
        return element.options[element.selectedIndex].value;
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
    function errorsIsVisisble(isVisible, errors) {
        var inputForm = document.getElementById("errorBlock");
        if (isVisible) {
            inputForm.style.display = "block";
            var errorList = document.getElementById("errorList");
            errorList.innerHTML = "";
            for (var i = 0; i < errors.length;i++) {
                errorList.innerHTML = errorList.innerHTML + "<li>"+errors[i]+"</li>";
            }
        } else {
            inputForm.style.display = "none";
        }
    }

    Formhandler.GetXmlElementsFromUrl = getXmlElementsFromUrl;
    Formhandler.CreateBatchCommand = createBatchCommand;
    Formhandler.RemoveAction = removeAction;
    Formhandler.Reset = reset;
    Formhandler.AddAction = addAction;
}(Importer.Formhandler = {}));