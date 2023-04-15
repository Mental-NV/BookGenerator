import React from 'react';
import { useHistory } from 'react-router-dom'
import BookForm from './BookForm';
import { createBook } from '../api/bookApi';

const HomePage: React.FC = () => {
    const history = useHistory();

    const handleGenerate = async (title: string) => {
        try {
            const response = await createBook(title);
            history.push(`/status/${response.BookId}`);
        } catch (error) {
            console.error('Error generating book:', error);
            throw error;
        }
    }

    return <BookForm onGenerate={handleGenerate} />;
};

export default HomePage;