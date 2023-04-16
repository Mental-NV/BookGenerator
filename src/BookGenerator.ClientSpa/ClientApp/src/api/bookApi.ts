import axios from "axios";

export interface CreateBookResponse {
    BookId: string;
}

export interface GetStatusResponse {
    BookTitle: string;
    Status: string;
    Progress: number;
    ErrorMessage: string;
}

const apiClient = axios.create({
    baseURL: import.meta.env.VITE_WEBAPI_BASE_URL,
    headers: {
        'Content-Type': 'application/json',
    },
});

export const createBook = async (title: string): Promise<CreateBookResponse> => {
    try {
        const response = await apiClient.post<CreateBookResponse>('/api/book', title);
        return response.data;
    } catch (error) {
        console.error('Error creating book:', error);
        throw error;
    }
};

export const getBookStatus = async (bookId: string): Promise<GetStatusResponse> => {
    try {
        const response = await apiClient.get<GetStatusResponse>(`/api/book/${bookId}`);
        return response.data;
    } catch (error) {
        console.error('Error retrieving book status:', error);
        throw error;
    }
};
