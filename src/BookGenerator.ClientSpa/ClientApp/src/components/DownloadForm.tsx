import React, { useMemo } from 'react';
import { BookFile } from '../api/bookApi';
import styles from './DownloadForm.module.css';

interface DownloadFormProps {
    bookFile: BookFile;
}

const DownloadForm: React.FC<DownloadFormProps> = ({ bookFile }) => {
    const blobUrl = useMemo(() => {
        const blob = new Blob([bookFile.Content], { type: bookFile.ContentType });
        const url = URL.createObjectURL(blob);
        return url;
    }, [bookFile]);

    return (
        <div className={styles.container}>
            <h3 className={styles.heading}>Download {bookFile.Name}</h3>
            <a
                href={blobUrl}
                download={bookFile.Name}
                className={styles.downloadLink}
            >
                Download
            </a>
        </div>
    );
};

export default DownloadForm;
