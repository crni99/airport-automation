import React from 'react';
import { Entities } from '../../utils/const';
import SearchButton from './search/SearchButton';
import ClearInputButton from './search/ClearInputsButton';
import ExportButtons from './ExportButtons';

export default function SearchInputWithButton({ type, setTriggerFetch, isUser }) {

    const handleSearch = () => {
        setTriggerFetch(true);
    };

    const exportButtonsOrSpace = isUser === 'User' ? (
        <>&nbsp;</>
    ) : (
        <ExportButtons dataType={type} />
    );

    const renderInput = () => {
        switch (type) {
            case Entities.AIRLINES:
                return (
                    <>
                        <div className='me-3'>
                            {exportButtonsOrSpace}
                        </div>
                        <div className="input-group me-3">
                            <label htmlFor="searchInput" className="input-group-text">Search by Name:</label>
                            <input
                                type="text"
                                id="searchInput"
                                className="form-control"
                                placeholder="Air Serbia"
                            />
                        </div>
                        <div className='me-3'>
                            <SearchButton onClick={handleSearch} />
                        </div>
                        <ClearInputButton />
                    </>
                );

            case Entities.API_USERS:
                return (
                    <div className='container-fluid'>
                        <div className="row mb-2">
                            <div className="col-md-4 mb-2">
                                <div className="input-group">
                                    <label htmlFor="roleSelect" className="input-group-text">Role:</label>
                                    <select id="roleSelect" className="form-control">
                                        <option value="">Select a role</option>
                                        <option value="User">User</option>
                                        <option value="Admin">Admin</option>
                                        <option value="SuperAdmin">Super Admin</option>
                                    </select>
                                </div>
                            </div>
                            <div className="col-md-4 mb-2">
                                <div className="input-group">
                                    <label htmlFor="username" className="input-group-text">Username:</label>
                                    <input type="text" id="username" className="form-control" placeholder="Enter username" />
                                </div>
                            </div>
                            <div className="col-md-4 mb-2">
                                <div className="input-group">
                                    <label htmlFor="password" className="input-group-text">Password:</label>
                                    <input type="text" id="password" className="form-control" placeholder="Enter password" />
                                </div>
                            </div>
                        </div>
                        <div className='row d-flex align-items-center justify-content-end'>
                            <div className='col-md-2 mb-2'></div>
                            <div className='col-md-2 mb-2'></div>
                            <div className='col-md-2 mb-2'></div>
                            <div className='col-md-2 mb-2'></div>
                            <div className='col-md-2 mb-2'>
                                <div className="input-group me-3">
                                    <ClearInputButton />
                                </div>
                            </div>
                            <div className='col-md-2 mb-2'>
                                <div className="input-group me-3 d-flex align-items-center justify-content-end">
                                    <SearchButton onClick={handleSearch} />
                                </div>
                            </div>
                        </div>
                    </div>
                );

            case Entities.DESTINATIONS:
                return (
                    <>
                        <div className='me-3'>
                            {exportButtonsOrSpace}
                        </div>
                        <div className="input-group me-3">
                            <label htmlFor="city" className="input-group-text">City:</label>
                            <input
                                type="text"
                                id="city"
                                className="form-control"
                                placeholder="Belgrade"
                            />
                        </div>
                        <div className="input-group me-3">
                            <label htmlFor="airport" className="input-group-text">Airport:</label>
                            <input
                                type="text"
                                id="airport"
                                className="form-control"
                                placeholder="Belgrade Nikola Tesla"
                            />
                        </div>
                        <div className="me-3">
                            <SearchButton onClick={handleSearch} />
                        </div>
                        <ClearInputButton />
                    </>
                );

            case Entities.FLIGHTS:
                return (
                    <div className="container-fluid">
                        <div className="row mb-3 align-items-end">
                            <div className="col-12 col-md-5 mb-2">
                                <div className="input-group">
                                    <label htmlFor="startDate" className="input-group-text">Start Date</label>
                                    <input
                                        type="date"
                                        id="startDate"
                                        className="form-control"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-5 mb-2">
                                <div className="input-group">
                                    <label htmlFor="endDate" className="input-group-text">End Date</label>
                                    <input
                                        type="date"
                                        id="endDate"
                                        className="form-control"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-2 mb-2 text-md-end">
                                <SearchButton onClick={handleSearch} />
                            </div>
                        </div>
                        <div className="row align-items-center">
                            <div className="col-6 col-md-2 mb-2">
                                {exportButtonsOrSpace}
                            </div>
                            <div className="col-6 col-md-2 offset-md-8 mb-2 text-end">
                                <ClearInputButton />
                            </div>
                        </div>
                    </div>
                );

            case Entities.PASSENGERS:
                return (
                    <div className="container-fluid">
                        <div className="row mb-3 align-items-end">
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="firstName" className="input-group-text">First Name:</label>
                                    <input
                                        type="text"
                                        id="firstName"
                                        className="form-control"
                                        placeholder="Ognjen"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="lastName" className="input-group-text">Last Name:</label>
                                    <input
                                        type="text"
                                        id="lastName"
                                        className="form-control"
                                        placeholder="Andjelic"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="uprn" className="input-group-text">UPRN:</label>
                                    <input
                                        type="text"
                                        id="uprn"
                                        className="form-control"
                                        placeholder="1234567890123"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2 d-flex justify-content-md-end">
                                {exportButtonsOrSpace}
                            </div>
                        </div>
                        <div className="row align-items-end">
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="passport" className="input-group-text">Passport:</label>
                                    <input
                                        type="text"
                                        id="passport"
                                        className="form-control"
                                        placeholder="P12345678"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="address" className="input-group-text">Address:</label>
                                    <input
                                        type="text"
                                        id="address"
                                        className="form-control"
                                        placeholder="123 Main Street, New York, United States"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="phone" className="input-group-text">Phone:</label>
                                    <input
                                        type="number"
                                        id="phone"
                                        className="form-control"
                                        placeholder="123-456-7890"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2 d-flex justify-content-md-end gap-4">
                                <SearchButton onClick={handleSearch} />
                                <ClearInputButton />
                            </div>
                        </div>
                    </div>
                );

            case Entities.PILOTS:
                return (
                    <div className="container-fluid">
                        <div className="row mb-2">
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="firstName" className="input-group-text">First Name:</label>
                                    <input
                                        type="text"
                                        id="firstName"
                                        className="form-control"
                                        placeholder="Ognjen"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="lastName" className="input-group-text">Last Name:</label>
                                    <input
                                        type="text"
                                        id="lastName"
                                        className="form-control"
                                        placeholder="Andjelic"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="uprn" className="input-group-text">UPRN:</label>
                                    <input
                                        type="number"
                                        id="uprn"
                                        className="form-control"
                                        placeholder="1234567890123"
                                    />
                                </div>
                            </div>
                            <div className="col-12 col-md-3 mb-2">
                                <div className="input-group">
                                    <label htmlFor="flyingHours" className="input-group-text">Flying Hours:</label>
                                    <input
                                        type="number"
                                        id="flyingHours"
                                        className="form-control"
                                        placeholder="10"
                                    />
                                </div>
                            </div>
                        </div>
                        <div className="row mb-3">
                            <div className="col-12 d-flex justify-content-md-end gap-4">
                                {exportButtonsOrSpace}
                                <ClearInputButton />
                                <SearchButton onClick={handleSearch} />
                            </div>
                        </div>
                    </div>
                );

            case Entities.PLANE_TICKETS:
                return (
                    <div className='container-fluid'>
                        <div className='row mb-2'>
                            <div className='col-md-4 mb-2'>
                                <div className="input-group me-3">
                                    <label htmlFor="price" className="input-group-text">Price:</label>
                                    <input
                                        type="number"
                                        id="price"
                                        className="form-control"
                                        placeholder="100"
                                    />
                                </div>
                            </div>
                            <div className='col-md-4 mb-2'>
                                <div className="input-group me-3">
                                    <label htmlFor="purchaseDate" className="input-group-text">Purchase Date:</label>
                                    <input
                                        type="date"
                                        id="purchaseDate"
                                        className="form-control"
                                    />
                                </div>
                            </div>
                            <div className='col-md-4 mb-2'>
                                <div className="input-group me-3">
                                    <label htmlFor="seatNumber" className="input-group-text">Seat Number:</label>
                                    <input
                                        type="number"
                                        id="seatNumber"
                                        className="form-control"
                                        placeholder="16"
                                    />
                                </div>
                            </div>
                        </div>
                        <div className='row d-flex align-items-center justify-content-end'>
                            <div className='col-md-2 mb-2'></div>
                            <div className='col-md-2 mb-2'></div>
                            <div className='col-md-2 mb-2'>
                                {exportButtonsOrSpace}
                            </div>
                            <div className='col-md-2 mb-2'></div>
                            <div className='col-md-2 mb-2'>
                                <div className="input-group me-3">
                                    <ClearInputButton />
                                </div>
                            </div>
                            <div className='col-md-2 mb-2'>
                                <div className="input-group me-3 d-flex align-items-center justify-content-end">
                                    <SearchButton onClick={handleSearch} />
                                </div>
                            </div>
                        </div>
                    </div>
                );

            case Entities.TRAVEL_CLASSES:
                return (
                    <div className='mb-3'>
                        {exportButtonsOrSpace}
                    </div>
                );

            default:
                return (
                    <div className="input-group me-3">
                    </div>
                );
        }
    };

    return renderInput();
}