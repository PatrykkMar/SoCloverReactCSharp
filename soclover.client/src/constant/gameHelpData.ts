export interface HelpSection {
    title: string;
    description: string;
    tips?: string[];
}

export const STATE_INSTRUCTIONS: Record<string, HelpSection> = {
    "Lobby": {
        title: "Oczekiwanie na graczy ⏳",
        description: "Siedzicie w poczekalni pokoju. Gdy wszyscy znajdą się na liście po lewej stronie, ktoś musi kliknąć zielony przycisk \"Start Game\", aby uruchomić rozgrywkę.",
        tips: [
            "Skopiuj kod pokoju klikając ikonę schowka 📋 obok nazwy pokoju."
        ]
    },
    "Writing": {
        title: "Faza pisania podpowiedzi ✍️",
        description: "Twoja plansza została uzupełniona słowami. Znajdź wspólne mianowniki dla sąsiadujących par haseł i wpisz swoje podpowiedzi na krawędziach.",
        tips: [
            "Podpowiedź musi być pojedynczym słowem.",
            "Nie możesz używać słów o tym samym rdzeniu co hasła główne.",
            "Inni gracze nie widzą teraz Twoich kart!"
        ]
    },
    "Solving": {
        title: "Faza dedukcji i zgadywania 🧐",
        description: "Przed Wami plansza jednego z graczy! Wspólnie manipulujcie kartami haseł, aby dopasować je do podpowiedzi na krawędziach.",
        tips: [
            "Właściciel koniczynki musi zachować absolutne milczenie i kamienną twarz!",
            "Dążycie do zgadnięcia przy najmniejszej liczbie prób.",
            "Karty można obracać przyciskiem na środku karty. Można je też przeciągać myszką na planszę.",
            "Przeciągnięcie karty na zajęty slot planszy zwraca poprzednią kartę na slocie do ręki.",
            "Aby sprawdzić poprawność, należy wcisnąć przycisk \"Check\". Jeżeli jako drużyna się poddajecie, możecie wcisnąć \"Return to writing\"",
            "Poprawne sloty planszy pokolorywane są zielonym kolorem"
        ]
    },
};
