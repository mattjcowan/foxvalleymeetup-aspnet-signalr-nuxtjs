const webshot = require('webshot')

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
    //     userAgent: 'Mozilla/5.0 (iPhone; U; CPU iPhone OS 3_2 like Mac OS X; en-us) AppleWebKit/531.21.20 (KHTML, like Gecko) Mobile/7B298g'
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
        },
        userAgent: 'Mozilla/5.0 (iPad; CPU OS 11_0 like Mac OS X) AppleWebKit/604.1.34 (KHTML, like Gecko) Version/11.0 Mobile/15A5341f Safari/604.1'
    }
    webshot(url, imageName, options, function (err) {
        if (!err) {
            callback(null, imageName);
        } else {
            callback(err, imageName);
        }
    });
};