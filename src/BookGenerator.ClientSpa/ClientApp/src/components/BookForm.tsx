import React, { useState } from 'react';

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
        <div className="flex flex-col h-screen justify-center">
            <h1 className="text-center">Book Generator</h1>
            <form className="flex flex-row flex-nowrap" onSubmit={handleSubmit} role="form">
                <label className="flex-none" htmlFor="book-title">Title</label>
                <input className="grow" type="text" id="bookTitleInput"
                    data-testid="bookTitleInput" value={title}
                    placeholder="Enter a book title"
                    onChange={handleChange} disabled={isFormDisabled} />
                <button className="flex-none" type="submit" id="generateButton"
                    data-testid="generateButton" disabled={isFormDisabled}>Generate</button>
            </form>
        </div>
    );
}

export default BookForm;