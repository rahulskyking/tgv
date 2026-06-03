
//quick tag open
document
    .getElementById("open-tag-modal")
    ?.addEventListener("click", () => {

        document
            .getElementById("tag-modal")
            .classList.remove("hidden");

    });

document
    .getElementById("close-tag-modal")
    ?.addEventListener("click", () => {

        document
            .getElementById("tag-modal")
            .classList.add("hidden");

    });

document
    .getElementById("save-tag-btn")
    ?.addEventListener("click", async () => {

        const name =
            document
                .getElementById("tag-name")
                .value;

        const response =
            await fetch(
                "/Admin/Tags/QuickCreate",
                {
                    method: "POST",

                    headers:
                    {
                        "Content-Type":
                            "application/json"
                    },

                    body: JSON.stringify({
                        name
                    })
                });

        const tag =
            await response.json();

        const select =
            document.getElementById(
                "tags-select");

        const option =
            new Option(
                tag.name,
                tag.id,
                true,
                true);

        select.add(option);

        if (window.tomSelectTags) {
            window.tomSelectTags.addOption({
                value: tag.id,
                text: tag.name
            });

            window.tomSelectTags.addItem(
                tag.id);
        }

        document
            .getElementById("tag-modal")
            .classList.add("hidden");

    });

// Add Game modal
document
    .getElementById("open-game-modal")
    ?.addEventListener("click", () => {

        document
            .getElementById("game-modal")
            .classList.remove("hidden");

    });

document
    .getElementById("close-game-modal")
    ?.addEventListener("click", () => {

        document
            .getElementById("game-modal")
            .classList.add("hidden");

    });

document
    .getElementById("save-game-btn")
    ?.addEventListener("click", async () => {

        const name =
            document
                .getElementById("game-name")
                .value;

        const response =
            await fetch(
                "/Admin/Games/QuickCreate",
                {
                    method: "POST",

                    headers:
                    {
                        "Content-Type":
                            "application/json"
                    },

                    body: JSON.stringify({
                        name
                    })
                });

        const game =
            await response.json();

        const select =
            document.getElementById(
                "games-select");

        const option =
            new Option(
                game.name,
                game.id,
                true,
                true);

        select.add(option);

        if (window.tomSelectGames) {
            window.tomSelectGames.addOption({
                value: game.id,
                text: game.name
            });

            window.tomSelectGames.addItem(
                game.id);
        }

        document
            .getElementById("game-modal")
            .classList.add("hidden");

    });