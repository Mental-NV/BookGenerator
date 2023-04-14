import React from 'react';
import { BrowserRouter as Router, Route, Switch } from 'react-router-dom';
import BookForm from './BookForm';

const AppRouter: React.FC = () => {
    const handleGenerate = (bookId: string) => {
        console.log(`Book generated with id: ${bookId}`);
        alert(`Book generated with id: ${bookId}`);
    }

    return (
        <Router>
            <Switch>
                <Route path="/">
                    <BookForm onGenerate={handleGenerate} />
                </Route>
            </Switch>
        </Router>
    );
};

export default AppRouter;