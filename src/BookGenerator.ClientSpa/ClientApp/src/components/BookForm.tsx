import React, { useState } from 'react'

interface BookFormProps {
    onGenerate: (title: string) => void;
}

const BookForm: React.FC<BookFormProps> = ({ onGenerate }) => {
    const [title, setTitle] = useState('');
    const [isFormDisabled, setIsFormDisabled] = useState(false);

    const handleChange = (event: React.ChangeEvent<HTMLInputElement>) => {
        setTitle(event.target.value);
    };

    const handleSubmit = async (event: React.FormEvent<HTMLFormElement>) => {
        event.preventDefault();
        setIsFormDisabled(true);
        onGenerate(title);
    };

    return (
        <div>
            <h1>Book Generator</h1>
            <form onSubmit={handleSubmit}>
                <label htmlFor="book-title">Title</label>
                <input type="text" id="book-title" value={title} onChange={handleChange} disabled={isFormDisabled} />
                <button type="submit" disabled={isFormDisabled}>Generate</button>
            </form>
        </div>
    );
}

export default BookForm;