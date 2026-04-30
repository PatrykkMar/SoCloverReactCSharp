import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import { SocketProvider } from './context/SocketContext';
import Home from './pages/Home';
import Game from './pages/Game';



function App() {

    return (
        <SocketProvider>
            <BrowserRouter>
                <Routes>
                    <Route path="/" element={<Home />} />
                    <Route path="/game" element={<Game />} />
                </Routes>
            </BrowserRouter>
        </SocketProvider>
    );

}

export default App;