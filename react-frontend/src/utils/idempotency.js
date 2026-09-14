export function generateIdempotencyKey(resourceType) {
    const uuid = (typeof crypto !== 'undefined' && crypto.randomUUID)
        ? crypto.randomUUID()
        : `${Date.now()}-${Math.random().toString(36).slice(2)}`;

    return `${resourceType}-${uuid}`;
}

async function sha256Base64(str) {
    const data = new TextEncoder().encode(str);
    const hashBuffer = await crypto.subtle.digest('SHA-256', data);
    const bytes = new Uint8Array(hashBuffer);
    const binary = Array.from(bytes, b => String.fromCharCode(b)).join('');
    return btoa(binary);
}

function cleanBase64(b64) {
    return b64.replace(/\+/g, '').replace(/\//g, '').replace(/=/g, '');
}

export async function generateContentIdempotencyKey(resourceType, data) {
    const json = JSON.stringify(data);
    const hash = await sha256Base64(json);
    return `${resourceType}-${cleanBase64(hash).substring(0, 12).toLowerCase()}`;
}