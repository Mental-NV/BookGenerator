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
        <div className="flex flex-col min-h-screen justify-center items-center">
            <div className="w-full max-w-2xl h-24 flex flex-col">
                <h1 className="text-center text-slate-900 font-bold text-3xl mb-6">Book Generator</h1>
                <form className="flex flex-row flex-nowrap" onSubmit={handleSubmit} role="form">
                    <label className="flex-none hidden" htmlFor="book-title">Title</label>
                    <input className="grow" type="text" id="bookTitleInput"
                        data-testid="bookTitleInput" value={title}
                        placeholder="Enter a book title"
                        onChange={handleChange} disabled={isFormDisabled} />
                    <button className="flex-none" type="submit" id="generateButton"
                        data-testid="generateButton" disabled={isFormDisabled}>Generate</button>
                </form>
            </div>
        </div>
    );
}

export default BookForm;