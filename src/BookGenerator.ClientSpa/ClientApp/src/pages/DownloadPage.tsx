import React from 'react';
import { useParams } from 'react-router-dom';

interface RouteParams {
    bookId: string;
}

const DownloadPage: React.FC = () => {
    const { bookId } = useParams<RouteParams>();
    return (
        <div>
            <h1>Download Page</h1>
            <p>Your book with ID {bookId} is ready for download.</p>
        </div>
    );
}

export default DownloadPage;