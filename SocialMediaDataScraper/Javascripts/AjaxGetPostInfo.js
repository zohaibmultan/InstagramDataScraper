async function fetchPostInfo(shortcode, sessionId = null) {
    const INSTAGRAM_DOCUMENT_ID = "25491481827105234";
    const variables = encodeURIComponent(JSON.stringify({
        shortcode: shortcode,
        fetch_tagged_user_count: null,
        hoisted_comment_id: null,
        hoisted_reply_id: null
    }, null, 0));

    const body = `variables=${variables}&doc_id=${INSTAGRAM_DOCUMENT_ID}`;
    const url = "https://www.instagram.com/graphql/query";

    // Initialize headers
    const headers = {
        "Content-Type": "application/x-www-form-urlencoded",
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/129.0.0.0 Safari/537.36",
        "Accept": "application/json",
        "Accept-Language": "en-US,en;q=0.9",
        "x-ig-app-id": "936619743392459",
        "x-requested-with": "XMLHttpRequest",
        "Origin": "https://www.instagram.com",
        "Referer": `https://www.instagram.com/p/${shortcode}/`
    };

    // Add session cookie if provided
    if (sessionId) {
        headers["Cookie"] = `sessionid=${sessionId}`;
    }

    try {
        // Optional: Pre-flight request to establish session
        await fetch("https://www.instagram.com", {
            method: "GET",
            headers: {
                "User-Agent": headers["User-Agent"],
                "Accept": "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8",
                "Accept-Language": headers["Accept-Language"],
                "x-ig-app-id": headers["x-ig-app-id"]
            }
        });

        const response = await fetch(url, {
            method: "POST",
            headers: headers,
            body: body,
            credentials: sessionId ? "include" : "omit"
        });

        const data = await response.json();

        // Check if data contains the expected structure
        if (!data?.data?.xdt_shortcode_media) {
            console.warn("Unexpected response structure:", data);
            return null;
        }

        console.log("Post data:", data.data.xdt_shortcode_media);
        return data.data.xdt_shortcode_media;
    } catch (error) {
        console.error("Error fetching post info:", error.message);
        throw error;
    }
}

fetchPostInfo('DOgAbkYCG1o', '1994792b8f6-8641f3')
    .then(result => console.log("Post data:", result))
    .catch(error => console.error("Failed to fetch post:", error));