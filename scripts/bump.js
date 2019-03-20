const Filehound = require('filehound')
const fs = require('fs-extra')
const path = require('path')
const args = require('args-parser')(process.argv)

const increment = args.increment || 'build'
const rootDir = args.dir || 'src'
let set = args.set

Filehound.create()
    .ext('csproj')
    .paths(path.join(__dirname, '../' + rootDir))
    .find((err, csProjFiles) => {
        if (err) return console.error("handle err", err);

        csProjFiles.forEach(csProj => {
            let xml = fs.readFileSync(csProj, 'utf-8')
            if (xml.indexOf('<Version>') === -1) {
                xml = xml.replace('</TargetFramework>', '</TargetFramework>\n    <Version>1.0.0.0</Version>')
                fs.writeFileSync(csProj, xml, 'utf-8')
            }
            const start = xml.indexOf('<Version>') + 9;
            const end = xml.indexOf('</Version>');
            let cVersion = xml.substr(start, end - (start)).split('.', 4).map(s => parseInt(s.match(/\d+/g)[s.match(/\d+/g).length - 1]))

            if (increment == 'build' && cVersion.length > 3) cVersion[3]++ // = new Date(new Date().toGMTString()).getTime()
            else if (increment == 'patch' && cVersion.length > 2) { cVersion[2]++; cVersion[3] = 0 }
            else if (increment == 'minor' && cVersion.length > 1) { cVersion[1]++; cVersion[2] = 0; cVersion[3] = 0 }
            else if (increment == 'major' && cVersion.length > 0) { cVersion[0]++; cVersion[1] = 0; cVersion[2] = 0; cVersion[3] = 0 }

            const newVersion = (set ? set : cVersion.join('.'))
            xml = xml.substr(0, start) + newVersion + xml.substr(end)
            fs.writeFileSync(csProj, xml)
            console.log(`${path.basename(csProj)} set to version ${newVersion}`)
        });
    });
