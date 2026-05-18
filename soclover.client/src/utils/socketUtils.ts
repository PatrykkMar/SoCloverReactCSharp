export const getOrCreatePlayerGuid = (): string => {
    const KEY = "so_clover_player_guid";
    let id = localStorage.getItem(KEY);

    if (!id) {
        id = window.crypto.randomUUID();
        localStorage.setItem(KEY, id);
    }

    return id;
};