import React from 'react';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import '@testing-library/jest-dom';
import BookForm from './BookForm';

describe('BookForm', () => {
    const onGenerateMock = jest.fn();

    beforeEach(() => {
        render(<BookForm onGenerate={onGenerateMock} />);
    });

    afterEach(() => {
        jest.clearAllMocks();
    });

    test('render book title input and generate button', () => {
        const input = screen.getByTestId(/bookTitleInput/i);
        const button = screen.getByTestId(/generateButton/i);

        expect(input).toBeInTheDocument();
        expect(button).toBeInTheDocument();
    });

    test('handles user input', async () => {
        const input = screen.getByTestId(/bookTitleInput/i);
        const testValue = 'Test book title';

        await userEvent.type(input, testValue);

        expect(input).toHaveValue(testValue);
    });

    test('handles form submit', async () => {
        const input = screen.getByTestId(/bookTitleInput/i);
        const form = screen.getByRole('form');
        const testValue = 'Test book title';

        await userEvent.type(input, testValue);
        fireEvent.submit(form);

        expect(onGenerateMock).toHaveBeenCalledTimes(1);
        expect(onGenerateMock).toHaveBeenCalledWith(testValue);
    });

    test('disable generate button after submittion', async () => {
        const input = screen.getByTestId(/bookTitleInput/i);
        const button = screen.getByTestId(/generateButton/i);
        const form = screen.getByRole('form');

        userEvent.type(input, 'Test book title');
        fireEvent.submit(form);
        expect(button).toBeDisabled();
    });
});