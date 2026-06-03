

document.addEventListener(
    "DOMContentLoaded",
    function () {

        const tags =
            document.getElementById(
                "tags-select");

        if (tags) {
            window.tomSelectTags =
                new TomSelect(
                    tags,
                    {
                        plugins:
                            [
                                "remove_button"
                            ]
                    });
        }

        const games =
            document.getElementById(
                "games-select");

        if (games) {
            window.tomSelectGames =
                new TomSelect(
                    games,
                    {
                        plugins:
                            [
                                "remove_button"
                            ]
                    });
        }

        // TAG MODAL

        document
            .getElementById(
                "open-tag-modal")
            ?.addEventListener(
                "click",
                () => {
                    document
                        .getElementById(
                            "tag-modal")
                        .classList
                        .remove(
                            "hidden");
                });

        document
            .getElementById(
                "close-tag-modal")
            ?.addEventListener(
                "click",
                () => {
                    document
                        .getElementById(
                            "tag-modal")
                        .classList
                        .add(
                            "hidden");
                });

        document
            .getElementById(
                "save-tag-btn")
            ?.addEventListener(
                "click",
                async () => {

                    const name =
                        document
                            .getElementById(
                                "tag-name")
                            .value
                            .trim();

                    if (!name) {
                        alert(
                            "Please enter a tag name.");

                        return;
                    }

                    const response =
                        await fetch(
                            "/Admin/Tags/QuickCreate",
                            {
                                method:
                                    "POST",

                                headers:
                                {
                                    "Content-Type":
                                        "application/json"
                                },

                                body:
                                    JSON.stringify(
                                        {
                                            name:
                                                name
                                        })
                            });

                    if (!response.ok) {
                        alert(
                            "Unable to create tag.");

                        return;
                    }

                    const tag =
                        await response.json();

                    window
                        .tomSelectTags
                        .addOption(
                            {
                                value:
                                    tag.id,

                                text:
                                    tag.name
                            });

                    window
                        .tomSelectTags
                        .addItem(
                            tag.id);

                    window
                        .tomSelectTags
                        .refreshOptions(
                            false);

                    document
                        .getElementById(
                            "tag-name")
                        .value = "";

                    document
                        .getElementById(
                            "tag-modal")
                        .classList
                        .add(
                            "hidden");

                });

        // GAME MODAL

        document
            .getElementById(
                "open-game-modal")
            ?.addEventListener(
                "click",
                () => {
                    document
                        .getElementById(
                            "game-modal")
                        .classList
                        .remove(
                            "hidden");
                });

        document
            .getElementById(
                "close-game-modal")
            ?.addEventListener(
                "click",
                () => {
                    document
                        .getElementById(
                            "game-modal")
                        .classList
                        .add(
                            "hidden");
                });

        document
            .getElementById(
                "save-game-btn")
            ?.addEventListener(
                "click",
                async () => {

                    const name =
                        document
                            .getElementById(
                                "game-name")
                            .value
                            .trim();

                    if (!name) {
                        alert(
                            "Please enter a game name.");

                        return;
                    }

                    const response =
                        await fetch(
                            "/Admin/Games/QuickCreate",
                            {
                                method:
                                    "POST",

                                headers:
                                {
                                    "Content-Type":
                                        "application/json"
                                },

                                body:
                                    JSON.stringify(
                                        {
                                            name:
                                                name
                                        })
                            });

                    if (!response.ok) {
                        alert(
                            "Unable to create game.");

                        return;
                    }

                    const game =
                        await response.json();

                    window
                        .tomSelectGames
                        .addOption(
                            {
                                value:
                                    game.id,

                                text:
                                    game.name
                            });

                    window
                        .tomSelectGames
                        .addItem(
                            game.id);

                    window
                        .tomSelectGames
                        .refreshOptions(
                            false);

                    document
                        .getElementById(
                            "game-name")
                        .value = "";

                    document
                        .getElementById(
                            "game-modal")
                        .classList
                        .add(
                            "hidden");

                });

    });
