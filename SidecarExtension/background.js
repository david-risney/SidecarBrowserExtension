var port = chrome.runtime.connectNative('net.deletethis.myhost');
port.onMessage.addListener(function (msg) {
    console.log('Received', msg);
    handleMessage(msg);
});
port.onDisconnect.addListener(function () {
    console.log('Disconnected');
});

async function searchHistory(args) {
    let resolve, reject;
    const promise = new Promise((resolveIn, rejectIn) => {
        resolve = resolveIn;
        reject = rejectIn;
    });
    chrome.history.search(args, result => {
        const resultObj = { result };
        resolve(resultObj);
    });
    return promise;
}

async function queryTabs(args) {
    let resolve, reject;
    const promise = new Promise((resolveIn, rejectIn) => {
        resolve = resolveIn;
        reject = rejectIn;
    });

    chrome.tabs.query(args, result => {
        const resultObj = { result };
        resolve(resultObj);
    });

    return promise;
}

async function updateTab(args) {
    let resolve, reject;
    const promise = new Promise((resolveIn, rejectIn) => {
        resolve = resolveIn;
        reject = rejectIn;
    });

    chrome.tabs.update(args.tabId, args.updateProperties, result => {
        resolve(result);
    });

    return promise;
}

async function handleMessage(msg) {
    switch (msg.action) {
        case "searchHistory":
            port.postMessage(await searchHistory(msg.args));
            break;

        case "queryTabs":
            port.postMessage(await queryTabs(msg.args));
            break;

        case "updateTab":
            port.postMessage(await updateTab(msg.args));
            break;

        default:
            console.log("Unhandled message action: " + msg.action);
            port.postMessage(msg);
            break;
    }
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