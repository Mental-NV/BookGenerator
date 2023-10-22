import React, { useState, useEffect } from 'react';
import { getBookStatus, GetStatusResponse } from '../api/bookApi';

interface BookStatusProps {
    status: GetStatusResponse;
    width: number;
}

const BookStatus: React.FC<BookStatusProps> = ({ status, width }) => {

    return (
        <div className="flex flex-col min-h-screen justify-center items-center">
            <div className="w-11/12 max-w-2xl h-52 flex flex-col">
                <h1 className="text-center text-slate-900 font-bold text-3xl mb-4">{status.BookTitle}</h1>
                <div className="h-10 w-full bg-gray-200 rounded-full dark:bg-gray-700">
                    <div
                        className="h-10 pt-3 bg-blue-600 text-sm font-medium text-blue-100 text-center p-1 leading-none rounded-full"
                        style={{ width: width + '%' }}>{status.Progress}%</div>
                </div>

            </div>
        </div>
    );
}

export default BookStatus;