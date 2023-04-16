import React from 'react';
import { BrowserRouter as Router } from 'react-router-dom';
import { Route, Switch } from 'react-router-dom';
import StatusPage from './StatusPage';
import HomePage from './HomePage';

const AppRouter: React.FC = () => {
    return (
        <Router>
            <Switch>
                <Route exact path="/" component={HomePage} />
                <Route path="/status/:bookId" component={StatusPage} />
            </Switch>
        </Router>
    );
};

export default AppRouter;