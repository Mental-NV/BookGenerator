import React from 'react';
import { BookFile } from '../api/bookApi';

interface DownloadFormProps {
    bookFile: BookFile;
}

const DownloadForm: React.FC<DownloadFormProps> = ({ bookFile }) => {
    const downloadBook = () => {
        const blob = new Blob([bookFile.Content], { type: bookFile.ContentType });
        const url = URL.createObjectURL(blob);

        const link = document.createElement('a');
        link.href = url;
        link.download = bookFile.Name;
        link.click();

        URL.revokeObjectURL(url);
    };

    return (
        <div>
            <h3>Download {bookFile.Name}</h3>
            <button onClick={downloadBook}>Download</button>
        </div>
    );
};

export default DownloadForm;
