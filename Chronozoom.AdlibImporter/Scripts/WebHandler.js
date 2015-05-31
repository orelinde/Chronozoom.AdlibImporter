(function (Webhandler) {
    var getXmlElementsFromUrl = function (url) {
        xml = new XMLHttpRequest();
        xml.open("GET", url, true);
        xml.send();
        console.log(xml.responseText);
    }

    Webhandler.GetXmlElementsFromUrl = getXmlElementsFromUrl;

}(Importer.Webhandler = {}));