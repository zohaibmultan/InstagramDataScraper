async function fetchUserInfo(username) {
    var url = `https://www.instagram.com/api/v1/users/web_profile_info/?username=${username}`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                "x-ig-app-id": "936619743392459",
                "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/62.0.3202.94 Safari/537.36",
                "Accept-Language": "en-US,en;q=0.9,ru;q=0.8",
                "Accept-Encoding": "gzip, deflate, br",
                "Accept": "*/*",
            }
        });

        const result = {
            status: true,
            data: await response.json()
        };

        console.log(result);
        window.chrome.webview.postMessage(result);
    }
    catch (error) {
        const result = {
            status: false,
            error: error.message
        };
        console.log(result);
        window.chrome.webview.postMessage(result);
    }
}

//fetchUserInfo('iammadabbasi');