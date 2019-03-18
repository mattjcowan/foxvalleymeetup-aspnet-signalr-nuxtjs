var takeScreenshot = require('./nsTakeScreenshot');

var url = 'https://devblogs.microsoft.com/dotnet/announcing-net-core-3-preview-3/'
var imageName = './wwwroot/uploads/af25a89442674dfab717607cf45591bc-0.png';

var callback = function (err, file) {
    if (err) console.log('err', err);
    console.log('file', file);
}

takeScreenshot(callback, url, imageName)