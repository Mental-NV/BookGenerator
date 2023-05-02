import React, { useState, useEffect } from 'react';
import { getBookStatus, GetStatusResponse } from '../api/bookApi';
import ProgressBar from './ProgressBar';

interface BookStatusProps {
    status: GetStatusResponse;
    width: number;
}

const BookStatus: React.FC<BookStatusProps> = ({ status, width }) => {

    return (
        <div className="flex flex-col min-h-screen justify-center items-center">
            <div className="w-full max-w-2xl flex flex-col">
                <h1 className="text-3xl font-bold text-center mb-4">{status.BookTitle}</h1>
                <div className="w-full bg-gray-200 rounded-full dark:bg-gray-700">
                    <div
                        className="bg-blue-600 text-sm font-medium text-blue-100 text-center p-1 leading-none rounded-full"
                        style={{ width: width + '%' }}>{status.Progress}%</div>
                </div>

            </div>
        </div>
    );
}

export default BookStatus;