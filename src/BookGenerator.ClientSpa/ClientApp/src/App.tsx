import React, { lazy, Suspense } from 'react'
import { BrowserRouter as Router } from 'react-router-dom';
import { Route, Switch } from 'react-router-dom';

const StatusPage = lazy(() => import('./pages/StatusPage'));
const HomePage = lazy(() => import('./pages/HomePage'));
const DownloadPage = lazy(() => import('./pages/DownloadPage'));

const App: React.FC = () => {
    return (
        <Router>
            <Suspense fallback={<div>Loading...</div>}>
                <Switch>
                    <Route exact path="/" component={HomePage} />
                    <Route path="/status/:bookId" component={StatusPage} />
                    <Route path="/download/:bookId" component={DownloadPage} />
                </Switch>
            </Suspense>
        </Router>
    );
}

export default App;