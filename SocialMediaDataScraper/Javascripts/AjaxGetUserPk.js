async function fetchUserPk(username) {
    var url = `https://www.instagram.com/web/search/topsearch/?query=${username}`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'accept': '*/*',
                'accept-encoding': 'gzip, deflate, br, zstd',
                'accept-language': 'en-US,en;q=0.9',
                'referer': 'https://www.instagram.com/',
                'user-agent': 'Mozilla/5.0 (iPhone; CPU iPhone OS 18_6_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.4 Mobile/15E148 Safari/604.1',
            },
            credentials: 'include'
        });

        var data = {
            status: true,
            data: await response.json()
        };
        console.log(data);
        window.chrome.webview.postMessage(data);
    }
    catch (error) {
        var data = {
            status: false,
            error: error.message
        };
        console.log(data);
        window.chrome.webview.postMessage();
    }
}

//fetchUserPk('iammadabbasi');