import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import BookStatus from './BookStatus';
import { getBookStatus, GetStatusResponse } from '../api/bookApi';

interface RouteParams {
    bookId: string;
}

const StatusPage: React.FC = () => {
    const { bookId } = useParams<RouteParams>();
    const [status, setStatus] = useState<GetStatusResponse | null>(null);

    const fetchStatus = async () => {
        console.log(`Fetching status for book ${bookId}...`);
        const statusData = await getBookStatus(bookId);

        if (statusData.Status !== 'Pending') {
            clearInterval(timer);
            console.log(`Book ${bookId} is no longer pending, clearing interval...`);
        }

        setStatus(statusData);
    }

    const [timer] = useState<NodeJS.Timeout>(setInterval(fetchStatus, 10000));

    useEffect(() => {
        console.log(`useEffect for book ${bookId}...`);
        fetchStatus();
        return () => clearInterval(timer);
    }, []);

    if (!status) {
        return <div>Loading...</div>;
    }

    return <BookStatus status={status} />;
};

export default StatusPage;