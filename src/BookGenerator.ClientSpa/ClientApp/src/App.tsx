import React from 'react'
import BookForm from './components/BookForm'

const App: React.FC = () => {

    const handleGenerate = (title: string) => {
        console.log(title);
    }

    return (
        <div>
            <h1>Book Generator</h1>
            <BookForm onGenerate={handleGenerate} />
        </div>
    );
}

export default App;