import React, { useMemo } from 'react';
import { BookFile } from '../api/bookApi';

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
        <div>
            <h3>Download {bookFile.Name}</h3>
            <a
                href={blobUrl}
                download={bookFile.Name}
            >
                Download
            </a>
        </div>
    );
};

export default DownloadForm;
