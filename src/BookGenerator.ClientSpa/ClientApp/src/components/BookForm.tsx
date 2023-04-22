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
        <div className="container h-screen mx-auto py-8">
            <h1 className="text-2xl font-bold text-center mb-6 mt-8">Book Generator</h1>
            <form className="w-full max-w-sm mx-auto bg-white p-8 rounded-md shadow-md" onSubmit={handleSubmit} role="form">
                <label className="hidden text-gray-700 text-sm font-bold mr-2" htmlFor="book-title">Title</label>
                <input className="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:border-indigo-500"
                    type="text" id="bookTitleInput" data-testid="bookTitleInput" value={title}
                    placeholder="Enter a book title"
                    onChange={handleChange} disabled={isFormDisabled} />
                <button className="bg-indigo-500 text-white text-sm font-bold py-2 px-4 rounded-md ml-5 hover:bg-indigo-600 transition duration-300"
                    type="submit" id="generateButton" data-testid="generateButton" disabled={isFormDisabled}>Generate</button>
            </form>
        </div>
    );
}

export default BookForm;