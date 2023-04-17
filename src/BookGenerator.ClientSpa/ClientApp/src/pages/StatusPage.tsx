import React, { useEffect, useState } from 'react';
import { useParams, useHistory } from 'react-router-dom';
import BookStatus from '../components/BookStatus';
import { getBookStatus, GetStatusResponse } from '../api/bookApi';

interface RouteParams {
    bookId: string;
}

const StatusPage: React.FC = () => {
    const { bookId } = useParams<RouteParams>();
    const [status, setStatus] = useState<GetStatusResponse | null>(null);
    const history = useHistory();

    const fetchStatus = async () => {
        console.log(`Fetching status for book ${bookId}`);
        const statusData = await getBookStatus(bookId);
        setStatus(statusData);
    }

    useEffect(() => {
        console.log(`StatusPage mounted for book ${bookId}`);
        let timer: NodeJS.Timeout = setInterval(fetchStatus, 10000);
        return () => {
            console.log(`StatusPage unmounted for book ${bookId}`);
            clearInterval(timer);
        }
    }, [bookId]);

    useEffect(() => {
        if (status?.Status == 'Completed') {
            history.push(`/download/${bookId}`);
        }
    }, [status]);

    if (!status) {
        return <div>Loading...</div>;
    }

    return <BookStatus status={status} />;
};

export default StatusPage;