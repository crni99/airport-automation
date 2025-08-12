import { validateField } from "./validation";

export const validateFields = (type, formData, fields) => {
    const errorMessages = [];

    fields.forEach((field) => {
        const error = validateField(type, field, formData[field]);
        if (error) {
            errorMessages.push(error);
        }
    });

    return errorMessages.length > 0 ? errorMessages.join(' ') : null;
};