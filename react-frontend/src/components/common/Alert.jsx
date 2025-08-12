export default function Alert({ alertType, children }) {
    let alertClasses = "text-center mt-4 alert ";

    if (alertType === 'success') {
        alertClasses += "alert-success";
    } else if (alertType === 'info') {
        alertClasses += "alert-info";
    } else if (alertType === 'danger' || alertType === 'error') {
        alertClasses += "alert-danger";
    } else {
        console.warn('Alert type does not exist:', alertType);
    }

    return (
        <div className={alertClasses} role="alert">
            {children}
        </div>
    );
}
