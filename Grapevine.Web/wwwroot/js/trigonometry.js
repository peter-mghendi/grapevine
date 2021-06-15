"use strict";

// TODO CancelToken support


// HTML elements

const chartElement = document.getElementById('chart');
const startButton = document.getElementById("startButton");
const startInput = document.getElementById("startInput");
const countInput = document.getElementById("countInput");
const delayInput = document.getElementById("delayInput");

const data = {
    labels: [],
    datasets: [
        {
            label: 'Sine',
            data: [],
            borderColor: "#DC2626",
            backgroundColor: "#F87171",
            tension: 0.4
        },
        {
            label: 'Cosine',
            data: [],
            borderColor: "#059669",
            backgroundColor: "#34D399",
            tension: 0.4
        }
    ]
};

const config = {
    type: 'line',
    data: data,
    options: {
        responsive: true,
        plugins: {
            legend: {
                position: 'top',
            },
            title: {
                display: true,
                text: 'Trigonometry'
            }
        },
        scales: {
            x: {
                display: true,
                title: {
                    display: true,
                    text: 'Number'
                }
            },
            y: {
                display: true,
                title: {
                    display: true,
                    text: 'Value'
                },
                suggestedMin: -1,
                suggestedMax: 1
            }
        }
    }
};

var chart = new Chart(chartElement.getContext('2d'), config);

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
    const Start = parseInt(startInput.value);
    const Count = parseInt(countInput.value);
    const Delay = parseInt(delayInput.value);

    connection.stream("Trigonometries", { Start, Count, Delay })
        .subscribe({
            next: (item) => {
                const data = chart.data;
                data.labels = Array.isArray(data.labels) ? data.labels : [data.labels];

                if (data.labels.length > 20) {
                    data.labels.shift();
                    data.datasets[0].data.shift();
                    data.datasets[1].data.shift();
                }

                data.labels.push(item.num);
                data.datasets[0].data.push(item.sin);
                data.datasets[1].data.push(item.cos);

                chart.update();
            },
            complete: () => console.log("Stream completed."),
            error: (err) => console.error(err),
        });

    event.preventDefault();
});