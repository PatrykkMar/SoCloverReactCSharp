import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import { SocketProvider } from './context/SocketContext';
import Home from './pages/Home';
import Game from './pages/Game';
import { RoomProvider } from './context/RoomContext';



function App() {

    return (
        <SocketProvider>
            <RoomProvider>
                <BrowserRouter>
                    <Routes>
                        <Route path="/" element={<Home />} />
                        <Route path="/game" element={<Game />} />
                    </Routes>
                </BrowserRouter>
            </RoomProvider>
        </SocketProvider>
    );

}

export default App;