async function fetchFollowing(requestId, user_pk, username, max) {
    var url = `https://www.instagram.com/api/v1/friendships/${user_pk}/following/?count=12`;
    url += max ? `&max_id=${max}` : '';

    try {
        const response = await fetch(url, {
            method: 'GET',
            headers: {
                'accept': '*/*',
                'accept-encoding': 'gzip, deflate, br, zstd',
                'accept-language': 'en-US,en;q=0.9',
                'referer': `https://www.instagram.com/${username}/following/`,
                'user-agent': 'Mozilla/5.0 (iPhone; CPU iPhone OS 18_6_2 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/18.4 Mobile/15E148 Safari/604.1',
                'x-ig-app-id': '1217981644879628',
                'x-requested-with': 'XMLHttpRequest'
            },
            credentials: 'include'
        });
        const data = {
            status: true,
            requestId: requestId,
            data: await response.json()
        };
        //const json = JSON.stringify(data);
        window.chrome.webview.postMessage(data);
    }
    catch (error) {
        window.chrome.webview.postMessage(JSON.stringify({
            status: false,
            questId: requestId,
            error: error.message
        }));
    }
}

//fetchFollowing('1','2459396194','iammadabbasi');