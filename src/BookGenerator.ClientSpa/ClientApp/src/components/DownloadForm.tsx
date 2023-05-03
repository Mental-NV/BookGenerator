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
        <div className="flex flex-col min-h-screen justify-center items-center">
            <div className="w-full max-w-2xl h-52 flex flex-col">
                <h1 className="text-center text-slate-900 font-bold text-3xl mb-4">Download {bookFile.Name}</h1>
                <div className="flex flex-col justify-center items-center">
                    <a
                        href={blobUrl}
                        download={bookFile.Name}
                    >
                        <button
                            type="button"
                            className="w-40 px-4 py-3 bg-blue-600 rounded-md text-white outline-none focus:ring-4 shadow-lg transform active:scale-x-75 transition-transform mx-5 flex"
                        >
                            <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4" />
                            </svg>

                            <span className="ml-2">Download</span>
                        </button>
                    </a>
                </div>
            </div>
        </div>
    );
};

export default DownloadForm;
