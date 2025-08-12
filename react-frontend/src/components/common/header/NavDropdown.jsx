export default function NavDropdown({ type, text, items }) {
    return (
        <li className="nav-item dropdown">
            <div className="btn-group">
                <a className="nav-link" href={`/${type}`}>{text}</a>
                <button
                    type="button"
                    className="btn btn-link dropdown-toggle dropdown-toggle-split"
                    data-bs-toggle="dropdown"
                    aria-haspopup="true"
                    aria-expanded="false"
                >
                    <span className="visually-hidden">Toggle Dropdown</span>
                </button>
                <ul className="dropdown-menu">
                    {items.map((item, index) => (
                        <li key={index}>
                            <a className="dropdown-item" href={`/${type}/${item.action}`}>{item.label}</a>
                        </li>
                    ))}
                </ul>
            </div>
        </li>
    );
}
