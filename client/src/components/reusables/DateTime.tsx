import React from 'react';

interface DateTimeInputProps {
    value: Date | undefined;
    onChange: (date: Date) => void;
    className?: string;
    required?: boolean;
    disabled?: boolean;
}


const DateTimeInput: React.FC<DateTimeInputProps> = ({
                                                         value,
                                                         onChange,
                                                         className = '',
                                                         required = false,
                                                         disabled = false
                                                     }) => {
    // Format Date for date input (YYYY-MM-DD)
    const getDateValue = (): string => {
        if (!value) return '';
        const year = value.getFullYear();
        const month = String(value.getMonth() + 1).padStart(2, '0');
        const day = String(value.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    // Format Date for time input (HH:MM)
    const getTimeValue = (): string => {
        if (!value) return '';
        const hours = String(value.getHours()).padStart(2, '0');
        const minutes = String(value.getMinutes()).padStart(2, '0');
        return `${hours}:${minutes}`;
    };

    // Handle date change
    const handleDateChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const dateVal = e.target.value;
        if (!dateVal) return;

        const newDate = value ? new Date(value) : new Date();
        const [year, month, day] = dateVal.split('-').map(num => parseInt(num));

        newDate.setFullYear(year);
        newDate.setMonth(month - 1); // Month is 0-indexed in JS Date
        newDate.setDate(day);

        onChange(newDate);
    };

    // Handle time change
    const handleTimeChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        const timeVal = e.target.value;
        if (!timeVal) return;

        const newDate = value ? new Date(value) : new Date();
        const [hours, minutes] = timeVal.split(':').map(num => parseInt(num));

        newDate.setHours(hours);
        newDate.setMinutes(minutes);

        onChange(newDate);
    };

    return (
        <div className="flex gap-2">
            <input
                type="date"
                className={className}
                value={getDateValue()}
                onChange={handleDateChange}
                required={required}
                disabled={disabled}
            />
            <input
                type="time"
                className={className}
                value={getTimeValue()}
                onChange={handleTimeChange}
                required={required}
                disabled={disabled}
            />
        </div>
    );
};

export default DateTimeInput;