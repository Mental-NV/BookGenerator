import axios, { AxiosResponse } from "axios";

export interface CreateBookResponse {
    BookId: string;
}

export interface GetStatusResponse {
    BookTitle: string;
    Status: string;
    Progress: number;
    ErrorMessage: string;
}

export interface BookFile {
    Name: string;
    ContentType: string;
    Content: Blob;
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

export const getBookFile = async (bookId: string): Promise<BookFile> => {
    try {
        const response: AxiosResponse<Blob> = await apiClient.get<Blob>(`/api/book/download2/${bookId}`, {
            responseType: 'blob',
        });

        const contentDisposition = response.headers['content-disposition'];
        const contentType = response.headers['content-type'];

        // Regular expression to extract the file name
        const filenameRegex = /filename[^;=\n]*=((['"]).*?\2|[^;\n]*)/;

        // Match the regex against the Content-Disposition header
        const matches = filenameRegex.exec(contentDisposition);

        // Extract the file name from the matches and remove double quotes
        const fileName = matches && matches[1] ? matches[1].replace(/['"]+/g, '') : 'unknown';

        return {
            Name: fileName,
            ContentType: contentType,
            Content: response.data,
        };
    } catch (error) {
        console.error('Error retrieving book file:', error);
        throw error;
    }
};

