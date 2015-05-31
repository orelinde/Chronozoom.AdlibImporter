(function (Webhandler) {

    /* This method will call the Axiell api with the given url.
    It will try to get a record so that we can take te xml elements*/
    var getXmlElementsFromUrl = function (url,database,callback) {
        $.ajax({
            url: url + 'wwwopac.ashx?&database=' + database + '&command=search&output=json&limit=1&search=all&xmltype=unstructured',
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
    }

    var getMetadata = function (url,database, callback) {
        $.ajax({
            url: url + 'wwwopac.ashx?&database=' + database + '&output=json&command=getmetadata',
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
    }


    // Public properties
    Webhandler.GetXmlElementsFromUrl = getXmlElementsFromUrl;
    Webhandler.GetMetadata = getMetadata;

}(Importer.Webhandler = {}));