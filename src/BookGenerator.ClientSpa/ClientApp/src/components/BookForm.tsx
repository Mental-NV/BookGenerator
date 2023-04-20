import React, { useState } from 'react';
import styles from './BookForm.module.css';

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
        <div className={styles.container}>
            <h1>Book Generator</h1>
            <form onSubmit={handleSubmit} role="form">
                <label htmlFor="book-title">Title</label>
                <input type="text" id="bookTitleInput" data-testid="bookTitleInput" value={title} onChange={handleChange} disabled={isFormDisabled} className={styles.input} />
                <button type="submit" id="generateButton" data-testid="generateButton" disabled={isFormDisabled} className={styles.button}>Generate</button>
            </form>
        </div>
    );
}

export default BookForm;