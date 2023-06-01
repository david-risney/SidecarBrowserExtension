/*

msg in
    sourceId - unique ID of sender
    messageId - ID of message
    action - action string enum
    args - JSON object action specific args

msg out
    targetId - unique ID of sender (matches msg in)
    messageId - ID of message (matches msg in)
    success - boolean if it was success or not
    result - JSON result object for success or error text for failure
    

*/

const port = chrome.runtime.connectNative('net.deletethis.myhost');
port.onMessage.addListener(function (msg) {
    console.log('Received', msg);
    handleMessage(msg);
});
port.onDisconnect.addListener(function () {
    console.log('Disconnected');
});

const nameToHandlerMap = {};

// https://developer.chrome.com/docs/extensions/reference/bookmarks/#method-search
// query
// title (exact match)
// url (exact match)
nameToHandlerMap.searchBookmarks = function (args, resolve) {
    chrome.bookmarks.search(args, resolve);
}

// https://developer.chrome.com/docs/extensions/reference/history/#method-search
// text
// startTime
// endTime
// maxResults (100)
nameToHandlerMap.searchHistory = function (args, resolve) {
    chrome.history.search(args, resolve);
}

// https://developer.chrome.com/docs/extensions/reference/tabs/#method-query
// title
// url
nameToHandlerMap.queryTabs = function (args, resolve) {
    chrome.tabs.query(args, resolve);
}

// https://developer.chrome.com/docs/extensions/reference/tabs/#method-update
// tabId
// updateProperties {
//    active
//    url
// }
nameToHandlerMap.updateTab = function (args, resolve) {
    chrome.tabs.update(args.tabId, args.updateProperties, resolve);
}

// uri
// sizeInPixels
nameToHandlerMap.getFavicon = function (args, resolve, reject) {
    const faviconUri = new URL(chrome.runtime.getURL("/_favicon/"));
    faviconUri.searchParams.set("pageUrl", args.uri);
    faviconUri.searchParams.set("size", "" + args.sizeInPixels);
    const faviconUriAsString = faviconUri.toString();

    fetch(faviconUriAsString)
        .then(response => response.blob())
        .then(blob => {
            var reader = new FileReader();
            reader.readAsDataURL(blob);
            reader.onload = function (e) {
                resolve(e.target.result);
            };
            reader.onerror = function (e) {
                reject(e.message);
            }
        }, error => {
            reject(error);
        });
}

function handleMessage(msg) {
    let resolve, reject;
    const promise = new Promise((resolveIn, rejectIn) => {
        resolve = resolveIn;
        reject = rejectIn;
    });

    try {
        const handler = nameToHandlerMap[msg.action];
        if (handler) {
            handler(msg.args, resolve, reject);
        } else {
            throw new Error("Unknown message action '" + msg.action + "'' from message " + JSON.stringify(msg));
        }
    }
    catch (err) {
        // unknown message action error or other errors from extension APIs
        reject(err.message);
    }

    promise.then(result => {
        const reply = {
            targetId: msg.sourceId,
            messageId: msg.messageId,
            success: true,
            result
        };
        console.log("Success", reply);
        port.postMessage(reply);
    }, result => {
        const reply = {
            targetId: msg.sourceId,
            messageId: msg.messageId,
            success: false,
            result
        };
        console.log("Error", reply);
        port.postMessage(reply);
    });
}

/*
let port = browser.runtime.connectNative("SidecarNativeApp");

port.onMessage.addListener((response) => {
  console.log("Received: " + response);
});

browser.browserAction.onClicked.addListener(() => {
  console.log("Sending:  ping");
  port.postMessage("ping");
});
*/