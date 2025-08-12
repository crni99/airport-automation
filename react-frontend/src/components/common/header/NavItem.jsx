export default function NavItem({ type, text }) {
    return (
        <li className="nav-item">
            <a className="nav-link" href={`/${type}`}>{text}</a>
        </li>
    );
}