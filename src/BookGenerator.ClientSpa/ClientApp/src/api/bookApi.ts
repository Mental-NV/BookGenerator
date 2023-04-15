import axios from "axios";

interface CreateBookResponse {
    BookId: string;
}

const apiClient = axios.create({
    baseURL: `${import.meta.env.VITE_WEBAPI_BASE_URL}/api`,
    headers: {
        'Content-Type': 'application/json',
    },
});

export const createBook = async (title: string): Promise<CreateBookResponse> => {
    try {
        const response = await apiClient.post<CreateBookResponse>('/book', title);
        return response.data;
    } catch (error) {
        console.error('Error creating book:', error);
        throw error;
    }
};
