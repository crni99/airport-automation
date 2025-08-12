import React from 'react';
import useFetch from "../../hooks/useFetch";
import Alert from '../common/Alert';
import { Entities } from '../../utils/const.js';

// Not working properly if there isn't connection with the API
export default function HealthCheck() {
    const { data, dataExist, error, isLoading, isError } = useFetch(Entities.HEALTH_CHECKS, null, null, null);

    const extractErrorMessage = (error) => {
        if (error && error.message) {
            return error.message;
        }
        return "An unknown error occurred";
    };

    return (
        <div className="container mt-5">
            <div className="row justify-content-between">
                <div className="col-md-6"></div>
            </div>
            <div className="form-horizontal">
                <div className="form-group">
                    {isLoading && <Alert alertType="info" alertText="Loading..." />}
                    {isError && error && <Alert alertType="error" alertText={extractErrorMessage(error)} />}
                    {data && dataExist && !isError && !error ? (
                        <div>
                            <dl className="row">
                                <dt className="col-sm-2">Status</dt>
                                <dd className="col-sm-4">
                                    {data.status === "Healthy" ? (
                                        <h5><span className="badge text-bg-success">{data.status}</span></h5>
                                    ) : (
                                        <h5><span className="badge text-bg-danger">{data.status}</span></h5>
                                    )}
                                </dd>
                                <dt className="col-sm-2">Total Duration</dt>
                                <dd className="col-sm-4">{data.totalDuration}</dd>
                            </dl>
                            <hr />
                            <br />
                            <dl className="row">
                                <dt className="col-sm-3">Name</dt>
                                <dt className="col-sm-3">Description</dt>
                                <dt className="col-sm-3">Duration</dt>
                                <dt className="col-sm-3">Status</dt>
                            </dl>
                            <hr />
                            {Object.keys(data.entries).map((key, index) => (
                                <div key={index}>
                                    <dl className="row">
                                        <dd className="col-sm-3">{key}</dd>
                                        <dd className="col-sm-3">{data.entries[key].description}</dd>
                                        <dd className="col-sm-3">{data.entries[key].duration}</dd>
                                        <dd className="col-sm-3">
                                            {data.entries[key].status === "Healthy" ? (
                                                <h5><span className="badge text-bg-success">{data.entries[key].status}</span></h5>
                                            ) : (
                                                <h5><span className="badge text-bg-danger">{data.entries[key].status}</span></h5>
                                            )}
                                        </dd>
                                    </dl>
                                    <hr />
                                </div>
                            ))}
                        </div>
                    ) : (
                        <Alert alertType="info" alertText="No data available" />
                    )}
                </div>
            </div>
        </div>
    );
}
