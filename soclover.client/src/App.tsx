import { BrowserRouter, Route, Routes } from 'react-router-dom';
import './App.css';
import { SocketProvider } from './context/SocketContext';
import Home from './pages/Home';
import Game from './pages/Game';
import { RoomProvider } from './context/RoomContext';
import { BoardProvider } from './context/BoardContext';
import LanguageSelector from './components/LanguageSelector';
import './i18n';

function App() {

    return (
        <>
            <LanguageSelector />
            <SocketProvider>
                <RoomProvider>
                    <BoardProvider>
                        <BrowserRouter>
                            <Routes>
                                <Route path="/" element={<Home />} />
                                <Route path="/game" element={<Game />} />
                            </Routes>
                        </BrowserRouter>
                    </BoardProvider>
                </RoomProvider>
            </SocketProvider>
        </>
    );

}

export default App;