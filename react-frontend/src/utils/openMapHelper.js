export default function openMap(address) {
    const mapsUrl = "https://www.google.com/maps/search/?api=1&query=" + address;
    window.open(mapsUrl, '_blank');
}