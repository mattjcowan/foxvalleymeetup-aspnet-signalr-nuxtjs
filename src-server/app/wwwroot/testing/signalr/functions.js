"use strict";

function isJson(item) {
    item = typeof item !== "string"
        ? JSON.stringify(item)
        : item;

    try {
        item = JSON.parse(item);
    } catch (e) {
        return false;
    }

    return (typeof item === "object" && item !== null);
}

function toUlOrString(obj) {
    var str = ''
    if (_.isArray(obj)) {
        str += '<ul>';
        obj.forEach(function (item) {
            str += `<li>${toUlOrString(item)}</li>`;
        });
        str += '</ul>';
    } else if (_.isObject(obj)) {
        str += '<ul>';
        Object.keys(obj).forEach(function (k) {
            str += `<li><strong>${k}</strong>: ${toUlOrString(obj[k])}</li>`;
        });
        str += '</ul>';
    } else {
        str = DOMPurify.sanitize(obj);
    }
    return str;
}

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

function convertToDiv(action, data) {
    data = data || '';
    var htmlStr = '<div class="row no-gutters">';
    if (action === 'card') {
        if (data) {
            if (!isJson(data)) {
                data = { body: data }
            }
            if (data.img) {
                htmlStr += `<div class="col-md-3"><img src="${DOMPurify.sanitize(data.img)}" class="card-img" alt="${DOMPurify.sanitize(data.title || '')}"></img></div>`
                htmlStr += '<div class="col-md-9">'
            } else {
                htmlStr += '<div class="col-md-12">'
            }
            htmlStr += '<div class="card-body">'
            if (data.title) {
                htmlStr += `<h5 class="card-title">${DOMPurify.sanitize(data.title)}</h5>`
            }
            if (data.subtitle) {
                htmlStr += `<h6 class="card-subtitle mb-2 text-muted">${DOMPurify.sanitize(data.subtitle)}</h6>`
            }
            if (data.body) {
                htmlStr += `<p class="card-text">${DOMPurify.sanitize(data.body)}</p>`
            }
            if (data.links) {
                data.links.forEach(function (link) {
                    htmlStr += `<a `;
                    Object.keys(link).forEach(function (k) {
                        htmlStr += `${k}=${link[k]} `;
                    });
                    if (!link.href) htmlStr += 'href="#" ';
                    htmlStr += ` class="card-link">${link.title || 'Details'}</a> `;
                })
            }
            htmlStr += '</div></div>';
        }
    } else {
        if (data) {
            htmlStr += '<div class="col-md-12"><div class="card-body"><p class="card-text">'
            htmlStr += toUlOrString(data);
            htmlStr += '</p></div></div>';
        }
    }
    htmlStr += '</div>';

    var div = document.createElement('div');
    div.classList.add("card");
    div.classList.add("mb-3");
    //div.style = 'display: inline-block;'
    //var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
    //div.textContent = htmlStr.trim();
    div.innerHTML = htmlStr.trim();
    return div
}

function convertFormToMessage(form) {
    var message = {
        data: {}
    }
    var formData = Array.from(new FormData(form))
    for (var i = 0; i < formData.length; i++) {
        var k = formData[i][0]
        var v = formData[i][1]
        if (k === 'data') {
            if (isJson(v)) {
                message.data = JSON.parse(v);
            } else {
                message.data['body'] = v
            }
        } else {
            message[k] = v
        }
    }
    return message;
}

function sampleData() {
    document.getElementById('dataInput')
        .value = JSON.stringify({
            title: 'Title of a card',
            subtitle: 'Subtitle goes here',
            img: 'https://placekitten.com/200/250',
            body: 'This is the body of a card',
            links: [
                { title: 'Vue.js', href: 'https://vuejs.org', target: '_blank' },
                { title: 'Nuxt.js', href: 'https://nuxtjs.org', target: '_blank' }
            ]
        }, null, '  ');
    document.getElementById('actionInput')
        .value = 'card';
}