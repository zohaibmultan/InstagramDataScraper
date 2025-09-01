async function fetchUserInfo(username, requestId) {
    var url = `https://www.instagram.com/api/v1/users/web_profile_info/?username=${username}`;

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'Host': 'www.instagram.com',
                'X-Requested-With': 'XMLHttpRequest',
                'Sec-Ch-Prefers-Color-Scheme': 'dark',
                'Sec-Ch-Ua-Platform': '"Linux"',
                'X-Ig-App-Id': '936619743392459',
                'Sec-Ch-Ua-Model': '""',
                'Sec-Ch-Ua-Mobile': '?0',
                'User-Agent': 'Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/122.0.6261.112 Safari/537.36',
                'Accept': '*/*',
                'X-Asbd-Id': '129477',
                'Sec-Fetch-Site': 'same-origin',
                'Sec-Fetch-Mode': 'cors',
                'Sec-Fetch-Dest': 'empty',
                'Referer': 'https://www.instagram.com/',
                'Accept-Language': 'en-US,en;q=0.9',
                'Priority': 'u=1, i',
            },
            credentials: 'include'
        });

        const result = {
            status: true,
            requestId: requestId,
            data: await response.json()
        };

        console.log(result);
        window.chrome.webview.postMessage(result);
    }
    catch (error) {
        const result = {
            status: false,
            questId: requestId,
            error: error.message
        };
        console.log(result);
        window.chrome.webview.postMessage(result);
    }
}

//fetchUserInfo('iammadabbasi', 'abc123');