import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import { getBookFile, BookFile } from '../api/bookApi';
import DownloadForm from '../components/DownloadForm';
import styles from './DownloadPage.module.css';

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
            <div className={styles.container}>
                <h1 className={styles.heading}>Download Page</h1>
                <p className={styles.loadingText}>
                    Your book with ID {bookId} is downloading.
                </p>
            </div>
        );
    }

    return <DownloadForm bookFile={bookFile} />;
}

export default DownloadPage;