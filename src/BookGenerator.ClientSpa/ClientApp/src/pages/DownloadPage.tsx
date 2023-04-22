import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getBookFile, BookFile } from '../api/bookApi';
import DownloadForm from '../components/DownloadForm';

interface RouteParams {
    bookId: string;
}

const DownloadPage: React.FC = () => {
    const { bookId } = useParams<RouteParams>();
    const [bookFile, setBookFile] = useState<BookFile | null>(null);

    useEffect(() => {
        const fetchBookFile = async () => {
            const bookFileData = await getBookFile(bookId);
            setBookFile(bookFileData);
        };

        fetchBookFile();
    }, [bookId]);

    if (!bookFile) {
        return (
            <div>
                <h1>Download Page</h1>
                <p>
                    Your book with ID {bookId} is downloading.
                </p>
            </div>
        );
    }

    return <DownloadForm bookFile={bookFile} />;
}

export default DownloadPage;