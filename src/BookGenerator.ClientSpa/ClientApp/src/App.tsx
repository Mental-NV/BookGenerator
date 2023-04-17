import React from 'react'
import { BrowserRouter as Router } from 'react-router-dom';
import { Route, Switch } from 'react-router-dom';
import StatusPage from './pages/StatusPage';
import HomePage from './pages/HomePage';
import DownloadPage from './pages/DownloadPage';

const App: React.FC = () => {
    return (
        <Router>
            <Switch>
                <Route exact path="/" component={HomePage} />
                <Route path="/status/:bookId" component={StatusPage} />
                <Route path="/download/:bookId" component={DownloadPage} />
            </Switch>
        </Router>
    );
}

export default App;