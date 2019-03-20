const webshot = require('webshot')
const os = require("os")

module.exports = function (callback, url, imageName) {
    // const optionsMobile = {
    //     screenSize: {
    //         width: 400,
    //         height: 800
    //     },
    //     shotSize: {
    //         width: 400,
    //         height: 'all'
    //     },
    //     userAgent: 'Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.20 (KHTML, li$
    // }
    // most common based on: https://www.rapidtables.com/web/dev/screen-resolution-statistics.html
    const options = {
        screenSize: {
            width: 1366,
            height: 768
        },
        shotSize: {
            width: 1366,
            height: 768
        }
    }
    if (os.type() === 'Linux') {
        options.phantomPath = '/usr/local/bin/phantomjs'
    }
    webshot(url, imageName, options, function (err) {
        if (!err) {
            callback(null, imageName);
        } else {
            callback(err, imageName);
        }
    });
};
