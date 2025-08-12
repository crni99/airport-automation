import React, { useState, useContext, useEffect } from 'react';
import { DataContext } from '../../store/data-context';
import { getAuthToken, authenticateUser } from '../../utils/auth';
import Alert from '../common/Alert';
import LoadingSpinner from './LoadingSpinner';

export default function Home() {
    const dataCtx = useContext(DataContext);
    const isLoggedIn = getAuthToken() !== null;
    const [userName, setUserName] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState(null);
    const [loading, setLoading] = useState(false);

    const [currentDateTime, setCurrentDateTime] = useState('');

    const handleFormSubmit = async (event) => {
        event.preventDefault();
        setLoading(true);
        setError(null);
        try {
            const authError = await authenticateUser(userName, password, dataCtx.apiUrl);
            if (authError) {
                setError(authError.message);
            }
        } catch (error) {
            console.error('Error:', error);
            setError('An unexpected error occurred. Please try again later.');
        } finally {
            setLoading(false);
        }
    };

    useEffect(() => {
        const updateDateTime = () => {
            const now = new Date();
            const dateOptions = { weekday: 'long', year: 'numeric', month: 'long', day: 'numeric' };
            const timeOptions = { hour12: false };
            const dateTime = `${now.toLocaleDateString([], dateOptions)} - ${now.toLocaleTimeString([], timeOptions)}`;
            setCurrentDateTime(dateTime);
        };

        updateDateTime();

        const intervalId = setInterval(updateDateTime, 1000);

        return () => {
            clearInterval(intervalId);
        };
    }, []);

    return (
        <div className="container">
            <br />
            <br />
            <br />
            <main role="main" className="pb-3">
                <div>
                    <div className="form-horizontal">
                        <div className="form-group">
                            <h1>Airport Automation React</h1>
                            <p>{currentDateTime}</p>
                        </div>
                    </div>
                    {!isLoggedIn &&
                        <div className="row">
                            <div className="col-md-4">
                                {error && <Alert alertType="error" alertText={error} />}
                                {loading ? (
                                    <div className="d-flex justify-content-center py-4">
                                        <LoadingSpinner />
                                    </div>
                                ) : (
                                    <form onSubmit={handleFormSubmit}>
                                        <div className="form-group pb-3">
                                            <label htmlFor="UserName" className="control-label">Username</label>
                                            <input
                                                id="UserName"
                                                name="UserName"
                                                maxLength="50"
                                                className="form-control"
                                                required
                                                value={userName}
                                                onChange={(e) => setUserName(e.target.value)}
                                            />
                                        </div>
                                        <div className="form-group pb-4">
                                            <label htmlFor="Password" className="control-label">Password</label>
                                            <input
                                                id="Password"
                                                name="Password"
                                                type="password"
                                                maxLength="50"
                                                className="form-control"
                                                required
                                                value={password}
                                                onChange={(e) => setPassword(e.target.value)}
                                            />
                                        </div>
                                        <div className="form-group">
                                            <button type="submit" className="btn btn-primary">Sign In</button>
                                        </div>
                                    </form>
                                )}
                            </div>
                        </div>
                    }
                </div>
            </main>
        </div>
    );
}
