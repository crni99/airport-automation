import React from 'react';
import { FontAwesomeIcon } from '@fortawesome/react-fontawesome';
import { faGithub, faLinkedin } from '@fortawesome/free-brands-svg-icons';
import { faEnvelope } from '@fortawesome/free-solid-svg-icons';

export default function Footer() {

    const currentYear = new Date().getFullYear();

    return (
        <footer className="border-top footer text-muted">
            <div className="container footer-text d-flex justify-content-between align-items-center">
                <span>{currentYear} &copy; Airport Automation React</span>
                <div>
                    <a href="https://github.com/crni99" target="_blank" rel="noopener noreferrer">
                        <FontAwesomeIcon icon={faGithub} size="lg" className="mx-2 custom-icon" title="GitHub" />
                    </a>
                    <a href="https://www.linkedin.com/in/ognj3n" target="_blank" rel="noopener noreferrer">
                        <FontAwesomeIcon icon={faLinkedin} size="lg" className="mx-2 custom-icon" title="LinkedIn" />
                    </a>
                    <a href="mailto:andjelicb.ognjen@gmail.com" target="_blank" rel="noopener noreferrer">
                        <FontAwesomeIcon icon={faEnvelope} size="lg" className="mx-2 custom-icon" title="Email" />
                    </a>
                </div>
            </div>
        </footer>
    );
}
