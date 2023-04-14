import React from 'react'
import BookForm from './components/BookForm'

const App: React.FC = () => {

    const handleGenerate = (bookId: string) => {
        console.log(`Book generated with id: ${bookId}`);
        alert(`Book generated with id: ${bookId}`);
    }

    return (
        <div>
            <h1>Book Generator</h1>
            <BookForm onGenerate={handleGenerate} />
        </div>
    );
}

export default App;