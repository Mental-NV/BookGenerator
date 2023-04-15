import React from 'react';
import { useParams } from 'react-router-dom';

interface RouteParams {
    bookId: string;
}

const BookStatus: React.FC = () => {
    const { bookId } = useParams<RouteParams>();

    return (
        <div>
            <h1>Book Status</h1>
            <p>Book ID: {bookId}</p>
        </div>
    );
};

export default BookStatus;