"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/trigonometry").build();

// Disable send button until connection is established
document.getElementById("startButton").disabled = true;

connection.start().then(function () {
    console.log("Signal connection established.");
    document.getElementById("startButton").disabled = false;
}).catch(function (err) {
    return console.error(err.toString());
});

document.getElementById("startButton").addEventListener("click", function (event) {
    var count = parseInt(document.getElementById("countInput").value);
    var delay = parseInt(document.getElementById("delayInput").value);

    connection.stream("Counter", count, delay)
        .subscribe({
            next: (item) => {
                var li = document.createElement("li");
                li.textContent = item;
                document.getElementById("list").appendChild(li);
            },
            complete: () => {
                var li = document.createElement("li");
                li.textContent = "Stream completed";
                document.getElementById("list").appendChild(li);
            },
            error: (err) => {
                var li = document.createElement("li");
                li.textContent = err;
                document.getElementById("list").appendChild(li);
            },
        });

    event.preventDefault();
});