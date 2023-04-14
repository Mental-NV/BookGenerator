import axios from "axios";

interface CreateBookResponse {
    BookId: string;
}

const apiClient = axios.create({
    baseURL: 'https://localhost:7195/api',
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
