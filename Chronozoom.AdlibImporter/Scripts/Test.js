(function (Test) {
    Test.get = function () {
        console.log("IIFE");
    }
}(Importer.Test = {}));