"use strict";

// HTML element references
const chartElement = document.getElementById('chart');

const startButton = document.getElementById("startButton");
const stopButton  = document.getElementById("stopButton");
const resetButton = document.getElementById("resetButton");

const startInput = document.getElementById("startInput");
const countInput = document.getElementById("countInput");
const delayInput = document.getElementById("delayInput");

// SignalR stream subscription
let subscription;

// Initial chart data
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

// Chart config
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

// Initialize chart
const chart = new Chart(chartElement.getContext('2d'), config);

// Create SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/trigonometry")
    .build();

// Start SignalR connection
connection.start()
    .then(function () {
        console.log("SignalR connection established.");
        startButton.disabled = false;
    }).catch(function (err) {
        return console.error(err.toString());
    });

// Start stream, update chart
startButton.addEventListener("click", function (event) {
    const Start = parseInt(startInput.value);
    const Count = parseInt(countInput.value);
    const Delay = parseInt(delayInput.value);

    console.log("Attempting to start SignalR stream...");

    // Subscribe to SignalR stream
    subscription = connection.stream("Trigonometries", { Start, Count, Delay })
        .subscribe({
            next: (item) => {
                const data = chart.data;

                // Make sure data.labels is array
                data.labels = Array.isArray(data.labels) ? data.labels : [data.labels];

                // Simulate scrolling effect
                if (data.labels.length > 20) {
                    data.labels.shift();
                    data.datasets[0].data.shift();
                    data.datasets[1].data.shift();
                }

                // Push incoming data onto chart
                data.labels.push(item.num);
                data.datasets[0].data.push(item.sin);
                data.datasets[1].data.push(item.cos);

                chart.update();
            },
            complete: () => {
                startButton.disabled = false;
                stopButton.disabled = true;
                resetButton.disabled = false;
                console.log("Stream completed.");
            },
            error: (err) => {
                startButton.disabled = false;
                stopButton.disabled = true;
                resetButton.disabled = false;
                console.error(err);
            },
        });

    startButton.disabled = true;
    stopButton.disabled = false;
    resetButton.disabled = true;

    event.preventDefault();
});

// Cancel stream
stopButton.addEventListener("click", function (event) {
    console.log("Attempting to cancel SignalR stream...");

    // Dispose SignalR stream subscription
    if (subscription) subscription.dispose();

    startButton.disabled = false;
    stopButton.disabled = true;
    resetButton.disabled = false;

    console.log("SignalR stream was cancelled.");
    event.preventDefault();
});

// Reset page to initial state
resetButton.addEventListener("click", function (event) {
    // Reset chart
    chart.data.labels = [];
    chart.data.datasets[0].data = [];
    chart.data.datasets[1].data = [];
    chart.update();

    // Reset inputs
    startInput.value = null;
    countInput.value = null;
    delayInput.value = null;

    // Reset controls
    startButton.disabled = false;
    stopButton.disabled = true;
    resetButton.disabled = true;

    event.preventDefault();
});