import React, { useState } from 'react'
import { createBook } from '../api/bookApi';

interface BookFormProps {
    onGenerate: (bookId: string) => void;
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

        try {
            const response = await createBook(title);
            onGenerate(response.BookId);
        } catch (error) {
            console.error('Error generating book:', error);
        }
    };

    return (
        <form onSubmit={handleSubmit}>
            <label htmlFor="book-title">Title</label>
            <input type="text" id="book-title" value={title} onChange={handleChange} disabled={isFormDisabled} />
            <button type="submit" disabled={isFormDisabled}>Generate</button>
        </form>
    );
}

export default BookForm;