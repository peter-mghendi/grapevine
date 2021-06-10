"use strict";

// TODO CancelToken support

// HTML elements
const startButton = document.getElementById("startButton");
const startInput = document.getElementById("startInput");
const countInput = document.getElementById("countInput");
const delayInput = document.getElementById("delayInput");

// Disable send button until connection is established
startButton.disabled = true;

// Create SignalR Connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/trigonometry")
    .build();

connection.start()
    .then(function () {
        console.log("SignalR connection established.");
        startButton.disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

startButton.addEventListener("click", function (event) {
    const start = parseInt(startInput.value);
    const count = parseInt(countInput.value);
    const delay = parseInt(delayInput.value);

    connection.stream("Trigonometries", start, count, delay)
        .subscribe({
            next: (item) => {
                const li = document.createElement("li");
                li.textContent = `{ number: ${item.num}, sine\t: ${item.sin}, cosine\t: ${item.cos}, tangent\t: ${item.tan} }`;
                document.getElementById("list").appendChild(li);
            },
            complete: () => console.log("Stream completed."),
            error: (err) => console.error(err),
        });

    event.preventDefault();
});