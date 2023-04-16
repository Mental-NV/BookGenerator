import React, { useState, useEffect } from 'react';
import { getBookStatus, GetStatusResponse } from '../api/bookApi';
import ProgressBar from './ProgressBar';

interface BookStatusProps {
    status: GetStatusResponse;
}

const BookStatus: React.FC<BookStatusProps> = ({ status }) => {

    return (
        <div>
            <h1>{status.BookTitle}</h1>
            <p>Status: {status.Status}</p>
            <ProgressBar progress={status.Progress} />
            {status.ErrorMessage && <p>Error: {status.ErrorMessage}</p>}
        </div>
    );
}

export default BookStatus;