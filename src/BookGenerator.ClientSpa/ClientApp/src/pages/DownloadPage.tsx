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
            <div className="flex flex-col min-h-screen justify-center items-center">
                <div className="w-full max-w-2xl h-52 flex flex-col">
                    <h1 className="text-center text-slate-900 font-bold text-3xl mb-4">Download Page</h1>
                    <p className="text-center">
                        Your book with ID {bookId} is downloading.
                    </p>
                </div>
            </div>
        );
    }

    return <DownloadForm bookFile={bookFile} />;
}

export default DownloadPage;