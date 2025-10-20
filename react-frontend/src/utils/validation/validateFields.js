import { validateField } from "./validation";

export const validateFields = (type, formData, fields) => {
    const errors = {}; 
    fields.forEach((field) => {
        const error = validateField(type, field, formData[field]); 
        if (error) {
            errors[field] = error; 
        }
    });
    return Object.keys(errors).length > 0 ? errors : null;
};