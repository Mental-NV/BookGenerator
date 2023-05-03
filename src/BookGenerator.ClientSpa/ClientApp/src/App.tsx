import React, { lazy, Suspense } from 'react'
import { BrowserRouter as Router } from 'react-router-dom';
import { Route, Switch } from 'react-router-dom';

const StatusPage = lazy(() => import('./pages/StatusPage'));
const HomePage = lazy(() => import('./pages/HomePage'));
const DownloadPage = lazy(() => import('./pages/DownloadPage'));

const App: React.FC = () => {
    return (
        <Router>
            <Suspense
                fallback={<div className="flex flex-col min-h-screen justify-center items-center">
                    <div className="w-full max-w-2xl h-52 flex flex-col">
                        <p className="text-center">
                            Loading...
                        </p>
                    </div>
                </div>}>
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